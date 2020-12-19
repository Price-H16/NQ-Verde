using System;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class UnbanHandler : IPacketHandler
    {
        #region Instantiation

        public UnbanHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Unban(UnbanPacket unbanPacket)
        {
            if (unbanPacket != null)
            {
                Session.AddLogsCmd(unbanPacket);
                var name = unbanPacket.CharacterName;
                var chara = DAOFactory.CharacterDAO.LoadByName(name);
                if (chara != null)
                {
                    var log = ServerManager.Instance.PenaltyLogs.Find(s =>
                        s.AccountId == chara.AccountId && s.Penalty == PenaltyType.Banned && s.DateEnd > DateTime.Now);
                    if (log != null)
                    {
                        log.DateEnd = DateTime.Now.AddSeconds(-1);
                        Character.InsertOrUpdatePenalty(log);
                        Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"),
                            10));
                    }
                    else
                    {
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_BANNED"), 10));
                    }
                }
                else
                {
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_FOUND"), 10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(UnbanPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}