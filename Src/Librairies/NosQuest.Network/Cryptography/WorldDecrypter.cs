using NosQuest.Network.Extensions;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace NosQuest.Network.Cryptography
{
    public class WorldDecrypter : IDecrypter
    {
        #region Members

        private readonly INetworkClientInformation _client;
        private readonly Encoding _encoding = Encoding.Default;

        #endregion

        #region Instantiation

        public WorldDecrypter(INetworkClientInformation client) => _client = client;

        #endregion

        #region Properties

        private int EncryptionKey => _client.SessionId;

        #endregion

        #region Methods

        public string Decode(ReadOnlySpan<byte> bytesBuffer)
        {
            if (EncryptionKey == 0)
            {
                return DecryptUnauthed(bytesBuffer);
            }

            return DecryptAuthed(bytesBuffer);
        }

        private string DecryptAuthed(ReadOnlySpan<byte> str)
        {
            var encryptedString = new StringBuilder();

            int sessionKey = EncryptionKey & 0xFF;
            byte sessionNumber = unchecked((byte)(EncryptionKey >> 6));
            sessionNumber &= 0xFF;
            sessionNumber &= 3;

            switch (sessionNumber)
            {
                case 0:
                    foreach (byte character in str)
                    {
                        byte firstbyte = unchecked((byte)(sessionKey + 0x40));
                        byte highbyte = unchecked((byte)(character - firstbyte));
                        encryptedString.Append((char)highbyte);
                    }

                    break;

                case 1:
                    foreach (byte character in str)
                    {
                        byte firstbyte = unchecked((byte)(sessionKey + 0x40));
                        byte highbyte = unchecked((byte)(character + firstbyte));
                        encryptedString.Append((char)highbyte);
                    }

                    break;

                case 2:
                    foreach (byte character in str)
                    {
                        byte firstbyte = unchecked((byte)(sessionKey + 0x40));
                        byte highbyte = unchecked((byte)(character - firstbyte ^ 0xC3));
                        encryptedString.Append((char)highbyte);
                    }

                    break;

                case 3:
                    foreach (byte character in str)
                    {
                        byte firstbyte = unchecked((byte)(sessionKey + 0x40));
                        byte highbyte = unchecked((byte)(character + firstbyte ^ 0xC3));
                        encryptedString.Append((char)highbyte);
                    }

                    break;

                default:
                    encryptedString.Append((char)0xF);
                    break;
            }

            string[] temp = encryptedString.ToString().Split((char)0xFF);

            var save = new StringBuilder();

            for (int i = 0; i < temp.Length; i++)
            {
                save.Append(DecryptPrivate(temp[i].AsSpan()));
                if (i < temp.Length - 2)
                {
                    save.Append((char)0xFF);
                }
            }

            return save.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string DecryptPrivate(ReadOnlySpan<char> str)
        {
            var receiveData = new MemoryStream();
            char[] table = { ' ', '-', '.', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'n' };
            for (int count = 0; count < str.Length; count++)
            {
                if (str[count] <= 0x7A)
                {
                    int len = str[count];

                    for (int i = 0; i < len; i++)
                    {
                        count++;

                        try
                        {
                            receiveData.Append(unchecked((byte)(str[count] ^ 0xFF)));
                        }
                        catch
                        {
                            receiveData.Append(255);
                        }
                    }
                }
                else
                {
                    int len = str[count];
                    len &= 0x7F;

                    for (int i = 0; i < len; i++)
                    {
                        count++;
                        int highbyte;
                        try
                        {
                            highbyte = str[count];
                        }
                        catch
                        {
                            highbyte = 0;
                        }

                        highbyte &= 0xF0;
                        highbyte >>= 0x4;

                        int lowbyte;
                        try
                        {
                            lowbyte = str[count];
                        }
                        catch
                        {
                            lowbyte = 0;
                        }

                        lowbyte &= 0x0F;

                        if (highbyte != 0x0 && highbyte != 0xF)
                        {
                            receiveData.Append(unchecked((byte)table[highbyte - 1]));
                            i++;
                        }

                        if (lowbyte != 0x0 && lowbyte != 0xF)
                        {
                            receiveData.Append(unchecked((byte)table[lowbyte - 1]));
                        }
                    }
                }
            }

            return _encoding.GetString(receiveData.ToArray());
        }

        private string DecryptUnauthed(in ReadOnlySpan<byte> str)
        {
            try
            {
                var encryptedStringBuilder = new StringBuilder();
                for (int i = 1; i < str.Length; i++)
                {
                    if (Convert.ToChar(str[i]) == 0xE)
                    {
                        return encryptedStringBuilder.ToString();
                    }

                    int firstbyte = Convert.ToInt32(str[i] - 0xF);
                    int secondbyte = firstbyte;
                    secondbyte &= 240;
                    firstbyte = Convert.ToInt32(firstbyte - secondbyte);
                    secondbyte >>= 4;

                    switch (secondbyte)
                    {
                        case 0:
                        case 1:
                            encryptedStringBuilder.Append(' ');
                            break;

                        case 2:
                            encryptedStringBuilder.Append('-');
                            break;

                        case 3:
                            encryptedStringBuilder.Append('.');
                            break;

                        default:
                            secondbyte += 0x2C;
                            encryptedStringBuilder.Append(Convert.ToChar(secondbyte));
                            break;
                    }

                    switch (firstbyte)
                    {
                        case 0:
                            encryptedStringBuilder.Append(' ');
                            break;

                        case 1:
                            encryptedStringBuilder.Append(' ');
                            break;

                        case 2:
                            encryptedStringBuilder.Append('-');
                            break;

                        case 3:
                            encryptedStringBuilder.Append('.');
                            break;

                        default:
                            firstbyte += 0x2C;
                            encryptedStringBuilder.Append(Convert.ToChar(firstbyte));
                            break;
                    }
                }

                return encryptedStringBuilder.ToString();
            }
            catch (OverflowException)
            {
                return string.Empty;
            }
        }

        #endregion
    }
}