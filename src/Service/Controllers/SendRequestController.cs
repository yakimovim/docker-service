using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service.Controllers.Utilities;

namespace Service.Controllers
{
    public class SendRequestController : Controller
    {
        private readonly HttpClient _httpClient;

        public SendRequestController(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new System.ArgumentNullException(nameof(httpClient));
        }

        [HttpGet]
        [MainMenuElement("Send request")]
        public IActionResult Index()
        {
            return View(new SendRequestModel());
        }

        [HttpPost]
        public async Task<IActionResult> SendRequest(SendRequestDto dto)
        {
            var request = new HttpRequestMessage
            {
                Method = new HttpMethod(dto.RequestMethod),
                RequestUri = new Uri(dto.RequestUrl),
                Content = (dto.RequestMethod != "Get") ? new StringContent(dto.RequestBody) : null
            };

            var response = await _httpClient.SendAsync(request);

            var model = new SendRequestModel
            {
                RequestUrl = dto.RequestUrl,
                RequestMethod = dto.RequestMethod,
                RequestBody = dto.RequestBody,
                ResponseBody = await response.Content.ReadAsStringAsync()
            };

            return View("Index", model);
        }
    }

    public class SendRequestDto
    {
        public string RequestUrl { get; set; } = string.Empty;

        public string RequestMethod { get; set; } = "Get";

        public string RequestBody { get; set; } = string.Empty;
    }

    public class SendRequestModel : SendRequestDto
    {
        public string ResponseBody { get; set; } = string.Empty;
    }
}
