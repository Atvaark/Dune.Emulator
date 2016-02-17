namespace Dune.Packets.Impl
{
    public class TusUserAreaReadResponseFooter : EmptyBase 
    {
        public override PacketType Type
        {
            get { return PacketType.TusUserAreaReadResponseFooter; }
        }
    }
}