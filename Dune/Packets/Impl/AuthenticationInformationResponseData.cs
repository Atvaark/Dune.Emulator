namespace Dune.Packets.Impl
{
    public class AuthenticationInformationResponseData : DataChunkResponseBase
    {
        public override PacketType Type
        {
            get { return PacketType.AuthenticationInformationResponseData; }
        }
    }
}