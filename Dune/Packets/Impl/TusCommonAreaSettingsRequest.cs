namespace Dune.Packets.Impl
{
    public class TusCommonAreaSettingsRequest : PropertyPacketBase
    {
        public override PacketType Type
        {
            get { return PacketType.TusCommonAreaSettingsRequest; }
        }
    }
}