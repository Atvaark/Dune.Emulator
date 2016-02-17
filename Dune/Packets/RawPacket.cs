using System;
using System.IO;

namespace Dune.Packets
{
    public class RawPacket : PacketBase
    {
        private readonly PacketType _type;
        private byte[] _payload;

        public RawPacket(ushort sequence, PacketType type)
        {
            Sequence = sequence;
            _type = type;
        }

        public override PacketType Type
        {
            get { return _type; }
        }

        public override void ParsePayload(byte[] payload)
        {
            _payload = payload;
        }

        public byte[] Payload
        {
            get { return _payload; }
        }

        protected override byte[] SerializePayload()
        {
            return _payload;
        }

        public static RawPacket Read(Stream stream)
        {
            const int headerSize = 2 * sizeof(ushort) + sizeof(uint);
            byte[] header = new byte[headerSize];
            if (ReadStream(stream, header, headerSize) < headerSize)
            {
                throw new ApplicationException();
            }

            ushort len = ToUInt16(header, 0);
            ushort sequence = ToUInt16(header, 2);
            PacketType packetType = (PacketType)ToUInt32(header, 4);
            byte[] payload = new byte[len];
            if (ReadStream(stream, payload, len) < len)
            {
                throw new ApplicationException();
            }

            RawPacket packet = new RawPacket(sequence, packetType);
            packet.ParsePayload(payload);
            return packet;
        }

        private static ushort ToUInt16(byte[] buffer, int startIndex)
        {
            return (ushort)((buffer[startIndex] << 8) | buffer[startIndex + 1]);
        }

        private static uint ToUInt32(byte[] buffer, int startIndex)
        {
            return (uint)((buffer[startIndex] << 24) | (buffer[startIndex + 1] << 16) | (buffer[startIndex + 2] << 8) | buffer[startIndex + 3]);
        }

        private static int ReadStream(Stream stream, byte[] buffer, int size)
        {
            int bytesBuffered = 0;
            while (bytesBuffered < size)
            {
                int bytesRead = stream.Read(buffer, bytesBuffered, size - bytesBuffered);
                if (bytesRead == 0)
                {
                    break;
                }

                bytesBuffered += bytesRead;
            }

            return bytesBuffered;
        }
    }
}