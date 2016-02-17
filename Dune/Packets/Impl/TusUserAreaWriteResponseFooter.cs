namespace Dune.Packets.Impl
{
    public class TusUserAreaWriteResponseFooter : EmptyBase 
    {
        public override PacketType Type
        {
            get { return PacketType.TusUserAreaWriteResponseFooter; }
        }
    }
}