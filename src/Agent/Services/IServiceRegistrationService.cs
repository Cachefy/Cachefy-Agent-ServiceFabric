using Agent.Models;

namespace Agent.Services
{
    public interface IServiceRegistrationService
    {
        Task RegisterServiceAsync(RegisterService registerService, CancellationToken ct = default);
    }
}