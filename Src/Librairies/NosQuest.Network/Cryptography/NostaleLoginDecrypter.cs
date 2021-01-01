using System;
using System.Text;

namespace NosQuest.Network.Cryptography
{
    public class NostaleLoginDecrypter : IDecrypter
    {
        #region Methods

        public string Decode(ReadOnlySpan<byte> bytesBuffer)
        {
            var decryptedPacket = new StringBuilder();
            foreach (byte character in bytesBuffer)
            {
                decryptedPacket.Append(character > 14
                    ? Convert.ToChar(character - 0xF ^ 0xC3)
                    : Convert.ToChar(0x100 - (0xF - character) ^ 195));
            }

            return decryptedPacket.ToString();
        }

        #endregion
    }
}