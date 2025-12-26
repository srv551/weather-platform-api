using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Http.Resilience;
using OpenTelemetry;
using OpenTelemetry.Instrumentation.Runtime;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;
using Swashbuckle.AspNetCore.Filters;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using WeatherApi.Api.Middleware;
using WeatherApi.Api.Swagger.Examples;
using WeatherApi.Application.Interfaces;
using WeatherApi.Infrastructure.Config;
using WeatherApi.Infrastructure.Services;

namespace WeatherApi.Api
{
    /// <summary>
    /// Application entry point for the WeatherApi Web API.
    /// Configures services, middleware, OpenAPI, OpenTelemetry, and host execution.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Builds and runs the web application.
        /// This method mirrors the original top-level program file but exposes an explicit entry point
        /// to allow XML documentation and easier tooling support.
        /// </summary>
        /// <param name="args">Command-line arguments forwarded to the host.</param>
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
            .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);


            // Controllers
            builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter());
            });

            // -------- CORS --------
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("PublicCorsPolicy", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });


            // Swagger (OpenAPI)
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                // XML comments from API project
                var apiXml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var apiXmlPath = Path.Combine(AppContext.BaseDirectory, apiXml);
                if (File.Exists(apiXmlPath))
                {
                    options.IncludeXmlComments(apiXmlPath);
                }

                // XML comments from Application project (DTOs)
                var appXml = "WeatherApi.Application.xml";
                var appXmlPath = Path.Combine(AppContext.BaseDirectory, appXml);
                if (File.Exists(appXmlPath))
                {
                    options.IncludeXmlComments(appXmlPath);
                }

                // Examples
                options.ExampleFilters();

                options.UseInlineDefinitionsForEnums();
            });

            // register Swagger examples (from API assembly)
            builder.Services.AddSwaggerExamplesFromAssemblyOf<WeatherResultExample>();

            // Memory cache
            builder.Services.AddMemoryCache();

            // Today summary + advice + travel score services
            builder.Services.AddScoped<ITodaySummaryService, TodaySummaryService>();
            builder.Services.AddScoped<IWeatherAdviceService, WeatherAdviceService>();
            builder.Services.AddScoped<ITravelScoreService, TravelScoreService>();
            builder.Services.AddScoped<IOccupationWeatherService, OccupationWeatherService>();
            builder.Services.AddScoped<IHealthWeatherService, HealthWeatherService>();


            // Bind WeatherApiOptions
            builder.Services.Configure<WeatherApiOptions>(
                builder.Configuration.GetSection("WeatherApi"));

            // HttpClient + .NET 8 resilience
            builder.Services
                .AddHttpClient<IWeatherService, WeatherApiService>()
                .AddStandardResilienceHandler();

            // -------- API Versioning --------
            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;

                // URL segment versioning: /api/v1/weather
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";        // v1, v1.1, etc.
                options.SubstituteApiVersionInUrl = true;  // replace {version:apiVersion} in routes
            });

            // -------- Rate Limiting --------
            builder.Services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("global", limiterOptions =>
                {
                    limiterOptions.PermitLimit = 60;       // 60 req/min
                    limiterOptions.Window = TimeSpan.FromMinutes(1);
                    limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    limiterOptions.QueueLimit = 5;
                });
            });

            // -------- OpenTelemetry (traces + metrics + logs) --------
            var serviceName = "WeatherApi";
            var serviceVersion = "1.0.0";

            builder.Services.AddOpenTelemetry()
                .ConfigureResource(resource =>
                {
                    resource.AddService(serviceName: serviceName, serviceVersion: serviceVersion);
                })
                .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        // runtime instrumentation is only for metrics, not tracing
                        .AddConsoleExporter();
                })
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()   // ✅ valid here
                        .AddPrometheusExporter();      // exposes metrics for /metrics
                });

            // Send logs through OpenTelemetry to console
            builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.IncludeScopes = true;
                logging.ParseStateValues = true;
                logging.AddConsoleExporter();
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                // Expose OpenAPI docs at /openapi/v1.json, /openapi/v2.json, ...
                app.UseSwagger(options =>
                {
                    options.RouteTemplate = "openapi/{documentName}.json";
                });

                // Scalar UI at /docs
                app.MapScalarApiReference("/docs", options =>
                {
                    options
                        .WithTitle("Weather Platform API")
                        .WithClassicLayout()
                        //.ForceDarkMode()
                        .ExpandAllTags()
                        .SortTagsAlphabetically()
                        .SortOperationsByMethod();
                    // Default OpenAPI route pattern is /openapi/{documentName}.json, so no need to override.
                });

                // (Optional) you can also enable Swagger UI if you still want it:
                // var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                // app.UseSwaggerUI(options =>
                // {
                //     foreach (var description in provider.ApiVersionDescriptions)
                //     {
                //         options.SwaggerEndpoint(
                //             $"/openapi/{description.GroupName}.json",
                //             description.GroupName.ToUpperInvariant());
                //     }
                // });
            }

            app.UseApiExceptionHandling();

            app.UseHttpsRedirection();

            app.UseCors("PublicCorsPolicy");

            app.UseAuthorization();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseRateLimiter();

            // OTEL /metrics endpoint for Prometheus scraping
            app.UseOpenTelemetryPrometheusScrapingEndpoint();

            // Redirect root to docs
            app.MapGet("/", () => Results.Redirect("/docs"))
               .ExcludeFromDescription();

            app.MapControllers();
            await app.RunAsync();
        }
    }
}
