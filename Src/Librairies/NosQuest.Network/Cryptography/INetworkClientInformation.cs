using System.Text;

namespace NosQuest.Network.Cryptography
{
    public interface INetworkClientInformation
    {
        #region Properties

        Encoding Encoding { get; }

        int SessionId { get; }

        #endregion
    }
}
