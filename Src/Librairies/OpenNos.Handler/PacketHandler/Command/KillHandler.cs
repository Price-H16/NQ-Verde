using System;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class KillHandler : IPacketHandler
    {
        #region Instantiation

        public KillHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Kill(KillPacket killPacket)
        {
            if (killPacket != null)
            {
                Session.AddLogsCmd(killPacket);
                var sess = ServerManager.Instance.GetSessionByCharacterName(killPacket.CharacterName);
                if (sess != null)
                {
                    if (sess.Character.HasGodMode) return;

                    if (sess.Character.Hp < 1) return;

                    sess.Character.Hp = 0;
                    sess.Character.LastDefence = DateTime.Now;
                    Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.SkillUsed(UserType.Player,
                        Session.Character.CharacterId, 1, sess.Character.CharacterId, 1114, 4, 11, 4260, 0, 0, false, 0,
                        60000, 3, 0));
                    sess.SendPacket(sess.Character.GenerateStat());
                    if (sess.Character.IsVehicled) sess.Character.RemoveVehicle();
                    ServerManager.Instance.AskRevive(sess.Character.CharacterId);
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("USER_NOT_CONNECTED"), 0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(KillPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}