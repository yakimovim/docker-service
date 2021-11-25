using Microsoft.AspNetCore.Mvc;
using Service.Controllers.Utilities;

namespace Service.Controllers
{
    public class ResponsesController : Controller
    {
        [MainMenuElement("Responses")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
