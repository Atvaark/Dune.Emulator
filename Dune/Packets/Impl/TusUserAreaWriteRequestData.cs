namespace Dune.Packets.Impl
{
    public class TusUserAreaWriteRequestData : DataChunkBase
    {
        public override PacketType Type
        {
            get { return PacketType.TusUserAreaWriteRequestData; }
        }
    }
}