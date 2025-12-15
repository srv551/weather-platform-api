using System.Net;
using System.Text.Json;
using WeatherApi.Application.DTOs;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace WeatherApi.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // If the response has already started, we cannot reliably write a JSON body.
                if (context.Response.HasStarted)
                {
                    // Log at a high level and rethrow to allow server to handle the partial response.
                    var traceWhenStarted = context.TraceIdentifier;
                    _logger.LogError(ex, "Unhandled exception after response started. TraceId: {TraceId}", traceWhenStarted);
                    throw;
                }

                // Use the request's trace id so logs and client responses can be correlated.
                var traceId = Activity.Current?.Id ?? context.TraceIdentifier ?? Guid.NewGuid().ToString();

                // Choose a status code based on exception type. Tune this to your API semantics.
                var statusCode = MapExceptionToStatusCode(ex);

                // Build a safe error response (do not expose exception details in non-development environments).
                var err = new ErrorResponse
                {
                    Code = MapExceptionToErrorCode(ex),
                    Message = _env.IsDevelopment() ? $"{ex.Message}" : "An unexpected error occurred.",
                    TraceId = traceId
                };

                // Log the exception with the trace id and request info.
                _logger.LogError(ex, "Unhandled exception occurred. TraceId: {TraceId}, Method: {Method}, Path: {Path}",
                    traceId, context.Request.Method, context.Request.Path);

                // Clear any existing response state and write a fresh JSON body with an appropriate content-type.
                context.Response.Clear();
                context.Response.StatusCode = (int)statusCode;
                context.Response.ContentType = "application/json";
                // Add a trace id header for client-side correlation.
                context.Response.Headers["X-Trace-Id"] = traceId;

                // Serialize using default options (adjust if you have custom settings)
                var json = JsonSerializer.Serialize(err, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                await context.Response.WriteAsync(json);
            }
        }

        private static HttpStatusCode MapExceptionToStatusCode(Exception ex)
        {
            // Map specific exceptions to more accurate HTTP status codes.
            // Adjust mappings to match your API error contract.
            return ex switch
            {
                TaskCanceledException _ => HttpStatusCode.RequestTimeout, // likely a timeout
                OperationCanceledException _ => (HttpStatusCode)499, // client closed / cancelled             
                HttpRequestException _ => HttpStatusCode.BadGateway,
                UnauthorizedAccessException _ => HttpStatusCode.Unauthorized,
                KeyNotFoundException _ => HttpStatusCode.NotFound,
                ArgumentException _ or ArgumentNullException _ or FormatException _ => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };
        }

        private static string MapExceptionToErrorCode(Exception ex)
        {
            return ex switch
            {
                TaskCanceledException _ => "REQUEST_TIMEOUT",
                OperationCanceledException _ => "REQUEST_CANCELLED",
                HttpRequestException _ => "UPSTREAM_ERROR",
                UnauthorizedAccessException _ => "UNAUTHORIZED",
                KeyNotFoundException _ => "NOT_FOUND",
                ArgumentException _ or ArgumentNullException _ or FormatException _ => "INVALID_INPUT",
                _ => "UNEXPECTED_ERROR"
            };
        }
    }

    // Extension method for easier registration
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
