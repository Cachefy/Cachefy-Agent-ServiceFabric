using Agent.Models;
using Newtonsoft.Json;

namespace Agent.Services
{
    public class CacheService : ICacheService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CacheService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ServiceFabricAgentResponse> ClearAllCacheAsync(string baseUrl, CancellationToken ct = default)
        {
            try
            {
                using var _http = _httpClientFactory.CreateClient();

                var resp = await _http.DeleteAsync($"{baseUrl}/cache/flushall", ct);

                var body = await resp.Content.ReadAsStringAsync();

                var agentResponse = JsonConvert.DeserializeObject<ServiceFabricAgentResponse>(body)!;

                return agentResponse;
            }
            catch (Exception ex)
            {
                return new ServiceFabricAgentResponse
                {
                    ParametersDetails = new List<ParametersDetails>
                    {
                        new ParametersDetails
                        {
                            Name = "Keys",
                            Parameters = new Dictionary<string, string>
                            {
                                { "Error", ex.Message }
                            }
                        }
                    }
                };
            }
        }

        public async Task<ServiceFabricAgentResponse> ClearCacheByKeyAsync(string baseUrl, string key, CancellationToken ct = default)
        {
            try
            {
                using var _http = _httpClientFactory.CreateClient();

                var resp = await _http.DeleteAsync($"{baseUrl}/cache?key={key}", ct);

                var body = await resp.Content.ReadAsStringAsync();

                var agentResponse = JsonConvert.DeserializeObject<ServiceFabricAgentResponse>(body)!;

                return agentResponse;
            }
            catch (Exception ex)
            {
                return new ServiceFabricAgentResponse
                {
                    ParametersDetails = new List<ParametersDetails>
                    {
                        new ParametersDetails
                        {
                            Name = key,
                            Parameters = new Dictionary<string, string>
                            {
                                { "Error", ex.Message }
                            }
                        }
                    }
                };
            }
        }

        public async Task<ServiceFabricAgentResponse> GetAllKeysAsync(string baseUrl, CancellationToken ct = default)
        {
            using var _http = _httpClientFactory.CreateClient();

            var resp = await _http.GetAsync($"{baseUrl}/cache/keys", ct);

            var body = await resp.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<IEnumerable<object>>(body)!;

            ServiceFabricAgentResponse agentResponse = new()
            {
                CacheKeys = result,
            };

            return agentResponse;
        }

        public async Task<ServiceFabricAgentResponse> GetCacheByKeyAsync(string baseUrl, string key, CancellationToken ct = default)
        {
            using var _http = _httpClientFactory.CreateClient();

            var resp = await _http.GetAsync($"{baseUrl}/cache?key={key}", ct);

            var body = await resp.Content.ReadAsStringAsync();

            ServiceFabricAgentResponse agentResponse = new()
            {
                CacheResult = body
            };

            return agentResponse;
        }
    }
}
