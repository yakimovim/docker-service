using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Service.Services
{
	public class RequestSender
	{
        private readonly HttpClient _httpClient;

        public RequestSender(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<ResponseModel> SendRequest(RequestModel model)
		{
            var request = new HttpRequestMessage
            {
                Method = new HttpMethod(model.RequestMethod),
                RequestUri = new Uri(model.RequestUrl),
                Content = (model.RequestMethod != "Get") ? new StringContent(model.RequestBody) : null
            };

            var response = await _httpClient.SendAsync(request);

            var result = new ResponseModel
            {
                ResponseStatusCode = response.StatusCode,
                ResponseBody = await response.Content.ReadAsStringAsync()
            };

            return result;
        }
	}

    public class RequestModel
    {
        public string RequestUrl { get; set; } = string.Empty;

        public string RequestMethod { get; set; } = "Get";

        public string RequestBody { get; set; } = string.Empty;
    }

    public class ResponseModel
    {
        public string ResponseBody { get; init; }

		public HttpStatusCode ResponseStatusCode { get; init; }
	}
}
