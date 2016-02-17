using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Dune.Packets;
using Dune.Packets.Impl;

namespace Dune
{
    public class DuneServer
    {
        private readonly IPAddress _address;
        private readonly int _port;
        private readonly X509Certificate2 _certificate;
        private TcpListener _listener;
        private bool _listening;

        private static readonly TusProperty[] UrDragonProperties = 
        {
            // Generation
            new TusProperty(0, 0, 1), 

            // hearts
            new TusProperty(1, 10000000, 10000000), 
            new TusProperty(2, 10000000, 10000000),
            new TusProperty(3, 10000000, 10000000),
            new TusProperty(4, 10000000, 10000000),
            new TusProperty(5, 10000000, 10000000),
            new TusProperty(6, 10000000, 10000000),
            new TusProperty(7, 10000000, 10000000),
            new TusProperty(8, 10000000, 10000000),
            new TusProperty(9, 10000000, 10000000),
            new TusProperty(10, 10000000, 10000000),
            new TusProperty(11, 10000000, 10000000),
            new TusProperty(12, 10000000, 10000000),
            new TusProperty(13, 10000000, 10000000),
            new TusProperty(14, 10000000, 10000000),
            new TusProperty(15, 1000000, 1000000),

            // hearts max
            new TusProperty(16, 1000000, 1000000),
            new TusProperty(17, 1000000, 1000000),
            new TusProperty(18, 1000000, 1000000),
            new TusProperty(19, 1000000, 1000000),
            new TusProperty(20, 1000000, 1000000),
            new TusProperty(21, 1000000, 1000000),
            new TusProperty(22, 1000000, 1000000),
            new TusProperty(23, 1000000, 1000000),
            new TusProperty(24, 1000000, 1000000),
            new TusProperty(25, 1000000, 1000000),
            new TusProperty(26, 1000000, 1000000),
            new TusProperty(27, 1000000, 1000000),
            new TusProperty(28, 1000000, 1000000),
            new TusProperty(29, 1000000, 1000000),
            new TusProperty(30, 1000000, 1000000),

            // fight counter
            new TusProperty(31, 0, 0), // 0, fights

            // grace time unix timestamp
            new TusProperty(32, 0, 1455746607),

            // kill counter
            new TusProperty(33, 0, 0),

            // unknown values
            new TusProperty(34, 0, 0),
            new TusProperty(35, 17825793, 90229810),
            new TusProperty(36, 0, 0),
            new TusProperty(37, 17825793, 11772922),
            new TusProperty(38, 0, 0),
            new TusProperty(39, 17825793, 47409791),
            new TusProperty(40, 0, 0),

            // armor?
            new TusProperty(41, 0, 10800),

            // spawn time unix timestamp
            new TusProperty(42, 0, 1455746607),
        };

        public DuneServer(IPAddress address, int port, X509Certificate2 certificate)
        {
            _address = address;
            _port = port;
            _certificate = certificate;
        }

        public async Task ListenAsync(CancellationToken ct)
        {
            if (_listening) return;

            _listener = new TcpListener(_address, _port);
            _listener.Start();
            _listening = true;
            while (!ct.IsCancellationRequested)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();
                await Task.Run(() => HandleClient(client, ct), ct);
            }

            _listener.Stop();
            _listening = false;
        }

        private void HandleClient(TcpClient client, CancellationToken ct)
        {
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, 32768);
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, 32768);
            using (var sslStream = new SslStream(client.GetStream(), false))
            {
                sslStream.AuthenticateAsServer(_certificate, false, SslProtocols.Tls, false);
                var stream = new PacketStream(sslStream);
                string user = AuthenticateClient(stream, ct);

                Debug.WriteLine("Connect: " + user);
                bool run = true;
                while (run && !ct.IsCancellationRequested)
                {
                    Debug.WriteLine("Handle: " + user);
                    run = HandleRequests(stream, ct);
                }

                Debug.WriteLine("Disconnect: " + user);
            }
        }

        private string AuthenticateClient(PacketStream stream, CancellationToken ct)
        {
            // FastData
            stream.WritePacket(new FastDataRequest());
            var fastDataResponse = stream.ReadPacket<FastDataResponse>();

            // ConnectionSummary
            stream.WritePacket(new ConnectionSummaryNotification { Success = true, Unknown = 10 });

            // AuthenticationInformation Header
            var header = stream.ReadPacket<AuthenticationInformationRequestHeader>();
            stream.WritePacket(new AuthenticationInformationResponseHeader { ServerReadBufferLength = 256 });

            // TODO: Fix reading multiple chunks or chunk > int16.Max
            // AuthenticationInformation Data
            var data = stream.ReadPacket<AuthenticationInformationRequestData>();
            stream.WritePacket(new AuthenticationInformationResponseData { ChunkLength = data.ChunkLength, });

            // AuthenticationInformation Footer
            var footer = stream.ReadPacket<AuthenticationInformationRequestFooter>();
            stream.WritePacket(new AuthenticationInformationResponseFooter() { Success = true });

            return fastDataResponse.User;
        }

        private bool HandleRequests(PacketStream stream, CancellationToken ct)
        {
            bool run = true;
            PacketBase request = stream.ReadPacket();
            switch (request.Type)
            {
                case PacketType.TusCommonAreaAcquisitionRequest:
                    var getCommonAreaRequest = (TusCommonAreaAcquisitionRequest)request;
                    var getCommonAreaResponse = new TusCommonAreaAcquisitionResponse
                    {
                        Properties = UrDragonProperties.Where(t => getCommonAreaRequest.PropertyIndices.Contains(t.Index)).ToArray()
                    };
                    stream.WritePacket(getCommonAreaResponse);
                    break;
                case PacketType.DisconnectionRequest:
                    stream.WritePacket(new DisconnectionResponse { Success = true });
                    run = false;
                    break;
                default:
                    stream.WritePacket(new DisconnectionNotification { Notification = "" });
                    run = false;
                    break;
            }

            return run;
        }
    }
}