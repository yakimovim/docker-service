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
        private readonly double _probability;
        private Random _rnd = new Random((int)DateTime.Now.Ticks);

        protected ProbableResponseItem(double probability)
        {
            if (probability < 0 || probability >= 1)
                throw new ArgumentOutOfRangeException(nameof(probability));

            _probability = probability;
        }

        protected bool IsHappened()
        {
            return _rnd.NextDouble() < _probability;
        }
    }

    public sealed class Delay : ResponseItem
    {
        private readonly TimeSpan _delay;

        public Delay(TimeSpan delay)
        {
            _delay = delay;
        }

        public override async Task<ResponseItemExecutionResult> ExecuteAsync(ControllerBase controller)
        {
            await Task.Delay(_delay);

            return new ResponseItemExecutionResult();
        }
    }

    public sealed class ProbableDelay : ProbableResponseItem
    {
        private readonly TimeSpan _delay;

        public ProbableDelay(double probability, TimeSpan delay)
            : base(probability)
        {
            _delay = delay;
        }

        public override async Task<ResponseItemExecutionResult> ExecuteAsync(ControllerBase controller)
        {
            if(IsHappened())
            {
                await Task.Delay(_delay);
            }

            return new ResponseItemExecutionResult();
        }
    }

    public sealed class Response : ResponseItem
    {
        private readonly int _status;
        private readonly object _body;
        private readonly string _contentType;

        public Response(int status = 200,
            object body = null,
            string contentType = null)
        {
            _status = status;
            _body = body;
            _contentType = contentType;
        }

        public override async Task<ResponseItemExecutionResult> ExecuteAsync(ControllerBase controller)
        {
            if(!string.IsNullOrWhiteSpace(_contentType))
            {
                controller.Response.ContentType = _contentType;
            }

            var result = controller.StatusCode(_status, _body ?? string.Empty);

            return new ResponseItemExecutionResult(result);
        }
    }

    public sealed class ProbableResponse : ProbableResponseItem
    {
        private readonly int _status;
        private readonly object _body;
        private readonly string _contentType;

        public ProbableResponse(
            double probability,
            int status = 200,
            object body = null,
            string contentType = null)
            : base(probability)
        {
            _status = status;
            _body = body;
            _contentType = contentType;
        }

        public override async Task<ResponseItemExecutionResult> ExecuteAsync(ControllerBase controller)
        {
            if(IsHappened())
            {
                if (!string.IsNullOrWhiteSpace(_contentType))
                {
                    controller.Response.ContentType = _contentType;
                }

                var result = controller.StatusCode(_status, _body);

                return new ResponseItemExecutionResult(result);
            }

            return new ResponseItemExecutionResult();
        }
    }

    public sealed class Call : ResponseItem
    {
        private readonly Uri _url;
        private readonly bool _returnResponse;

        public Call(Uri url,
            bool returnResponse = false)
        {
            _url = url ?? throw new ArgumentNullException(nameof(url));
            _returnResponse = returnResponse;
        }

        public override async Task<ResponseItemExecutionResult> ExecuteAsync(ControllerBase controller)
        {
            using var client = new HttpClient();

            var response = await client.GetAsync(_url);

            if(_returnResponse)
            {
                controller.Response.ContentType = response.Content?.Headers?.ContentType?.MediaType;

                var body = await response.Content?.ReadAsStringAsync();

                var result = controller.StatusCode((int)response.StatusCode, body);
    
                return new ResponseItemExecutionResult(result);
            }

            return new ResponseItemExecutionResult();
        }
    }

    public sealed class ProbableCall : ProbableResponseItem
    {
        private readonly Uri _url;
        private readonly bool _returnResponse;

        public ProbableCall(
            double probability,
            Uri url,
            bool returnResponse = false)
            : base(probability)
        {
            _url = url ?? throw new ArgumentNullException(nameof(url));
            _returnResponse = returnResponse;
        }

        public override async Task<ResponseItemExecutionResult> ExecuteAsync(ControllerBase controller)
        {
            if(IsHappened())
            {
                using var client = new HttpClient();

                var response = await client.GetAsync(_url);

                if (_returnResponse)
                {
                    controller.Response.ContentType = response.Content?.Headers?.ContentType?.MediaType;

                    var body = await response.Content?.ReadAsStringAsync();

                    var result = controller.StatusCode((int)response.StatusCode, body);

                    return new ResponseItemExecutionResult(result);
                }
            }

            return new ResponseItemExecutionResult();
        }
    }
}
