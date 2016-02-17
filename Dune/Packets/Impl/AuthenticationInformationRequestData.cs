namespace Dune.Packets.Impl
{
    public class AuthenticationInformationRequestData : DataChunkBase
    {
        public override PacketType Type
        {
            get { return PacketType.AuthenticationInformationRequestData; }
        }
    }
}