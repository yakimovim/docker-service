using System.Collections.Generic;
using Service.Model;

namespace Service.Services
{
    public class ResponseDefinitions
    {
        private readonly object _aLock = new object();

        private IReadOnlyCollection<ResponseItem> _a = new[]
        {
            new Response()
        };

        public IReadOnlyCollection<ResponseItem> A
        {
            get 
            {
                lock(_aLock)
                {
                    return _a;
                }
            }
            set
            {
                lock (_aLock)
                {
                    _a = value;
                    if(_a == null || _a.Count == 0)
                    {
                        _a = new[]
                        {
                            new Response()
                        };
                    }
                }
            }
        }
    }
}
