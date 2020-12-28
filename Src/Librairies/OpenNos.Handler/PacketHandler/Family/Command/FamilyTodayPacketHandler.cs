using System;
using System.Linq;
using NosTale.Packets.Packets.FamilyCommandPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Family.Command
{
    public class FamilyTodayPacketHandler : IPacketHandler
    {
        #region Instantiation

        public FamilyTodayPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void TodayMessage(string packet)
        {
            if (Session.Character.Family != null && Session.Character.FamilyCharacter != null)
            {
                string msg = "";
                int i = 0;
                foreach (string str in packet.Split(' '))
                {
                    if (i > 1)
                    {
                        msg += str + " ";
                    }

                    i++;
                }

                Logger.LogUserEvent("GUILDCOMMAND", Session.GenerateIdentity(), $"[Today][{Session.Character.Family.FamilyId}]CharacterName: {Session.Character.Name} Title: {msg}");

                bool islog = Session.Character.Family.FamilyLogs.Any(s => s.FamilyLogType == FamilyLogType.DailyMessage && s.FamilyLogData.StartsWith(Session.Character.Name, StringComparison.CurrentCulture) && s.Timestamp.AddDays(1) > DateTime.Now);
                if (!islog)
                {
                    Session.Character.FamilyCharacter.DailyMessage = msg;
                    FamilyCharacterDTO fchar = Session.Character.FamilyCharacter;
                    DAOFactory.FamilyCharacterDAO.InsertOrUpdate(ref fchar);
                    Session.SendPacket(Session.Character.GenerateFamilyMemberMessage());
                    Session.Character.Family.InsertFamilyLog(FamilyLogType.DailyMessage, Session.Character.Name, message: msg);
                }
                else
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("CANT_CHANGE_MESSAGE")));
                }
            }
        }

        #endregion
    }
}