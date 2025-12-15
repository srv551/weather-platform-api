using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using WeatherApi.Infrastructure.Config;

namespace WeatherApi.Api.Controllers
{
    public class ProviderHealth
    {
        public string Name { get; set; } = "WeatherAPI.com";
        public string Status { get; set; } = "Unknown";       // Online / Error / Offline
        public string StatusCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public long LatencyMs { get; set; }
    }

    public class DiskMetrics
    {
        public string DriveName { get; set; } = string.Empty;
        public long TotalSpaceMb { get; set; }
        public long FreeSpaceMb { get; set; }
    }

    public class SystemMetrics
    {
        public string MachineName { get; set; } = string.Empty;
        public string OSDescription { get; set; } = string.Empty;
        public int ProcessorCount { get; set; }
        public double ProcessUptimeSeconds { get; set; }
        public double ProcessMemoryMb { get; set; }
        public double ManagedHeapMb { get; set; }
        public DiskMetrics Disk { get; set; } = new();
    }

    public class HealthResponse
    {
        public string Status { get; set; } = "Healthy";
        public string Message { get; set; } = "Weather API is running";
        public DateTime ServerTimeUtc { get; set; }
        public ProviderHealth Provider { get; set; } = new();
        public SystemMetrics System { get; set; } = new();
    }

    /// <summary>
    /// Provides health and diagnostics information about the API and its external dependencies.
    /// </summary>
    [ApiController]
    [EnableRateLimiting("global")]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly WeatherApiOptions _options;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HealthController> _logger;

        public HealthController(
            IOptions<WeatherApiOptions> options,
            IHttpClientFactory httpClientFactory,
            ILogger<HealthController> logger)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Returns the current health status of the API and its WeatherAPI provider.
        /// </summary>
        /// <response code="200">The API is healthy and the provider is reachable.</response>
        /// <response code="503">The API is running but the external provider is unavailable or unhealthy.</response>
        [HttpGet]
        [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> CheckHealth(CancellationToken cancellationToken)
        {
            var response = new HealthResponse
            {
                ServerTimeUtc = DateTime.UtcNow,
                System = GetSystemMetrics()
            };

            // Use a named client 'WeatherApi' configured in DI (recommended).
            // Fallback to default client if named client is not registered.
            var client = CreateHttpClientSafely("WeatherApi");

            // If base address set in options and client has no BaseAddress, set it (non-destructive).
            if (!string.IsNullOrWhiteSpace(_options.BaseUrl) && client.BaseAddress == null)
            {
                if (Uri.TryCreate(_options.BaseUrl, UriKind.Absolute, out var baseAddr))
                {
                    client.BaseAddress = baseAddr;
                }
            }

            // If API key missing, return 503 (since provider cannot be contacted)
            if (string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                response.Provider.Status = "Error";
                response.Provider.StatusCode = "MissingApiKey";
                response.Provider.Message = "Weather provider API key is not configured.";
                _logger.LogWarning("Health check: API key missing for Weather provider.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, response);
            }

            try
            {
                // Build query in a safe manner (avoid accidental logging of keys)
                var query = new Dictionary<string, string>
                {
                    ["key"] = _options.ApiKey,
                    ["q"] = "London",
                    ["aqi"] = "no"
                };

                var requestUri = QueryHelpers.AddQueryString("current.json", query);

                var sw = Stopwatch.StartNew();
                using var providerResponse = await client.GetAsync(requestUri, cancellationToken);
                sw.Stop();

                response.Provider.LatencyMs = sw.ElapsedMilliseconds;
                response.Provider.StatusCode = ((int)providerResponse.StatusCode).ToString();

                if (providerResponse.IsSuccessStatusCode)
                {
                    response.Provider.Status = "Online";
                    response.Provider.Message = "Success";
                    return Ok(response);
                }

                // Non-success (4xx/5xx)
                response.Provider.Status = "Error";
                response.Provider.Message = "Provider returned non-success response (invalid key, quota, or provider error).";

                // Log details for diagnostics (headers and status only — avoid body/logging key)
                _logger.LogWarning("Provider health check returned {StatusCode} for Weather provider. Reason: {ReasonPhrase}",
                    providerResponse.StatusCode, providerResponse.ReasonPhrase);

                return StatusCode(StatusCodes.Status503ServiceUnavailable, response);
            }
            catch (OperationCanceledException oce) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation(oce, "Health check cancelled by caller.");
                response.Provider.Status = "Offline";
                response.Provider.StatusCode = "Cancelled";
                response.Provider.Message = "Health check cancelled.";
                return StatusCode(StatusCodes.Status503ServiceUnavailable, response);
            }
            catch (TaskCanceledException tce) // likely timeout
            {
                _logger.LogError(tce, "Health check timed out when contacting provider.");
                response.Provider.Status = "Offline";
                response.Provider.StatusCode = "Timeout";
                response.Provider.Message = "Provider request timed out.";
                return StatusCode(StatusCodes.Status503ServiceUnavailable, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WeatherAPI health check failed with exception.");

                response.Provider.Status = "Offline";
                response.Provider.StatusCode = "Exception";
                response.Provider.Message = "An error occurred while checking provider health.";

                return StatusCode(StatusCodes.Status503ServiceUnavailable, response);
            }
        }

        private HttpClient CreateHttpClientSafely(string name)
        {
            // Try to create named client, fall back to default client if named not registered
            try
            {
                return _httpClientFactory.CreateClient(name);
            }
            catch
            {
                // If named client is not registered, this may throw; return the default client instead.
                return _httpClientFactory.CreateClient();
            }
        }

        private static SystemMetrics GetSystemMetrics()
        {
            var process = Process.GetCurrentProcess();

            // Guard: Process.StartTime can throw in some restricted environments; handle defensively.
            DateTime startTimeUtc;
            try
            {
                startTimeUtc = process.StartTime.ToUniversalTime();
            }
            catch
            {
                startTimeUtc = DateTime.UtcNow;
            }

            var uptimeSeconds = Math.Max(0, (DateTime.UtcNow - startTimeUtc).TotalSeconds);
            var processMemoryMb = process.WorkingSet64 / (1024.0 * 1024.0);
            var managedHeapMb = GC.GetTotalMemory(forceFullCollection: false) / (1024.0 * 1024.0);

            var disk = GetDiskMetrics();

            return new SystemMetrics
            {
                MachineName = Environment.MachineName,
                OSDescription = RuntimeInformation.OSDescription,
                ProcessorCount = Environment.ProcessorCount,
                ProcessUptimeSeconds = Math.Round(uptimeSeconds, 2),
                ProcessMemoryMb = Math.Round(processMemoryMb, 2),
                ManagedHeapMb = Math.Round(managedHeapMb, 2),
                Disk = disk
            };
        }

        private static DiskMetrics GetDiskMetrics()
        {
            try
            {
                var baseDir = AppContext.BaseDirectory;
                var root = Path.GetPathRoot(baseDir);

                // If root is null or empty (container / non-Windows), fall back to first ready drive or '/'
                if (string.IsNullOrEmpty(root))
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        root = "C:\\";
                    }
                    else
                    {
                        root = "/";
                    }
                }

                var drives = DriveInfo.GetDrives().Where(d => d.IsReady).ToArray();

                DriveInfo? drive = null;

                if (!string.IsNullOrEmpty(root))
                {
                    drive = drives.FirstOrDefault(d =>
                        string.Equals(d.RootDirectory.FullName, root, StringComparison.OrdinalIgnoreCase));
                }

                drive ??= drives.FirstOrDefault();

                if (drive == null)
                {
                    return new DiskMetrics
                    {
                        DriveName = "Unknown",
                        TotalSpaceMb = 0,
                        FreeSpaceMb = 0
                    };
                }

                return new DiskMetrics
                {
                    DriveName = drive.Name,
                    TotalSpaceMb = (long)(drive.TotalSize / (1024.0 * 1024.0)),
                    FreeSpaceMb = (long)(drive.AvailableFreeSpace / (1024.0 * 1024.0))
                };
            }
            catch (Exception ex)
            {
                // Do not throw from metrics retrieval — return defaults and log at call site if needed.
                return new DiskMetrics
                {
                    DriveName = "Unknown",
                    TotalSpaceMb = 0,
                    FreeSpaceMb = 0
                };
            }
        }
    }
}
