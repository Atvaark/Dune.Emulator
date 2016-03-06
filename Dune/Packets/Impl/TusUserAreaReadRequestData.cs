namespace Dune.Packets.Impl
{
    public class TusUserAreaReadRequestData : DataChunkReferenceBase
    {
        public override PacketType Type
        {
            get { return PacketType.TusUserAreaReadRequestData; }
        }
    }
}