namespace Dune.Packets.Impl
{
    public class TusUserAreaWriteResponseData : DataChunkReferenceBase
    {
        public override PacketType Type
        {
            get { return PacketType.TusUserAreaWriteResponseData; }
        }
    }
}