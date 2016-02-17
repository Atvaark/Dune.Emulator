namespace Dune.Packets.Impl
{
    public class TusUserAreaReadResponseData : DataChunkBase
    {
        public override PacketType Type
        {
            get { return PacketType.TusUserAreaReadResponseData; }
        }
    }
}