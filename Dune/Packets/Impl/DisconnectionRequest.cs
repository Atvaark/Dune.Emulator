namespace Dune.Packets.Impl
{
    public class DisconnectionRequest : BooleanBase
    {
        public override PacketType Type
        {
            get { return PacketType.DisconnectionRequest; }
        }
    }
}