using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;

namespace AcTelemetryC
{ 
    public sealed class DataReader : IDisposable
    {
        private const string Acpmf_physic = "acpmf_physics";
        private MemoryMappedFile? _shMemory;
        private MemoryMappedViewAccessor? _accessor;

        public bool IsConnected => _accessor != null;

        public bool TryConnect()
        {
            if (IsConnected)
            {
                return true;
            }
            try
            {
                _shMemory = MemoryMappedFile.OpenExisting(Acpmf_physic);
                _accessor = _shMemory.CreateViewAccessor(0, Marshal.SizeOf<PhysicsData>());
                return true;
            }
            catch (FileNotFoundException)
            {
                Cleanup();
                return false;
            }
        }

        public bool TryRead(out PhysicsData data)
        {
            if (_accessor != null)
            {
                try
                {
                    _accessor.Read(0, out data);
                    return true;
                }
                catch
                {
                    Cleanup();
                }
            }
            data = default;
            return false;
        }

        public void Dispose()
        {
            Cleanup();
            GC.SuppressFinalize(this);
        }

        private void Cleanup()
        {
            _accessor?.Dispose();
            _accessor = null;

            _shMemory?.Dispose();
            _shMemory = null;
        }

    }
}
