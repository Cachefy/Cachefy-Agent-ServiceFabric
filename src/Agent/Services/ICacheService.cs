using Agent.Models;

namespace Agent.Services
{
    public interface ICacheService
    {
        Task<ServiceFabricAgentResponse> ClearCacheByKeyAsync(string baseUrl, string key, CancellationToken ct = default);

        Task<ServiceFabricAgentResponse> ClearAllCacheAsync(string baseUrl, CancellationToken ct = default);

        Task<ServiceFabricAgentResponse> GetCacheByKeyAsync(string baseUrl, string key, CancellationToken ct = default);

        Task<ServiceFabricAgentResponse> GetAllKeysAsync(string baseUrl, CancellationToken ct = default);
    }
}
