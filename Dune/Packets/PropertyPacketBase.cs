using System.IO;
using Dune.Utility;

namespace Dune.Packets
{
    public abstract class PropertyPacketBase : PacketBase
    {
        public TusProperty[] Properties;

        public override void ParsePayload(byte[] payload)
        {
            var reader = new BigEndianBinaryReader(new MemoryStream(payload));
            byte length = payload.Length == 0 ? (byte)0 : reader.ReadByte();
            Properties = new TusProperty[length];
            for (int i = 0; i < length; i++)
            {
                Properties[i] = new TusProperty(reader.ReadByte(), reader.ReadUInt32(), reader.ReadUInt32());
            }
        }

        protected override byte[] SerializePayload()
        {
            byte[] payload = new byte[sizeof(byte) + Properties.Length * TusProperty.Size];
            var writer = new BigEndianBinaryWriter(new MemoryStream(payload));
            writer.Write((byte)Properties.Length);
            foreach (var property in Properties)
            {
                writer.Write(property.Index);
                writer.Write(property.Value1);
                writer.Write(property.Value2);
            }

            return payload;
        }
    }
}