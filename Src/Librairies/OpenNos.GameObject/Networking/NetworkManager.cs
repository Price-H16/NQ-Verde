using System;
using System.Collections.Generic;
using System.Linq;
using ChickenAPI.Enums;
using OpenNos.Core;
using OpenNos.Core.Networking.Communication.Scs.Communication.EndPoints.Tcp;
using OpenNos.Core.Networking.Communication.Scs.Server;
using OpenNos.Domain;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject
{
    public class NetworkManager<EncryptorT> : SessionManager where EncryptorT : CryptographyBase
    {
        #region Instantiation

        public NetworkManager(string ipAddress, int port, Type packetHandler, Type fallbackEncryptor,
            bool isWorldServer) : base(packetHandler, isWorldServer)
        {
            _encryptor = (EncryptorT) Activator.CreateInstance(typeof(EncryptorT));

            if (fallbackEncryptor != null)
                _fallbackEncryptor = (CryptographyBase) Activator.CreateInstance(fallbackEncryptor);

            _server = ScsServerFactory.CreateServer(new ScsTcpEndPoint(ipAddress, port));

            // Register events of the server to be informed about clients
            _server.ClientConnected += OnServerClientConnected;
            _server.ClientDisconnected += OnServerClientDisconnected;
            _server.WireProtocolFactory = new WireProtocolFactory<EncryptorT>();

            // Start the server
            _server.Start();

            Logger.Info(Language.Instance.GetMessageFromKey("STARTED"), memberName: "NetworkManager");
        }

        #endregion

        #region Properties

        private IDictionary<string, DateTime> ConnectionLog =>
            _connectionLog ?? (_connectionLog = new Dictionary<string, DateTime>());

        #endregion

        #region Members

        private readonly EncryptorT _encryptor;

        private readonly CryptographyBase _fallbackEncryptor;

        private readonly IScsServer _server;

        private IDictionary<string, DateTime> _connectionLog;

        #endregion

        #region Methods

        public override void StopServer()
        {
            _server.Stop();
            _server.ClientConnected -= OnServerClientDisconnected;
            _server.ClientDisconnected -= OnServerClientConnected;
        }
        
        private string GetFailPacket(AuthResponse response) => $"failc {(byte)response}";

        
        protected override ClientSession IntializeNewSession(INetworkClient client)
        {
            if (!CheckGeneralLog(client))
            {
                Logger.Warn(string.Format(Language.Instance.GetMessageFromKey("FORCED_DISCONNECT"), client.ClientId));
                client.Initialize(_fallbackEncryptor);
                client.SendPacket(GetFailPacket(AuthResponse.CantConnect));
                client.Disconnect();
                return null;
            }

            var session = new ClientSession(client);
            session.Initialize(_encryptor, _packetHandler, IsWorldServer);

            return session;
        }

        private bool CheckGeneralLog(INetworkClient client)
        {
            if (!client.IpAddress.Contains("127.0.0.1") && ServerManager.Instance.ChannelId != 51)
            {
                if (ConnectionLog.Count > 0)
                    foreach (var item in ConnectionLog.Where(cl =>
                            cl.Key.Contains(client.IpAddress.Split(':')[1]) &&
                            (DateTime.Now - cl.Value).TotalSeconds > 3)
                        .ToList())
                        ConnectionLog.Remove(item.Key);

                if (ConnectionLog.Any(c => c.Key.Contains(client.IpAddress.Split(':')[1]))) return false;
                ConnectionLog.Add(client.IpAddress, DateTime.Now);
                return true;
            }

            return true;
        }

        private void OnServerClientConnected(object sender, ServerClientEventArgs e)
        {
            AddSession(e.Client as NetworkClient);
        }

        private void OnServerClientDisconnected(object sender, ServerClientEventArgs e)
        {
            RemoveSession(e.Client as NetworkClient);
        }

        #endregion
    }
}