using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Service.Hubs;
using Service.Model;
using System;
using System.Threading.Tasks;

namespace Service.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly Health _health;
        private readonly IHubContext<HealthHub> _healthHubContext;

        public HealthController(
            Health health,
            IHubContext<HealthHub> healthHubContext)
        {
            _health = health ?? throw new System.ArgumentNullException(nameof(health));
            _healthHubContext = healthHubContext ?? throw new System.ArgumentNullException(nameof(healthHubContext));
        }

        [HttpGet]
        [Route("healthy")]
        public async Task<IActionResult> Healthy()
        {
            var status = (int)_health.Healthy;
            var timeStamp = DateTimeOffset.UtcNow;
            _health.AddHealthyStatus(status, timeStamp);
            await _healthHubContext.Clients.All.SendAsync("HealthStatus", status, timeStamp);
            return StatusCode(status);
        }

        [HttpGet]
        [Route("ready")]
        public async Task<IActionResult> Ready()
        {
            var status = (int)_health.Ready;
            var timeStamp = DateTimeOffset.UtcNow;
            _health.AddReadyStatus(status, timeStamp);
            await _healthHubContext.Clients.All.SendAsync("ReadyStatus", status, timeStamp);
            return StatusCode(status);
        }
    }
}
