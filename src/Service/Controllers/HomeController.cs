using Microsoft.AspNetCore.Mvc;
using Service.Controllers.Utilities;
using System;
using System.Diagnostics;

namespace Service.Controllers
{
    public class HomeController : Controller
    {
        private readonly HomePageModelFactory _modelFactory;

        public HomeController(HomePageModelFactory modelFactory)
        {
            _modelFactory = modelFactory ?? throw new ArgumentNullException(nameof(modelFactory));
        }

        [MainMenuElement("Home")]
        public IActionResult Index()
        {
            var model = _modelFactory.Create();

            return View(model);
        }
    }

    public sealed class HomePageModel
    {
        public string ComputerName { get; set; }

        public long PrivateMemoryBytes { get; set; }
    }

    public sealed class HomePageModelFactory
    {
        public HomePageModel Create()
        {
            var process = Process.GetCurrentProcess();

            return new HomePageModel
            {
                ComputerName = Environment.MachineName,
                PrivateMemoryBytes = process.PrivateMemorySize64
            };
        }
    }
}
