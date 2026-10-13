using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace AcTelemetryC
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PhysicsData
    {
        public int PacketID;
        public float Gas;
        public float Brake;
        public float Fuel;
        public int Gear;
        public int Rpms;
        public float SteerAngle;
        public float SpeedKmh;
    }
}