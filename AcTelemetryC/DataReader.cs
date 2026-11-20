using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;

namespace AcTelemetryC
{
    //Reads real-time physics telemetry from Assetto Corsa via Windows Shared Memory.
    public sealed class DataReader : IDisposable
    {
        // Named shared memory exposed by Assetto Corsa
        private const string Acpmf_physic = "acpmf_physics";
        private MemoryMappedFile? _shMemory;
        private MemoryMappedViewAccessor? _accessor;


        // Gets a value indicating whether the reader is currently connected to the simulator's shared memory.
        public bool IsConnected => _accessor != null;

        // Attempts to open the Assetto Corsa physics shared memory and initialize a memory view accessor.
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

        // Reads from shared memory from PhysicData
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
