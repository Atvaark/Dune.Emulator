using System.IO;
using System.Text;
using Dune.Utility;

namespace Dune.Packets.Impl
{
    public class FastDataResponse : PacketBase
    {
        public override PacketType Type
        {
            get { return PacketType.FastDataResponse; }
        }
        
        public string User { get; set; }

        public override void ParsePayload(byte[] payload)
        {
            var reader = new BigEndianBinaryReader(new MemoryStream(payload));
            byte unknown1 = reader.ReadByte(); // 3
            uint unknown2 = reader.ReadUInt32(); // 1
            ushort userLength = reader.ReadUInt16();
            User = Encoding.ASCII.GetString(reader.ReadBytes(userLength));
        }

        protected override byte[] SerializePayload()
        {
            byte[] user = Encoding.ASCII.GetBytes(User);
            byte[] payload = new byte[sizeof(byte) + sizeof(uint) + sizeof(ushort) + user.Length];
            var writer = new BigEndianBinaryWriter(new MemoryStream(payload));

            writer.Write((byte)3);
            writer.Write((uint)1);

            writer.Write((ushort)user.Length);
            writer.Write(user);

            // char 3
            // int 1
            // short len
            // string steamId
            return payload;
        }
    }
}