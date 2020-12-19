using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.GameObject.RainbowBattle;
using System.Linq;

namespace OpenNos.Handler.PacketHandler.Basic
{
    public class FbPacket : IPacketHandler
    {
        #region Instantiation

        public FbPacket(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Fb(FbPacket e)
        {
            if (e == null)
            {
                return;
            }

            var rbb = ServerManager.Instance.RainbowBattleMembers.Find(s => s.Session.Contains(Session));

            if (rbb == null)
            {
                return;
            }

            rbb.Session.Remove(Session);
            RainbowBattleManager.SendFbs(Session.CurrentMapInstance);
            Session.Character.Group?.LeaveGroup(Session);
            ServerManager.Instance.UpdateGroup(Session.Character.CharacterId);
            ServerManager.Instance.ChangeMap(Session.Character.CharacterId, Session.Character.MapId, Session.Character.MapX, Session.Character.MapY);
            Session.SendPacket(Session.Character.GenerateRaid(2, true));
        }
        #endregion
    }
}