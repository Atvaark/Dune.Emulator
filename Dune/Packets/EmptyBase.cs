namespace Dune.Packets
{
    public abstract class EmptyBase : PacketBase
    {
        public override void ParsePayload(byte[] payload)
        {
            // empty
        }

        protected override byte[] SerializePayload()
        {
            // empty
            return new byte[0];
        }
    }
}