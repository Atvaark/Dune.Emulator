using System.IO;
using Dune.Utility;

namespace Dune.Packets.Impl
{
    public class AuthenticationInformationResponseHeader : PacketBase
    {
        public override PacketType Type
        {
            get { return PacketType.AuthenticationInformationResponseHeader; }
        }

        public ushort ChunkLength { get; set; }

        public override void ParsePayload(byte[] payload)
        {
            var reader = new BigEndianBinaryReader(new MemoryStream(payload));
            ChunkLength = reader.ReadUInt16();
        }

        protected override byte[] SerializePayload()
        {
            byte[] payload = new byte[sizeof(int)];
            var writer = new BigEndianBinaryWriter(new MemoryStream(payload));
            // Default 256
            writer.Write(ChunkLength);
            return payload;
        }
    }
}