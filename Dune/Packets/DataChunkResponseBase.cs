using System.IO;
using Dune.Utility;

namespace Dune.Packets
{
    public abstract class DataChunkResponseBase : PacketBase
    {
        public int ChunkOffset { get; set; }
        public ushort ChunkLength { get; set; }

        public override void ParsePayload(byte[] payload)
        {
            var reader = new BigEndianBinaryReader(new MemoryStream(payload));
            ChunkOffset = reader.ReadInt32();
            ChunkLength = reader.ReadUInt16();
        }

        protected override byte[] SerializePayload()
        {
            byte[] payload = new byte[sizeof(int) + sizeof(short)];
            var writer = new BigEndianBinaryWriter(new MemoryStream(payload));
            writer.Write(ChunkOffset);
            writer.Write(ChunkLength);
            return payload;
        }
    }
}