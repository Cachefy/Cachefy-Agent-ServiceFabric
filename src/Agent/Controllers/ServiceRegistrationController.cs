using Agent.Models;
using Agent.Services;
using Microsoft.AspNetCore.Mvc;

namespace Agent.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceRegistrationController : ControllerBase
    {
        private readonly IServiceRegistrationService _serviceRegistrationService;

        public ServiceRegistrationController(IServiceRegistrationService serviceRegistrationService)
        {
            _serviceRegistrationService = serviceRegistrationService;
        }

        /// <summary>
        /// Register a service with the cache manager server using the configured API key
        /// </summary>
        /// <param name="serviceName">Name of the service to register</param>
        /// <param name="ct">Cancellation token</param>
        [HttpPost()]
        public async Task<IActionResult> RegisterService([FromBody] RegisterService serviceRequest)
        {
            try
            {
                await _serviceRegistrationService.RegisterServiceAsync(serviceRequest);
                return Ok(new { Message = $"Service '{serviceRequest.Name}' registered successfully" });
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(new { Error = "Registration failed", Details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Internal server error", Details = ex.Message });
            }
        }
    }
}