using Agent.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace Agent.Middleware
{
    /// <summary>
    /// Middleware that validates an incoming API key header against the configured key.
    /// Returns 401 (Unauthorized) when the header is missing or does not match.
    /// </summary>
    public class ApiKeyValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _configuredApiKey;

        // Header name as per requirement (note: user specified "x-api-key").
        public const string HeaderName = "x-api-key"; // Intentionally using provided spelling.

        public ApiKeyValidationMiddleware(RequestDelegate next, IOptions<AgentConfiguration> options)
        {
            _next = next;
            _configuredApiKey = options.Value.ApiKey ?? string.Empty;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip API key validation for ServiceRegistration endpoints
            if (context.Request.Path.StartsWithSegments("/api/ServiceRegistration", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            // If no API key configured we can either block all or skip. Requirement implies enforcement, so block.
            if (string.IsNullOrWhiteSpace(_configuredApiKey))
            {
                await WriteUnauthorizedAsync(context, "API key is not configured on server.");
                return;
            }

            if (!context.Request.Headers.TryGetValue(HeaderName, out var provided) || provided.Count == 0)
            {
                await WriteUnauthorizedAsync(context, "Missing API key header.");
                return;
            }

            // Constant time comparison to mitigate timing attacks (here simplified)
            if (!CryptographicEquals(_configuredApiKey, provided[0]!))
            {
                await WriteUnauthorizedAsync(context, "Invalid API key.");
                return;
            }

            await _next(context);
        }

        private static bool CryptographicEquals(string a, string b)
        {
            if (a.Length != b.Length) return false;
            var result = 0;
            for (int i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }
            return result == 0;
        }

        private static async Task WriteUnauthorizedAsync(HttpContext context, string message)
        {
            if (context.Response.HasStarted) return;
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";
            var payload = new { error = message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }

    public static class ApiKeyValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiKeyValidation(this IApplicationBuilder app)
            => app.UseMiddleware<ApiKeyValidationMiddleware>();
    }
}
