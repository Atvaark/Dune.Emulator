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

        public ushort ServerReadBufferLength { get; set; }

        public override void ParsePayload(byte[] payload)
        {
            var reader = new BigEndianBinaryReader(new MemoryStream(payload));
            ServerReadBufferLength = reader.ReadUInt16();
        }

        protected override byte[] SerializePayload()
        {
            byte[] payload = new byte[sizeof(int)];
            var writer = new BigEndianBinaryWriter(new MemoryStream(payload));
            // Default 256
            writer.Write(ServerReadBufferLength);
            return payload;
        }
    }
}