using Agent.Models;

namespace Agent.Services
{
    public interface IAgentService
    {
        Task<List<ServiceFabricAgentResponse>> SendOperation(string serviceName, Func<string, Task<ServiceFabricAgentResponse>> operation, string? id = null);
    }
}