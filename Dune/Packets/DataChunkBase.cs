using System.IO;
using Dune.Utility;

namespace Dune.Packets
{
    public abstract class DataChunkBase : PacketBase
    {
        public int Offset { get; set; }
        public ushort ChunkLength { get; set; }
        public byte[] Chunk { get; set; }

        public override void ParsePayload(byte[] payload)
        {
            var reader = new BigEndianBinaryReader(new MemoryStream(payload));
            Offset = reader.ReadInt32();
            ChunkLength = reader.ReadUInt16();

            Chunk = new byte[ChunkLength];
            ushort count = reader.ReadUInt16();
            reader.Read(Chunk, 0, count);
        }

        protected override byte[] SerializePayload()
        {
            byte[] payload = new byte[sizeof(uint) + 2 * sizeof(short) + ChunkLength];
            var writer = new BigEndianBinaryWriter(new MemoryStream(payload));
            writer.Write(Offset);
            writer.Write(ChunkLength);

            writer.Write(ChunkLength);
            writer.Write(Chunk, 0, ChunkLength);
            return payload;
        }
    }
}