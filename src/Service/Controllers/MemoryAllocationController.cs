using Microsoft.AspNetCore.Mvc;
using Service.Controllers.Utilities;
using Service.Model;

namespace Service.Controllers
{
    public class MemoryAllocationController : Controller
    {
        private readonly MemoryLoadProvider _memoryLoadProvider;

        public MemoryAllocationController(
            MemoryLoadProvider memoryLoadProvider
            )
        {
            _memoryLoadProvider = memoryLoadProvider ?? throw new System.ArgumentNullException(nameof(memoryLoadProvider));
        }

        [HttpGet]
        [MainMenuElement("Memory load")]
        public IActionResult Index()
        {
            return View(
                new MemoryLoad
                {
                    AllocatedBytes = _memoryLoadProvider.CurrentSizeOfAllocatedMemory,
                    PrivateMemoryBytes = _memoryLoadProvider.PrivateMemoryBytes,
                    WorkingSetBytes = _memoryLoadProvider.WorkingSetBytes
                }
            ); ;
        }

        public sealed class  MemoryAllocationDto
        {
            public long MemorySize {  get; set; }
        }

        [HttpPost]
        public IActionResult AllocateMemory(MemoryAllocationDto dto)
        {
            if(dto.MemorySize >= 0)
            {
                _memoryLoadProvider.SetMemoryConsumptionTo(dto.MemorySize);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ReleaseMemory()
        {
            _memoryLoadProvider.ReleaseMemory();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult CollectGarbage()
        {
            _memoryLoadProvider.CollectGarbage();

            return RedirectToAction(nameof(Index));
        }
    }

    public class MemoryLoad
    {
        public int AllocatedBytes { get; init; }
        public long PrivateMemoryBytes { get; init; }
        public long WorkingSetBytes { get; init; }
    }
}
