using System;

namespace Dune.Packets
{
    public abstract class BooleanBase : PacketBase
    {
        public bool Success { get; set; }

        public override void ParsePayload(byte[] payload)
        {
            Success = BitConverter.ToBoolean(payload, 0);
        }

        protected override byte[] SerializePayload()
        {
            return BitConverter.GetBytes(Success);
        }
    }
}