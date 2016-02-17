using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Dune.Server
{
    class Program
    {
        private static string _serverCertificateName = "dune.pfx";

        private static string _serverCertificatePassword = "";

        private static IPAddress _localServerAddress = IPAddress.Any;

        private static ushort _serverPort = 12501;

        static void Main()
        {
            HostServer().Wait();
        }

        private static async Task HostServer()
        {
            var serverCertificate = new X509Certificate2(_serverCertificateName, _serverCertificatePassword);
            var server = new DuneServer(_localServerAddress, _serverPort, serverCertificate);
            var cts = new CancellationTokenSource();

            var listenTask = server.ListenAsync(cts.Token);

            Console.WriteLine("Listening");
            while (!listenTask.IsCompleted)
            {
                await Task.Delay(10000);
                Console.WriteLine("Tick");
            }

            Console.WriteLine("Done Listening");
        }
    }
}
