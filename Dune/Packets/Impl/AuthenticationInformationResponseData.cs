namespace Dune.Packets.Impl
{
    public class AuthenticationInformationResponseData : DataChunkReferenceBase
    {
        public override PacketType Type
        {
            get { return PacketType.AuthenticationInformationResponseData; }
        }
    }
}