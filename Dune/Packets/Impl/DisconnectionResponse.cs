namespace Dune.Packets.Impl
{
    public class DisconnectionResponse : BooleanBase
    {
        public override PacketType Type
        {
            get { return PacketType.DisconnectionResponse; }
        }
    }
}