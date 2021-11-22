using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Service.Components
{
    public partial class RequestSender
    {
        public bool Loading { get; set; }

        public string RequestUrl { get; set; }

        public string RequestMethod { get; set; } = "Get";

        public string ResponseBody { get; set; }

        public HttpStatusCode ResponseStatusCode { get; set; }

        [Inject]
        public Services.RequestSender Sender { get; set; }

        private async Task HandleSubmit()
        { 
            ResponseBody = null;

            Loading = true;

            var request = new Services.RequestModel
            {
                RequestUrl = RequestUrl,
                RequestMethod = RequestMethod,
            };

            var response = await Sender.SendRequest(request);

            Loading = false;

            ResponseBody = response.ResponseBody;

            ResponseStatusCode = response.ResponseStatusCode;
        }
    }
}
