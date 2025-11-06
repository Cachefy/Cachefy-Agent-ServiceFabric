using Agent.Middleware;
using Agent.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Agent
{
    public static class Startup
    {
        public static IConfiguration Configuration { get; }

        public static void ConfigureServices(IServiceCollection services)
        {
            // Add HttpClient factory for external API calls
            services.AddHttpClient("default")
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler
                    {
                        // For Azure App Service, ensure TLS 1.2 or higher is used
                        SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13,
                        // Only disable certificate validation in development
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                });

            // Also add default HttpClient without specific config for compatibility
            services.AddHttpClient();

            services.AddControllers();

            // Register services
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IAgentService, AgentService>();
            services.AddScoped<IServiceRegistrationService, ServiceRegistrationService>();
        }

        public static void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            // API key validation middleware placed after routing (so endpoints are identified)
            // but before endpoints execute.
            app.UseApiKeyValidation();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // This maps API controllers
            });
        }
    }
}