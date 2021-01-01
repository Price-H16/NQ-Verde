using System;
using System.Collections.Concurrent;
using System.Linq;
using OpenNos.Core;
using OpenNos.Core.Networking;
using OpenNos.Domain;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject
{
    public class SessionManager
    {
        #region Instantiation

        public SessionManager(Type packetHandler, bool isWorldServer)
        {
            _packetHandler = packetHandler;
            IsWorldServer = isWorldServer;
        }

        #endregion

        #region Properties

        public bool IsWorldServer { get; set; }

        #endregion

        #region Members
        protected Type PacketHandler { get; }

        protected Type _packetHandler;

        protected ConcurrentDictionary<long, ClientSession> _sessions = new ConcurrentDictionary<long, ClientSession>();

        #endregion

        #region Methods
        public void AddSession(INetworkClient customClient)
        {
            Logger.Info(Language.Instance.GetMessageFromKey("NEW_CONNECT") + customClient.ClientId);

            var session = IntializeNewSession(customClient);
            customClient.SetClientSession(session);

            if (session != null && !_sessions.TryAdd(customClient.ClientId, session) && IsWorldServer)
            {
                Logger.Warn(string.Format(Language.Instance.GetMessageFromKey("FORCED_DISCONNECT"),
                    customClient.ClientId));
                customClient.Disconnect();
                _sessions.TryRemove(customClient.ClientId, out session);
            }
        }

        public virtual void StopServer()
        {
            _sessions.Clear();
            ServerManager.StopServer();
        }

        protected virtual ClientSession IntializeNewSession(INetworkClient client)
        {
            var session = new ClientSession(client);
            client.SetClientSession(session);
            return session;
        }

        protected void RemoveSession(INetworkClient client)
        {
            _sessions.TryRemove(client.ClientId, out var session);

            // check if session hasnt been already removed
            if (session != null)
            {
                session.IsDisposing = true;

                if (IsWorldServer && session.HasSelectedCharacter)
                {
                    session.Character.Mates.Where(s => s.IsTeamMember).ToList().ForEach(s => session.CurrentMapInstance?.Broadcast(session, s.GenerateOut(), ReceiverType.AllExceptMe));
                    session.CurrentMapInstance?.Broadcast(session, StaticPacketHelper.Out(UserType.Player, session.Character.CharacterId), ReceiverType.AllExceptMe);
                }

                session?.Destroy();

                if (IsWorldServer && session.HasSelectedCharacter)
                {
                    if (session.Character.Hp < 1)
                    {
                        session.Character.Hp = 1;
                    }

                    if (ServerManager.Instance.Groups.Any(s => s.IsMemberOfGroup(session.Character.CharacterId)))
                    {
                        ServerManager.Instance.GroupLeave(session);
                    }
                    session.Character.LeaveTalentArena(true);
                    session.Character.Save();
                    session.Character.BuffObservables?.Dispose();
                    session.Character.Life?.Dispose();
                }

                client.Disconnect();
                Logger.Info(Language.Instance.GetMessageFromKey("DISCONNECT") + client.ClientId);

                // session = null;
            }
        }

        #endregion
    }
}