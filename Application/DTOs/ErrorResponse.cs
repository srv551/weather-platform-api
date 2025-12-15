namespace WeatherApi.Application.DTOs
{
    /// <summary>
    /// Represents the standard error format returned by all API endpoints.
    /// Used for both handled and unhandled error scenarios.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// A machine-readable error code (e.g., "NOT_FOUND", "INVALID_INPUT", "UNAUTHORIZED").
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// A human-readable description of the error.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// A unique identifier that correlates this error with server logs (typically the request TraceId).
        /// </summary>
        public string TraceId { get; set; } = string.Empty;
    }
}
