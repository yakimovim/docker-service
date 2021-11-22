using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Service.Components
{
    public partial class RequestSender
    {
        public bool HasResponse { get; set; }

        public bool Loading { get; set; }

        public string Error { get; set; }

        public string RequestUrl { get; set; }

        public string RequestMethod { get; set; } = "Get";

        public string RequestBody { get; set; }

        public string ResponseBody { get; set; }

        public HttpStatusCode ResponseStatusCode { get; set; }

        [Inject]
        public Services.RequestSender Sender { get; set; }

        private async Task HandleSubmit()
        { 
            ResponseBody = null;

            Error = null;

            Loading = true;

            var request = new Services.RequestModel
            {
                RequestUrl = RequestUrl,
                RequestMethod = RequestMethod,
                RequestBody = RequestBody
            };

            try
            {
                var response = await Sender.SendRequest(request);

                ResponseBody = response.ResponseBody;

                ResponseStatusCode = response.ResponseStatusCode;
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }

            Loading = false;

            HasResponse = true;
        }
    }
}
