using Microsoft.AspNetCore.Mvc;
using Service.Model;

namespace Service.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly Health _health;

        public HealthController(Health health)
        {
            _health = health ?? throw new System.ArgumentNullException(nameof(health));
        }

        [HttpGet]
        [Route("healthy")]
        public IActionResult Healthy()
        {
            return StatusCode((int)_health.Healthy);
        }

        [HttpGet]
        [Route("ready")]
        public IActionResult Ready()
        {
            return StatusCode((int)_health.Ready);
        }
    }
}
