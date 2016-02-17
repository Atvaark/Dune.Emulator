namespace Dune.Packets.Impl
{
    public class TusUserAreaWriteResponseData : DataChunkResponseBase
    {
        public override PacketType Type
        {
            get { return PacketType.TusUserAreaWriteResponseData; }
        }
    }
}