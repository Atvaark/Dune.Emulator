using System;
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

        static DuneServer()
        {
            UrDragon = new OnlineUrDragon
            {
                Generation = 1,
                FightCount = 0,
                KillCount = 0,
                SpawnTime = DateTime.Now.AddDays(-1),
                GraceTime = DateTime.Now
            };

            const int maxHealth = 10000000;
            for (int i = 0; i < UrDragon.Hearts.Length; i++)
            {
                UrDragon.Hearts[i].MaxHealth = maxHealth;
                UrDragon.Hearts[i].Health = maxHealth;
            }
        }

        private static readonly OnlineUrDragon UrDragon;

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
            stream.WritePacket(new AuthenticationInformationResponseFooter { Success = true });

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
                        Properties = UrDragon.ToProperties(getCommonAreaRequest.PropertyIndices)
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