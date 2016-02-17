namespace Dune.Packets.Impl
{
    public class AuthenticationInformationRequestFooter : EmptyBase 
    {
        public override PacketType Type
        {
            get { return PacketType.AuthenticationInformationRequestFooter; }
        }
    }
}