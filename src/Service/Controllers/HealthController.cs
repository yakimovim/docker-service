using Microsoft.AspNetCore.Mvc;
using Service.Controllers.Utilities;
using Service.Model;
using System.Net;

namespace Service.Controllers
{
    public class HealthController : Controller
    {
        private readonly Health _health;

        public HealthController(Health health)
        {
            _health = health ?? throw new System.ArgumentNullException(nameof(health));
        }

        [MainMenuElement("Health")]
        [HttpGet]
        public IActionResult Index()
        {
            var model = new HealthModel
            {
                IsHealthy = _health.Healthy == HttpStatusCode.OK,
                IsReady = _health.Ready == HttpStatusCode.OK
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(HealthModel model)
        {
            _health.Healthy = model.IsHealthy ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
            _health.Ready = model.IsReady ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
            return RedirectToAction();
        }
    }

    /// <summary>
    /// Represents health status of the service.
    /// </summary>
    public sealed class HealthModel
    {
        /// <summary>
        /// Gets or sets if the service is healthy.
        /// </summary>
        public bool IsHealthy { get; set; }

        /// <summary>
        /// Gets or sets if the service is ready to process requests.
        /// </summary>
        public bool IsReady { get; set; }
    }
}
