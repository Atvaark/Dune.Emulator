using System.IO;
using System.Text;
using Dune.Utility;

namespace Dune.Packets.Impl
{
    public class TusUserAreaWriteRequestHeader : PacketBase
    {
        public override PacketType Type
        {
            get { return PacketType.TusUserAreaWriteRequestHeader; }
        }

        public int DataLength { get; set; }
        public string User { get; set; }

        public override void ParsePayload(byte[] payload)
        {
            var reader = new BigEndianBinaryReader(new MemoryStream(payload));
            ushort dataLength = reader.ReadUInt16();
            User = Encoding.ASCII.GetString(reader.ReadBytes(dataLength));
        }

        protected override byte[] SerializePayload()
        {
            byte[] user = Encoding.ASCII.GetBytes(User);
            byte[] payload = new byte[sizeof(int) + sizeof(ushort) + user.Length];
            var writer = new BigEndianBinaryWriter(new MemoryStream(payload));

            writer.Write(DataLength);
            writer.Write((ushort)user.Length);
            writer.Write(user);

            return payload;
        }
    }
}