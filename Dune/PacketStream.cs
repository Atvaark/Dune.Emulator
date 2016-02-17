using System;
using System.IO;
using Dune.Packets;
using Dune.Packets.Impl;

namespace Dune
{
    public class PacketStream
    {
        private readonly Stream _stream;
        private ushort _sequence;

        public PacketStream(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            _stream = stream;
        }

        public void WritePacket(PacketBase packet, ushort? sequence = null)
        {
            sequence = sequence ?? _sequence++;
            packet.Sequence = sequence.Value;
            byte[] data = packet.Serialize();
            _stream.Write(data, 0, data.Length);
        }

        public T ReadPacket<T>() where T : PacketBase
        {
            T packet = ReadPacket() as T;
            if (packet == null) throw new ApplicationException();
            return packet;
        }

        public RawPacket ReadRawPacket()
        {
            return RawPacket.Read(_stream);
        }

        public PacketBase ReadPacket()
        {
            RawPacket rawPacket = ReadRawPacket();

            PacketBase packet;
            switch (rawPacket.Type)
            {
                case PacketType.OnlineCheckRequest:
                    packet = new OnlineCheckRequest();
                    break;
                case PacketType.OnlineCheckResponse:
                    packet = new OnlineCheckResponse();
                    break;
                case PacketType.DisconnectionRequest:
                    packet = new DisconnectionRequest();
                    break;
                case PacketType.DisconnectionResponse:
                    packet = new DisconnectionResponse();
                    break;
                case PacketType.DisconnectionNotification:
                    packet = new DisconnectionNotification();
                    break;
                case PacketType.ReconnectionRequest:
                    packet = new ReconnectionRequest();
                    break;
                case PacketType.FastDataRequest:
                    packet = new FastDataRequest();
                    break;
                case PacketType.FastDataResponse:
                    packet = new FastDataResponse();
                    break;
                case PacketType.ConnectionSummaryNotification:
                    packet = new ConnectionSummaryNotification();
                    break;
                case PacketType.AuthenticationInformationRequestHeader:
                    packet = new AuthenticationInformationRequestHeader();
                    break;
                case PacketType.AuthenticationInformationResponseHeader:
                    packet = new AuthenticationInformationResponseHeader();
                    break;
                case PacketType.AuthenticationInformationRequestData:
                    packet = new AuthenticationInformationRequestData();
                    break;
                case PacketType.AuthenticationInformationResponseData:
                    packet = new AuthenticationInformationResponseData();
                    break;
                case PacketType.AuthenticationInformationRequestFooter:
                    packet = new AuthenticationInformationRequestFooter();
                    break;
                case PacketType.AuthenticationInformationResponseFooter:
                    packet = new AuthenticationInformationResponseFooter();
                    break;
                case PacketType.TusCommonAreaAcquisitionRequest:
                    packet = new TusCommonAreaAcquisitionRequest();
                    break;
                case PacketType.TusCommonAreaAcquisitionResponse:
                    packet = new TusCommonAreaAcquisitionResponse();
                    break;
                case PacketType.TusCommonAreaSettingsRequest:
                    packet = new TusCommonAreaSettingsRequest();
                    break;
                case PacketType.TusCommonAreaSettingsResponse:
                    packet = new TusCommonAreaSettingsResponse();
                    break;
                case PacketType.TusCommonAreaAddRequest:
                    packet = new TusCommonAreaAddRequest();
                    break;
                case PacketType.TusCommonAreaAddResponse:
                    packet = new TusCommonAreaAddResponse();
                    break;
                case PacketType.TusUserAreaWriteRequestHeader:
                    packet = new TusUserAreaWriteRequestHeader();
                    break;
                case PacketType.TusUserAreaWriteResponseHeader:
                    packet = new TusUserAreaWriteResponseHeader();
                    break;
                case PacketType.TusUserAreaWriteRequestData:
                    packet = new TusUserAreaWriteRequestData();
                    break;
                case PacketType.TusUserAreaWriteResponseData:
                    packet = new TusUserAreaWriteResponseData();
                    break;
                case PacketType.TusUserAreaWriteRequestFooter:
                    packet = new TusUserAreaWriteRequestFooter();
                    break;
                case PacketType.TusUserAreaWriteResponseFooter:
                    packet = new TusUserAreaWriteResponseFooter();
                    break;
                case PacketType.TusUserAreaReadRequestHeader:
                    packet = new TusUserAreaReadRequestHeader();
                    break;
                case PacketType.TusUserAreaReadResponseHeader:
                    packet = new TusUserAreaReadResponseHeader();
                    break;
                case PacketType.TusUserAreaReadRequestData:
                    packet = new TusUserAreaReadRequestData();
                    break;
                case PacketType.TusUserAreaReadResponseData:
                    packet = new TusUserAreaReadResponseData();
                    break;
                case PacketType.TusUserAreaReadRequestFooter:
                    packet = new TusUserAreaReadRequestFooter();
                    break;
                case PacketType.TusUserAreaReadResponseFooter:
                    packet = new TusUserAreaReadResponseFooter();
                    break;
                default:
                    return rawPacket;
            }

            packet.Sequence = rawPacket.Sequence;
            packet.ParsePayload(rawPacket.Payload);
            return packet;
        }

        public void Close()
        {
            _stream.Close();
        }
    }
}