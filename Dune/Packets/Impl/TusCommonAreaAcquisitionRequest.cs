using System;

namespace Dune.Packets.Impl
{
    public class TusCommonAreaAcquisitionRequest : PacketBase
    {
        public override PacketType Type
        {
            get { return PacketType.TusCommonAreaAcquisitionRequest; }
        }

        public byte[] PropertyIndices { get; set; }

        public override void ParsePayload(byte[] payload)
        {
            byte length = payload[0];
            PropertyIndices = new byte[length];
            Buffer.BlockCopy(payload, 1, PropertyIndices, 0, length);
        }

        protected override byte[] SerializePayload()
        {
            byte[] payload = new byte[sizeof(byte) + PropertyIndices.Length];
            payload[0] = (byte) PropertyIndices.Length;
            Buffer.BlockCopy(PropertyIndices, 0, payload, 1, PropertyIndices.Length);
            return payload;
        }
    }
}