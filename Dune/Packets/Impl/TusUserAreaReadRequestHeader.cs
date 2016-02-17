using System.IO;
using System.Text;
using Dune.Utility;

namespace Dune.Packets.Impl
{
    public class TusUserAreaReadRequestHeader : PacketBase
    {
        public override PacketType Type
        {
            get { return PacketType.TusUserAreaReadRequestHeader; }
        }

        public string User { get; set; }

        public override void ParsePayload(byte[] payload)
        {
            var reader = new BigEndianBinaryReader(new MemoryStream(payload));
            ushort dataLength = reader.ReadUInt16();
            User = Encoding.ASCII.GetString(reader.ReadBytes(dataLength));
        }

        protected override byte[] SerializePayload()
        {
            byte[] data = Encoding.ASCII.GetBytes(User);
            byte[] payload = new byte[sizeof(ushort) + data.Length];
            var writer = new BigEndianBinaryWriter(new MemoryStream(payload));

            writer.Write((ushort)data.Length);
            writer.Write(data);

            return payload;
        }
    }
}