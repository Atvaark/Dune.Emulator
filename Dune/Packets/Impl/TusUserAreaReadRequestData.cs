namespace Dune.Packets.Impl
{
    public class TusUserAreaReadRequestData : DataChunkResponseBase
    {
        public override PacketType Type
        {
            get { return PacketType.TusUserAreaReadRequestData; }
        }
    }
}