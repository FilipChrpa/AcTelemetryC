using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace AcTelemetryC
{ 
    internal class Program
    {
        static void Main(string[] args)
        {
            using var reader = new DataReader();

            Console.WriteLine("Waiting");

            while (!reader.TryConnect())
            {
                Console.Write(".");
                Thread.Sleep(1000);
            }
            Console.WriteLine("\n Pripojeno k Assetto Corsa");

            while (true)
            {
                if (reader.TryRead(out var data))
                {
                    Console.Write($"\r Speed: {data.SpeedKmh,5:F1} km/h | " + $"RPM: {data.Rpms,5} | " + $"Throttle: {data.Gas * 100,3:F0} % | " + $"Brake: {data.Brake * 100,3:F0} % | " + $"Gear: {data.Gear}   ");
                }
                else
                {
                    Console.WriteLine("Spojeni ztraceno");
                    break;
                }
                Thread.Sleep(50);
            }
        }
    }
}
