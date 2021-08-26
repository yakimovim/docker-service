using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace Service.Model
{
    public class Health
    {
        private readonly ReaderWriterLockSlim _healthLock = new ReaderWriterLockSlim();
        private readonly ReaderWriterLockSlim _readyLock = new ReaderWriterLockSlim();

        private HttpStatusCode _healthy = HttpStatusCode.OK;
        private HttpStatusCode _ready = HttpStatusCode.OK;
        private readonly ConcurrentQueue<(int status, DateTimeOffset timeStamp)> _healthyStatuses = new ConcurrentQueue<(int status, DateTimeOffset timeStamp)>();
        private readonly ConcurrentQueue<(int status, DateTimeOffset timeStamp)> _readyStatuses = new ConcurrentQueue<(int status, DateTimeOffset timeStamp)>();

        private const int StatusesLength = 10000;

        public Health(IOptions<HealthConfig> config)
        {
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            Ready = config.Value.IsReady
                ? HttpStatusCode.OK
                : HttpStatusCode.InternalServerError;
        }

        public HttpStatusCode Healthy
        {
            get
            {
                try
                {
                    _healthLock.EnterReadLock();
                    return _healthy;
                }
                finally
                {
                    _healthLock.ExitReadLock();
                }
            }
            set
            {
                try
                {
                    _healthLock.EnterWriteLock();
                    _healthy = value;
                }
                finally
                {
                    _healthLock.ExitWriteLock();
                }
            }
        }

        public HttpStatusCode Ready
        {
            get
            {
                try
                {
                    _readyLock.EnterReadLock();
                    return _ready;
                }
                finally
                {
                    _readyLock.ExitReadLock();
                }
            }
            set
            {
                try
                {
                    _readyLock.EnterWriteLock();
                    _ready = value;
                }
                finally
                {
                    _readyLock.ExitWriteLock();
                }
            }
        }

        public void AddHealthyStatus(int status, DateTimeOffset timeStamp)
        {
            _healthyStatuses.Enqueue((status, timeStamp));
            if(_healthyStatuses.Count > StatusesLength)
            {
                _healthyStatuses.TryDequeue(out _);
            }
        }

        public void AddReadyStatus(int status, DateTimeOffset timeStamp)
        {
            _readyStatuses.Enqueue((status, timeStamp));
            if (_readyStatuses.Count > StatusesLength)
            {
                _readyStatuses.TryDequeue(out _);
            }
        }

        public (int status, DateTimeOffset timeStamp)[] HealthyStatuses
            => _healthyStatuses.Reverse().ToArray();

        public (int status, DateTimeOffset timeStamp)[] ReadyStatuses
            => _readyStatuses.Reverse().ToArray();
    }

    /// <summary>
    /// Configuration of health settings.
    /// </summary>
    public class HealthConfig
    {
        /// <summary>
        /// Initial state of service readiness.
        /// </summary>
        public bool IsReady { get; set; } = true;
    }
}
