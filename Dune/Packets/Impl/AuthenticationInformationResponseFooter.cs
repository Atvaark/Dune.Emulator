namespace Dune.Packets.Impl
{
    public class AuthenticationInformationResponseFooter : BooleanBase
    {
        public override PacketType Type
        {
            get { return PacketType.AuthenticationInformationResponseFooter; }
        }
    }
}