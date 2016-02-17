using System;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using Dune.Packets;
using Dune.Packets.Impl;

namespace Dune
{
    public class DuneClient
    {
        private readonly string _host;
        private readonly int _port;
        private readonly ulong _steamId;
        private readonly byte[] _ticket;

        private bool _connected;
        private TcpClient _client;
        private PacketStream _stream;

        public DuneClient(string host, int port, ulong steamId, byte[] ticket)
        {
            _host = host;
            _port = port;

            _steamId = steamId;
            _ticket = ticket;
        }

        public void Connect()
        {
            if (_connected) return;

            _client = new TcpClient(_host, _port);
            _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, 32768);
            _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, 32768);
            _client.ReceiveTimeout = 30000;
            _client.SendTimeout = 30000;
            var stream = new SslStream(_client.GetStream());
            stream.AuthenticateAsClient(_host);
            _stream = new PacketStream(stream);

            Authenticate();

            _connected = true;
        }

        public void Disconnect()
        {
            if (!_connected) return;

            _stream.WritePacket(new DisconnectionRequest { Success = true });
            _stream.ReadPacket<DisconnectionResponse>();

            _stream.Close();

            _connected = false;
        }

        public TusProperty[] GetCommonArea(byte[] requestedPropertyIndices)
        {
            if (!_connected) throw new ApplicationException();

            _stream.WritePacket(new TusCommonAreaAcquisitionRequest { PropertyIndices = requestedPropertyIndices });
            var response = _stream.ReadPacket<TusCommonAreaAcquisitionResponse>();
            return response.Properties.ToArray();
        }

        public TusProperty[] GetCommonAreaSettings(TusProperty[] properties)
        {
            if (!_connected) throw new ApplicationException();

            _stream.WritePacket(new TusCommonAreaSettingsRequest() { Properties = properties });
            var response = _stream.ReadPacket<TusCommonAreaSettingsResponse>();
            return response.Properties.ToArray();
        }

        public TusProperty[] AddCommonArea(TusProperty[] properties)
        {
            if (!_connected) throw new ApplicationException();

            _stream.WritePacket(new TusCommonAreaAddRequest { Properties = properties });
            var response = _stream.ReadPacket<TusCommonAreaAddResponse>();
            return response.Properties.ToArray();
        }

        public byte[] GetUserArea()
        {
            if (!_connected) throw new ApplicationException();

            _stream.WritePacket(new TusUserAreaReadRequestHeader { User = _steamId.ToString("X16") });
            var headerResponse = _stream.ReadPacket<TusUserAreaReadResponseHeader>();

            // TODO: Fix reading multiple chunks or chunk > int16.Max
            _stream.WritePacket(new TusUserAreaReadRequestData { ChunkLength = (ushort)headerResponse.ServerReadBufferLength });
            var dataResponse = _stream.ReadPacket<TusUserAreaReadResponseData>();

            _stream.WritePacket(new TusUserAreaReadRequestFooter());
            _stream.ReadPacket<TusUserAreaReadResponseFooter>();

            return dataResponse.Chunk;
        }

        private void Authenticate()
        {
            // FastData
            var fastDataRequest = _stream.ReadPacket<FastDataRequest>();
            _stream.WritePacket(new FastDataResponse { User = _steamId.ToString("X16") }, fastDataRequest.Sequence);

            // ConnectionSummary
            _stream.ReadPacket<ConnectionSummaryNotification>();

            // AuthenticationInformation Header
            _stream.WritePacket(new AuthenticationInformationRequestHeader { DataLength = _ticket.Length });
            _stream.ReadPacket<AuthenticationInformationResponseHeader>();

            // TODO: Fix reading multiple chunks or chunk > int16.Max
            // AuthenticationInformation Data
            _stream.WritePacket(new AuthenticationInformationRequestData { Chunk = _ticket, ChunkLength = (ushort)_ticket.Length });
            _stream.ReadPacket<AuthenticationInformationResponseData>();

            // AuthenticationInformation Footer
            _stream.WritePacket(new AuthenticationInformationRequestFooter());
            _stream.ReadPacket<AuthenticationInformationResponseFooter>();

            _connected = true;
        }
    }
}