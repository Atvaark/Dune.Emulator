using System;

namespace Dune.Packets
{
    public abstract class PacketBase
    {
        /// <summary>
        /// Requests and notifications have to create a new sequence number.
        /// Responses have to use the sequence number of the request.
        /// Client and host can use different seeds when creating sequence numbers.
        /// </summary>
        public ushort Sequence { get; set; }
        public abstract PacketType Type { get; }

        public abstract void ParsePayload(byte[] payload);

        public byte[] Serialize()
        {
            byte[] payload = SerializePayload();
            const int headerSize = 2 * sizeof(ushort) + sizeof(uint);
            byte[] packetBuffer = new byte[headerSize + payload.Length];
            WriteUInt16((ushort)payload.Length, packetBuffer, 0);
            WriteUInt16(Sequence, packetBuffer, 2);
            WriteUInt32((uint)Type, packetBuffer, 4);
            Array.Copy(payload, 0, packetBuffer, 8, payload.Length);
            return packetBuffer;
        }

        protected abstract byte[] SerializePayload();

        private static void WriteUInt16(ushort value, byte[] buffer, int startIndex)
        {
            buffer[startIndex] = (byte)(value >> 8);
            buffer[startIndex + 1] = (byte)value;
        }
        private static void WriteUInt32(uint value, byte[] buffer, int startIndex)
        {
            buffer[startIndex] = (byte)(value >> 24);
            buffer[startIndex + 1] = (byte)(value >> 16);
            buffer[startIndex + 2] = (byte)(value >> 8);
            buffer[startIndex + 3] = (byte)value;
        }
    }
}