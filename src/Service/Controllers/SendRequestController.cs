using Microsoft.AspNetCore.Mvc;
using Service.Controllers.Utilities;

namespace Service.Controllers
{
	public class SendRequestController : Controller
    {
        [HttpGet]
        [MainMenuElement("Send request")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
