using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Service.Model
{
    public abstract class ResponseItem
    {
        /// <summary>
        /// Executes item logic.
        /// </summary>
        /// <param name="controller">Controller.</param>
        /// <returns>Execution result.</returns>
        public abstract Task<ResponseItemExecutionResult> ExecuteAsync(ControllerBase controller);

        /// <summary>
        /// Clones the response item.
        /// </summary>
        public abstract ResponseItem Clone();
    }

    public sealed class ResponseItemExecutionResult
    {
        private readonly IActionResult _result;

        public ResponseItemExecutionResult()
        {
            IsFinished = false;
        }

        public ResponseItemExecutionResult(IActionResult result)
        {
            _result = result ?? throw new ArgumentNullException(nameof(result));
            IsFinished = true;
        }

        /// <summary>
        /// Checks if response is ready and further response items should not be processed.
        /// </summary>
        public bool IsFinished { get; }

        /// <summary>
        /// Action result if response is ready.
        /// </summary>
        /// <remarks>
        /// Throws exception if response is not ready.
        /// </remarks>
        public IActionResult Result {
            get
            {
                if (!IsFinished) throw new InvalidOperationException();

                return _result;
            }
        }
    }

    public abstract class ProbableResponseItem : ResponseItem
    {
        private readonly Random _rnd = new Random((int)DateTime.Now.Ticks);
        private double _probability = 0.0;

        public double Probability 
        {
            get => _probability;
            set
            {
                if (value < 0 || value >= 1)
                    throw new ArgumentOutOfRangeException(nameof(value));

                _probability = value;
            }
        }

        protected bool IsHappened()
        {
            return _rnd.NextDouble() < Probability;
        }
    }

    public sealed class Delay : ResponseItem
    {
        private TimeSpan _delayDuration;

        public int DelayDurationInSeconds 
        {
            get => (int) _delayDuration.TotalSeconds;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                _delayDuration = TimeSpan.FromSeconds(value);
            }
        }

        public override async Task<ResponseItemExecutionResult> ExecuteAsync(ControllerBase controller)
        {
            await Task.Delay(_delayDuration);

            return new ResponseItemExecutionResult();
        }

        public override ResponseItem Clone()
        {
            return new Delay { DelayDurationInSeconds = DelayDurationInSeconds };
        }
    }

    public sealed class ProbableDelay : ProbableResponseItem
    {
        private TimeSpan _delayDuration;

        public int DelayDurationInSeconds
        {
            get => (int)_delayDuration.TotalSeconds;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                _delayDuration = TimeSpan.FromSeconds(value);
            }
        }

        public override async Task<ResponseItemExecutionResult> ExecuteAsync(ControllerBase controller)
        {
            if(IsHappened())
            {
                await Task.Delay(_delayDuration);
            }

            return new ResponseItemExecutionResult();
        }

        public override ResponseItem Clone()
        {
            return new ProbableDelay 
            { 
                Probability = Probability,
                DelayDurationInSeconds = DelayDurationInSeconds
            };
        }
    }

    public sealed class Response : ResponseItem
    {
        public int Status { get; set; } = 200;
        public string Body { get; set; }
        public string ContentType { get; set; }

        public override async Task<ResponseItemExecutionResult> ExecuteAsync(ControllerBase controller)
        {
            if(!string.IsNullOrWhiteSpace(ContentType))
            {
                controller.Response.ContentType = ContentType;
            }

            var result = controller.StatusCode(Status, Body ?? string.Empty);

            return new ResponseItemExecutionResult(result);
        }

        public override ResponseItem Clone()
        {
            return new Response
            {
                Status = Status,
                Body = Body,
                ContentType = ContentType
            };
        }
    }

    public sealed class ProbableResponse : ProbableResponseItem
    {
        public int Status { get; set; } = 200;
        public string Body { get; set; }
        public string ContentType { get; set; }

        public override async Task<ResponseItemExecutionResult> ExecuteAsync(ControllerBase controller)
        {
            if(IsHappened())
            {
                if (!string.IsNullOrWhiteSpace(ContentType))
                {
                    controller.Response.ContentType = ContentType;
                }

                var result = controller.StatusCode(Status, Body ?? string.Empty);

                return new ResponseItemExecutionResult(result);
            }

            return new ResponseItemExecutionResult();
        }

        public override ResponseItem Clone()
        {
            return new ProbableResponse
            {
                Probability = Probability,
                Status = Status,
                Body = Body,
                ContentType = ContentType
            };
        }
    }

    public sealed class Call : ResponseItem
    {
        private Uri _uri = new Uri("http://localhost");

        public string Url
        {
            get => _uri.ToString();
            set
            {
                _uri = new Uri(value);
            }
        }

        public string Method { get; set; } = "Get";

        public string Body { get; set; }

        public bool ReturnResponse { get; set; }

        public override async Task<ResponseItemExecutionResult> ExecuteAsync(ControllerBase controller)
        {
            using var client = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = new HttpMethod(Method),
                RequestUri = _uri,
            };

            if (request.Method != HttpMethod.Get)
            {
                request.Content = new StringContent(Body);
            }

            var response = await client.SendAsync(request);

            if (ReturnResponse)
            {
                controller.Response.ContentType = response.Content?.Headers?.ContentType?.MediaType;

                var body = await response.Content?.ReadAsStringAsync();

                var result = controller.StatusCode((int)response.StatusCode, body);
    
                return new ResponseItemExecutionResult(result);
            }

            return new ResponseItemExecutionResult();
        }

        public override ResponseItem Clone()
        {
            return new Call
            {
                Url = Url,
                Method = Method,
                Body = Body,
                ReturnResponse = ReturnResponse,
            };
        }
    }

    public sealed class ProbableCall : ProbableResponseItem
    {
        private Uri _uri = new Uri("http://localhost");

        public string Url
        {
            get => _uri.ToString();
            set
            {
                _uri = new Uri(value);
            }
        }

        public string Method { get; set; } = "Get";

        public string Body { get; set; }

        public bool ReturnResponse { get; set; }

        public override async Task<ResponseItemExecutionResult> ExecuteAsync(ControllerBase controller)
        {
            if(IsHappened())
            {
                using var client = new HttpClient();

                var request = new HttpRequestMessage
                {
                    Method = new HttpMethod(Method),
                    RequestUri = _uri,
                };

                if(request.Method != HttpMethod.Get)
                {
                    request.Content = new StringContent(Body);
                }

                var response = await client.SendAsync(request);

                if (ReturnResponse)
                {
                    controller.Response.ContentType = response.Content?.Headers?.ContentType?.MediaType;

                    var body = await response.Content?.ReadAsStringAsync();

                    var result = controller.StatusCode((int)response.StatusCode, body);

                    return new ResponseItemExecutionResult(result);
                }
            }

            return new ResponseItemExecutionResult();
        }

        public override ResponseItem Clone()
        {
            return new ProbableCall
            {
                Probability = Probability,
                Url = Url,
                Method = Method,
                Body = Body,
                ReturnResponse = ReturnResponse,
            };
        }
    }
}
