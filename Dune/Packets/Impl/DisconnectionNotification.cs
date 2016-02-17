using System.IO;
using System.Text;
using Dune.Utility;

namespace Dune.Packets.Impl
{
    public class DisconnectionNotification : PacketBase
    {
        public override PacketType Type
        {
            get { return PacketType.DisconnectionNotification; }
        }

        public byte Unknown { get; set; }
        public string Notification { get; set; }

        public override void ParsePayload(byte[] payload)
        {
            var reader = new BigEndianBinaryReader(new MemoryStream(payload));
            Unknown = reader.ReadByte();
            ushort notificationLength = reader.ReadUInt16();
            Notification = Encoding.ASCII.GetString(reader.ReadBytes(notificationLength));
        }

        protected override byte[] SerializePayload()
        {
            byte[] notification = Encoding.ASCII.GetBytes(Notification);
            byte[] payload = new byte[sizeof(byte) + sizeof(ushort) + notification.Length];
            var writer = new BigEndianBinaryWriter(new MemoryStream(payload));
            writer.Write(Unknown);
            writer.Write((ushort)notification.Length);
            writer.Write(notification);
            return payload;
        }
    }
}