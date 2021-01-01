using System;

namespace NosQuest.Network.Cryptography
{
    public interface IDecrypter
    {
        #region Methods

        string Decode(ReadOnlySpan<byte> bytesBuffer);

        #endregion
    }
}