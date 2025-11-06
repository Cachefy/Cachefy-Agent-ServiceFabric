using Microsoft.AspNetCore.Mvc;

namespace Agent.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController : ControllerBase
    {
        // Admin endpoint to trigger cluster-wide cache clear
        //Agent endpoint
        [HttpGet]
        public IActionResult Ping()
        {
            return Ok();
        }
    }
}