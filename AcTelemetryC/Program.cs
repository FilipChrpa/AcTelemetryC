using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace AcTelemetryC
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Physics
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
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Loading Assetto Corsa");

            MemoryMappedFile? ShMemory = null;
            MemoryMappedViewAccessor? Accessor = null;

            while (ShMemory == null)
            {
                try
                {
                    ShMemory = MemoryMappedFile.OpenExisting("acpmf_physics");
                    Accessor = ShMemory.CreateViewAccessor(0, Marshal.SizeOf<Physics>());
                    Console.WriteLine("Reading Telemetry.. \n");
                }
                catch (FileNotFoundException)
                {
                    Thread.Sleep(1000);
                    Console.Write(".");
                }
            }
            while (true)
            {
                Accessor.Read(0, out Physics data);

                Console.Write($"\r Speed: {data.SpeedKmh,5:F1} km/h | RMP: {data.Rpms,5} RPM | Throttle: {data.Gas * 100,3:F0} % ");
                Thread.Sleep(50);
            }

            //var ShMemory = MemoryMappedFile.OpenExisting("acpmf_physics");
            //var Accessor = ShMemory.CreateViewAccessor(0, Marshal.SizeOf < Physics >());

            //Accessor.Read(0, out Physics data);

            //Console.WriteLine($"Speed: {data.SpeedKmh} km/h");
        }
    }
}
