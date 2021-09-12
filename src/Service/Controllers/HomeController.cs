using Microsoft.AspNetCore.Mvc;
using Service.Controllers.Utilities;
using System;
using System.Diagnostics;
using System.IO;

namespace Service.Controllers
{
    public class HomeController : Controller
    {
        private readonly HomePageModelFactory _modelFactory;

        public HomeController(HomePageModelFactory modelFactory)
        {
            _modelFactory = modelFactory ?? throw new ArgumentNullException(nameof(modelFactory));
        }

        [MainMenuElement("Home", Order = int.MinValue)]
        public IActionResult Index()
        {
            var model = _modelFactory.Create();

            return View(model);
        }
    }

    public sealed class HomePageModel
    {
        public string ComputerName { get; set; }

        public string OperatingSystem { get; set; }

        public string DomainName { get; set; }

        public string UserName { get; set; }

        public int ProcessorCount { get; set; }

        public long PrivateMemoryBytes { get; set; }

        public long WorkingSetBytes { get; set; }

        public DriveInfo[] Drives { get; set; }
    }

    public sealed class HomePageModelFactory
    {
        public HomePageModel Create()
        {
            var process = Process.GetCurrentProcess();

            return new HomePageModel
            {
                ComputerName = Environment.MachineName, 
                OperatingSystem = Environment.OSVersion.ToString(),
                DomainName = Environment.UserDomainName,
                UserName = Environment.UserName,
                ProcessorCount = Environment.ProcessorCount,
                PrivateMemoryBytes = process.PrivateMemorySize64,
                WorkingSetBytes = Environment.WorkingSet,
                Drives = DriveInfo.GetDrives()
            };
        }
    }
}
