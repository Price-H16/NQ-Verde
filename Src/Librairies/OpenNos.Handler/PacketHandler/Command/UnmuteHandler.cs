﻿using System;
using System.Linq;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class UnmuteHandler : IPacketHandler
    {
        #region Instantiation

        public UnmuteHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Unmute(UnmutePacket unmutePacket)
        {
            if (unmutePacket != null)
            {
                Session.AddLogsCmd(unmutePacket);
                var name = unmutePacket.CharacterName;
                var chara = DAOFactory.CharacterDAO.LoadByName(name);
                if (chara != null)
                {
                    if (ServerManager.Instance.PenaltyLogs.Any(s =>
                        s.AccountId == chara.AccountId && s.Penalty == (byte) PenaltyType.Muted
                                                       && s.DateEnd > DateTime.Now))
                    {
                        var log = ServerManager.Instance.PenaltyLogs.Find(s =>
                            s.AccountId == chara.AccountId && s.Penalty == (byte) PenaltyType.Muted
                                                           && s.DateEnd > DateTime.Now);
                        if (log != null)
                        {
                            log.DateEnd = DateTime.Now.AddSeconds(-1);
                            Character.InsertOrUpdatePenalty(log);
                        }

                        Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"),
                            10));
                    }
                    else
                    {
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_MUTED"), 10));
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
                Session.SendPacket(Session.Character.GenerateSay(UnmutePacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}