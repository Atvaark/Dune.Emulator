using System.IO;
using System.Text;
using Dune.Utility;

namespace Dune.Packets.Impl
{
    public class ReconnectionRequest : PacketBase
    {
        public override PacketType Type
        {
            get { return PacketType.ReconnectionRequest; }
        }

        public string Host { get; set; }
        public ushort Port { get; set; }

        public override void ParsePayload(byte[] payload)
        {
            var reader = new BigEndianBinaryReader(new MemoryStream(payload));
            ushort hostLength = reader.ReadUInt16();
            Host = Encoding.ASCII.GetString(reader.ReadBytes(hostLength));
            Port = reader.ReadUInt16();
        }

        protected override byte[] SerializePayload()
        {
            byte[] host = Encoding.ASCII.GetBytes(Host);
            byte[] payload = new byte[sizeof(ushort) + sizeof(ushort) + host.Length];
            var writer = new BigEndianBinaryWriter(new MemoryStream(payload));
            writer.Write((ushort)host.Length);
            writer.Write(host);
            writer.Write(Port);
            return payload;
        }
    }
}