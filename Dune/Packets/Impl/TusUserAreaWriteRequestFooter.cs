namespace Dune.Packets.Impl
{
    public class TusUserAreaWriteRequestFooter : EmptyBase 
    {
        public override PacketType Type
        {
            get { return PacketType.TusUserAreaWriteRequestFooter; }
        }
    }
}