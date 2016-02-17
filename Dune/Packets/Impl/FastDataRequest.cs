namespace Dune.Packets.Impl
{
    public class FastDataRequest : EmptyBase 
    {
        public override PacketType Type
        {
            get { return PacketType.FastDataRequest;}
        }
    }
}