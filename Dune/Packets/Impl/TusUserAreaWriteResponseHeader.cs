using System.IO;
using Dune.Utility;

namespace Dune.Packets.Impl
{
    public class TusUserAreaWriteResponseHeader : PacketBase
    {
        public override PacketType Type
        {
            get { return PacketType.TusUserAreaWriteResponseHeader; }
        }

        public short ChunkLength { get; set; }

        public override void ParsePayload(byte[] payload)
        {
            var reader = new BigEndianBinaryReader(new MemoryStream(payload));
            ChunkLength = reader.ReadInt16();
        }

        protected override byte[] SerializePayload()
        {
            byte[] payload = new byte[sizeof(int)];
            var writer = new BigEndianBinaryWriter(new MemoryStream(payload));
            writer.Write(ChunkLength);
            return payload;
        }
    }
}