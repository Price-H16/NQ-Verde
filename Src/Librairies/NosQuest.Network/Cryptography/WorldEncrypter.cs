using System;
using System.Text;

namespace NosQuest.Network.Cryptography
{
    public class WorldEncrypter : IEncrypter
    {
        #region Members

        private readonly INetworkClientInformation _client;

        #endregion

        #region Instantiation

        public WorldEncrypter(INetworkClientInformation client) => _client = client;

        #endregion

        #region Methods

        public ReadOnlyMemory<byte> Encode(string packet)
        {
            byte[] strBytes = Encoding.Convert(Encoding.UTF8, _client.Encoding, Encoding.UTF8.GetBytes(packet));
            byte[] encryptedData = new byte[strBytes.Length + (int)Math.Ceiling((decimal)strBytes.Length / 126) + 1];

            int j = 0;
            for (int i = 0; i < strBytes.Length; i++)
            {
                if ((i % 126) == 0)
                {
                    encryptedData[i + j] = (byte)(strBytes.Length - i > 126 ? 126 : strBytes.Length - i);
                    j++;
                }

                encryptedData[i + j] = (byte)~strBytes[i];
            }

            encryptedData[encryptedData.Length - 1] = 0xFF;
            return encryptedData;
        }

        #endregion
    }
}