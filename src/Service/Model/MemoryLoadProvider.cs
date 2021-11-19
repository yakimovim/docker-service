using System;
using System.Diagnostics;

namespace Service.Model
{
    public class MemoryLoadProvider
    {
        private readonly object _lock = new object();

        private byte[] _memory = Array.Empty<byte>();

        public void IncreaseMemoryConsumptionBy(long bytes)
        {
            lock (_lock)
            {
                _memory = new byte[bytes + _memory.Length];
            }
        }

        public void SetMemoryConsumptionTo(long bytes)
        {
            lock (_lock)
            {
                _memory = new byte[bytes];
            }
        }

        public void ReleaseMemory()
        {
            lock (_lock)
            {
                _memory = Array.Empty<byte>();
            }
        }

        public void CollectGarbage()
        {
            lock (_lock)
            {
                GC.Collect();
            }
        }

        public int CurrentSizeOfAllocatedMemory
        {
            get
            {
                lock (_lock)
                {
                    return _memory.Length;
                }
            }
        }

        public long PrivateMemoryBytes
        {
            get
            {
                var process = Process.GetCurrentProcess();

                return process.PrivateMemorySize64;
            }
        }


        public long WorkingSetBytes { get => Environment.WorkingSet; }
    }
}
