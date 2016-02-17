namespace Dune.Packets.Impl
{
    public class TusUserAreaReadRequestFooter : EmptyBase 
    {
        public override PacketType Type
        {
            get { return PacketType.TusUserAreaReadRequestFooter; }
        }
    }
}