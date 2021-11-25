using Microsoft.AspNetCore.Mvc;
using Service.Model;
using System.IO;
using System.Threading.Tasks;

namespace Service.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkController : ControllerBase
    {
        private readonly ResponseDefinitions _responseDefinitions;

        public WorkController(ResponseDefinitions responseDefinitions)
        {
            _responseDefinitions = responseDefinitions ?? throw new System.ArgumentNullException(nameof(responseDefinitions));
        }

        [HttpGet, HttpPut, HttpPost, HttpDelete]
        [Route("echo")]
        public async Task<IActionResult> Echo()
        {
            if (Request.Method == "GET")
                return Ok();

            using var reader = new StreamReader(Request.Body);
            var content = await reader.ReadToEndAsync();

            return Ok(content);
        }

        [HttpGet, HttpPut, HttpPost, HttpDelete]
        [Route("a")]
        public async Task<IActionResult> DoWorkA()
        {
            foreach (var item in _responseDefinitions.A)
            {
                var result = await item.ExecuteAsync(this);
                if (result.IsFinished)
                    return result.Result;
            }

            return NotFound();
        }
    }
}
