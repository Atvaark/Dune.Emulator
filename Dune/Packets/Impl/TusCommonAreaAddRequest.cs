namespace Dune.Packets.Impl
{
    public class TusCommonAreaAddRequest : PropertyPacketBase
    {
        public override PacketType Type
        {
            get { return PacketType.TusCommonAreaAddRequest; }
        }
    }
}