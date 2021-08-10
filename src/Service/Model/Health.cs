using Microsoft.Extensions.Options;
using System;
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
