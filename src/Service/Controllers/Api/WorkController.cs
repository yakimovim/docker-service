using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Service.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkController : ControllerBase
    {
        [HttpGet, HttpPut, HttpPost, HttpDelete]
        [Route("a")]
        public async Task<IActionResult> DoWorkA()
        {
            return Ok();
        }
    }
}
