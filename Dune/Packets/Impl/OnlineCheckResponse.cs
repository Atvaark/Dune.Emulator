namespace Dune.Packets.Impl
{
    public class OnlineCheckResponse : EmptyBase 
    {
        public override PacketType Type
        {
            get { return PacketType.OnlineCheckResponse; }
        }
    }
}