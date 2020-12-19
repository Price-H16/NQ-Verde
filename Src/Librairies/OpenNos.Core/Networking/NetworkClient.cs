using System.Collections.Generic;
using OpenNos.Core.Networking.Communication.Scs.Communication;
using OpenNos.Core.Networking.Communication.Scs.Communication.Channels;
using OpenNos.Core.Networking.Communication.Scs.Communication.Messages;
using OpenNos.Core.Networking.Communication.Scs.Server;

namespace OpenNos.Core
{
    public class NetworkClient : ScsServerClient, INetworkClient
    {
        #region Instantiation

        public NetworkClient(ICommunicationChannel communicationChannel) : base(communicationChannel)
        {
        }

        #endregion

        #region Members

        private CryptographyBase _encryptor;
        private object _session;

        #endregion

        #region Properties

        public string IpAddress => RemoteEndPoint.ToString();

        public bool IsConnected => CommunicationState == CommunicationStates.Connected;

        public bool IsDisposing { get; set; }

        #endregion

        #region Methods

        public void Initialize(CryptographyBase encryptor)
        {
            _encryptor = encryptor;
        }

        public void SendPacket(string packet, byte priority = 10)
        {
            if (!IsDisposing && packet != null && packet != "")
            {
                var rawMessage = new ScsRawDataMessage(_encryptor.Encrypt(packet));
                SendMessage(rawMessage, priority);
            }
        }

        public void SendPacketFormat(string packet, params object[] param)
        {
            SendPacket(string.Format(packet, param));
        }

        public void SendPackets(IEnumerable<string> packets, byte priority = 10)
        {
            foreach (var packet in packets) SendPacket(packet, priority);
        }

        public void SetClientSession(object clientSession)
        {
            _session = clientSession;
        }

        #endregion
    }
}