using System.IO;
using Dune.Utility;

namespace Dune.Packets.Impl
{
    public class AuthenticationInformationRequestHeader : PacketBase
    {
        public override PacketType Type
        {
            get { return PacketType.AuthenticationInformationRequestHeader; }
        }

        public int DataLength { get; set; }

        public override void ParsePayload(byte[] payload)
        {
            var reader = new BigEndianBinaryReader(new MemoryStream(payload));
            byte unknown = reader.ReadByte(); // 2
            DataLength = reader.ReadInt32();
        }

        protected override byte[] SerializePayload()
        {
            byte[] payload = new byte[sizeof(byte) + sizeof(int)];
            var writer = new BigEndianBinaryWriter(new MemoryStream(payload));
            writer.Write((byte)2);
            writer.Write(DataLength);
            return payload;
        }
    }
}