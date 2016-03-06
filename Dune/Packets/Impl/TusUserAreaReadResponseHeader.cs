using System.IO;
using Dune.Utility;

namespace Dune.Packets.Impl
{
    public class TusUserAreaReadResponseHeader : PacketBase
    {
        public override PacketType Type
        {
            get { return PacketType.TusUserAreaReadResponseHeader; }
        }

        public int DataLength { get; set; }

        public override void ParsePayload(byte[] payload)
        {
            var reader = new BigEndianBinaryReader(new MemoryStream(payload));
            DataLength = reader.ReadInt32();
        }

        protected override byte[] SerializePayload()
        {
            byte[] payload = new byte[sizeof(int)];
            var writer = new BigEndianBinaryWriter(new MemoryStream(payload));
            // Default 2048
            writer.Write(DataLength);
            return payload;
        }
    }
}