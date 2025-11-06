using Agent.Models;

namespace Agent.Services
{
    public class ServiceRegistrationService : IServiceRegistrationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AgentConfiguration _configuration;

        public ServiceRegistrationService(
            IHttpClientFactory httpClientFactory,
            AgentConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private HttpClient CreateConfiguredHttpClient()
        {
            var httpClient = _httpClientFactory.CreateClient();

            // Set base address from AgentConfiguration.CacheServerUrl
            if (!string.IsNullOrEmpty(_configuration.CacheServerUrl))
            {
                httpClient.BaseAddress = new Uri(_configuration.CacheServerUrl);
            }

            // Add API key to headers if configured
            if (!string.IsNullOrEmpty(_configuration.ApiKey))
            {
                httpClient.DefaultRequestHeaders.Add("x-api-key", _configuration.ApiKey);
            }

            return httpClient;
        }

        public async Task RegisterServiceAsync(RegisterService registerService, CancellationToken ct = default)
        {
            using var _http = CreateConfiguredHttpClient();

            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(registerService), System.Text.Encoding.UTF8, "application/json");

            var response = await _http.PostAsync($"callback/register-service", content, ct);

            // Optionally handle response status
            response.EnsureSuccessStatusCode();
        }
    }
}