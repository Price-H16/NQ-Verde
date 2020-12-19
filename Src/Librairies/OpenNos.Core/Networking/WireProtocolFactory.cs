using OpenNos.Core.Networking.Communication.Scs.Communication.Protocols;

namespace OpenNos.Core
{
    public class WireProtocolFactory<EncryptorT> : IScsWireProtocolFactory where EncryptorT : CryptographyBase
    {
        #region Methods

        public IScsWireProtocol CreateWireProtocol() => new WireProtocol();

        #endregion
    }
}