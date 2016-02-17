namespace Dune.Packets.Impl
{
    public class TusCommonAreaAcquisitionResponse : PropertyPacketBase
    {
        public override PacketType Type
        {
            get { return PacketType.TusCommonAreaAcquisitionResponse; }
        }
    }
}