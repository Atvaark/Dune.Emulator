using System.IO;
using Dune.Utility;

namespace Dune.Packets.Impl
{
    public class ConnectionSummaryNotification : PacketBase
    {
        public override PacketType Type
        {
            get { return PacketType.ConnectionSummaryNotification; }
        }

        public bool Success { get; set; }
        public ushort Unknown { get; set; }

        public override void ParsePayload(byte[] payload)
        {
            var reader = new BigEndianBinaryReader(new MemoryStream(payload));
            Success = reader.ReadBoolean();
            Unknown = reader.ReadUInt16();
        }

        protected override byte[] SerializePayload()
        {
            // bool/char 01 
            // 00 0A
            byte[] payload = new byte[sizeof(byte) + sizeof(uint)];
            var writer = new BigEndianBinaryWriter(new MemoryStream(payload));
            writer.Write(Success);
            writer.Write(Unknown);
            return payload;
        }
    }
}