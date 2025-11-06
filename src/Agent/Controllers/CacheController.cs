using Agent.Services;
using Microsoft.AspNetCore.Mvc;

namespace Agent.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CacheController : ControllerBase
    {
        private readonly IAgentService _agentService;
        private readonly ICacheService _cacheService;

        public CacheController(IAgentService agentService, ICacheService cacheService)
        {
            _agentService = agentService;
            _cacheService = cacheService;
        }

        // Admin endpoint to trigger cluster-wide cache clear
        //Agent endpoint
        [HttpDelete]
        public async Task<IActionResult> ClearByKey([FromQuery] string serviceIdentifier, [FromQuery] string key)
        {
            var result = await _agentService.SendOperation(serviceIdentifier, async (baseUrl) => await _cacheService.ClearCacheByKeyAsync(baseUrl, key, default));
            return Ok(result);
        }

        // Admin endpoint to trigger cluster-wide cache clear
        //Agent endpoint
        [HttpDelete("flushall")]
        public async Task<IActionResult> ClearAll([FromQuery] string serviceIdentifier)
        {
            var result = await _agentService.SendOperation(serviceIdentifier, async (baseUrl) => await _cacheService.ClearAllCacheAsync(baseUrl, default));
            return Ok(result);
        }

        // Admin endpoint to trigger cluster-wide cache clear
        //Agent endpoint
        [HttpGet]
        public async Task<IActionResult> GetByKey([FromQuery] string serviceIdentifier, [FromQuery] string key, [FromQuery] string id)
        {
            var result = await _agentService.SendOperation(serviceIdentifier, async (baseUrl) => await _cacheService.GetCacheByKeyAsync(baseUrl, key, default), id);
            return Ok(result);
        }

        // Admin endpoint to trigger cluster-wide cache clear
        //Agent endpoint
        [HttpGet("keys")]
        public async Task<IActionResult> GetAllKeys([FromQuery] string serviceIdentifier)
        {
            var result = await _agentService.SendOperation(serviceIdentifier, async (baseUrl) => await _cacheService.GetAllKeysAsync(baseUrl, default));

            return Ok(result);
        }
    }
}