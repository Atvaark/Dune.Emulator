namespace Dune.Packets.Impl
{
    public class OnlineCheckRequest : EmptyBase 
    {
        public override PacketType Type
        {
            get { return PacketType.OnlineCheckRequest; }
        }
    }
}