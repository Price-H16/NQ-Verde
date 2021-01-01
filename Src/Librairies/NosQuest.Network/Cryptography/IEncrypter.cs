using System;

namespace NosQuest.Network.Cryptography
{
    public interface IEncrypter
    {
        #region Methods

        ReadOnlyMemory<byte> Encode(string packet);

        #endregion
    }
}