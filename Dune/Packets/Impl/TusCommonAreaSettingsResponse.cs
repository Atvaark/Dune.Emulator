namespace Dune.Packets.Impl
{
    public class TusCommonAreaSettingsResponse : PropertyPacketBase
    {
        public override PacketType Type
        {
            get { return PacketType.TusCommonAreaSettingsResponse; }
        }
    }
}