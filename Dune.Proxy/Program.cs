using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Dune.Packets;

namespace Dune.Proxy
{
    internal class Program
    {
        private static string _serverCertificateName = "dune.pfx";

        private static string _serverCertificatePassword = "";

        private static X509Certificate2 _serverCertificate;

        private static StreamWriter _logger;

        private static IPAddress _localAddress = IPAddress.Any;

        private static ushort _localPort = 12501;

        private static string _remoteAddress = "dune.dragonsdogma.com";

        private static ushort _remotePort = 12501;

        static void Log(string message)
        {
            Console.WriteLine(message);
            _logger.WriteLine(message);
        }

        static void Main()
        {
            HostProxy(_localAddress, _localPort, _remoteAddress, _remotePort);
        }

        private static void HostProxy(IPAddress localAddress, ushort localPort, string remoteAddress, ushort remotePort)
        {
            _serverCertificate = new X509Certificate2(_serverCertificateName, _serverCertificatePassword);
            _logger = new StreamWriter(new FileStream("log.txt", FileMode.Append)) { AutoFlush = true };
            var listener = new TcpListener(localAddress, localPort);
            Log("Listening: " + localPort);
            listener.Start();
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                ProcessClient(client, remoteAddress, remotePort);
            }
            Console.WriteLine("Done Listening");
        }

        private static void ProcessClient(TcpClient client, string remoteUrl, ushort remotePort)
        {
            var clientId = client.Client.RemoteEndPoint.ToString();
            Log("Connecting: " + clientId);

            SslStream clientSslStream = new SslStream(client.GetStream(), false);
            clientSslStream.AuthenticateAsServer(_serverCertificate, false, SslProtocols.Tls, false);
            var clientStream = new PacketStream(clientSslStream);
            var proxyClient = new TcpClient(remoteUrl, remotePort);
            proxyClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            proxyClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, 32768);
            proxyClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, 32768);
            SslStream proxySslStream = new SslStream(proxyClient.GetStream(), false);
            proxySslStream.AuthenticateAsClient(remoteUrl);
            var proxyStream = new PacketStream(proxySslStream);

            Log("Connected: " + clientId);

            while (true)
            {
                RawPacket serverPacket = proxyStream.ReadRawPacket();
                if (serverPacket == null) break;
                byte[] serverPacketData = serverPacket.Serialize();
                clientSslStream.Write(serverPacketData, 0, serverPacketData.Length);
                Log(string.Format("Server: {0:x} {1} {2}", serverPacket.Sequence, serverPacket.Type, BitConverter.ToString(serverPacket.Payload).Replace("-", " ")));

                if (serverPacket.Type == PacketType.DisconnectionResponse || serverPacket.Type == PacketType.DisconnectionNotification)
                {
                    break;
                }

                RawPacket clientPacket = clientStream.ReadRawPacket();
                if (clientPacket == null) break;
                byte[] clientPacketData = clientPacket.Serialize();
                proxySslStream.Write(clientPacketData, 0, clientPacketData.Length);
                Log(string.Format("Client: {0:x} {1} {2}", clientPacket.Sequence, clientPacket.Type, BitConverter.ToString(clientPacket.Payload).Replace("-", " ")));
            }

            Log("Disconnecting: " + clientId);
            proxySslStream.Close();
            clientSslStream.Close();
            Log("Disconnected: " + clientId);
        }
    }
}
