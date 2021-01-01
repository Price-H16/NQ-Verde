using System;
using System.Text;

namespace NosQuest.Network.Cryptography
{
    public class NostaleLoginEncrypter : IEncrypter
    {
        #region Members

        private readonly Encoding _encoding;

        #endregion

        #region Instantiation

        public NostaleLoginEncrypter(Encoding encoding) => _encoding = encoding;

        #endregion

        #region Methods

        public ReadOnlyMemory<byte> Encode(string packet)
        {
            byte[] tmp = _encoding.GetBytes(packet);
            if (tmp.Length == 0)
            {
                return null;
            }

            for (int i = 0; i < packet.Length; i++)
            {
                tmp[i] = Convert.ToByte(tmp[i] + 15);
            }

            tmp[tmp.Length - 1] = 25;
            return tmp;
        }

        #endregion
    }
}