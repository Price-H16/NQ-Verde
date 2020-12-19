using System;
using System.Linq;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class BlockRepHandler : IPacketHandler
    {
        #region Instantiation

        public BlockRepHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void BlockRep(BlockRepPacket blockRepPacket)
        {
            if (blockRepPacket != null)
            {
                Session.AddLogsCmd(blockRepPacket);
                if (blockRepPacket.Duration == 0) blockRepPacket.Duration = 60;

                blockRepPacket.Reason = blockRepPacket.Reason?.Trim();
                var character = DAOFactory.CharacterDAO.LoadByName(blockRepPacket.CharacterName);
                if (character != null)
                {
                    var session =
                        ServerManager.Instance.Sessions.FirstOrDefault(s =>
                            s.Character?.Name == blockRepPacket.CharacterName);
                    session?.SendPacket(blockRepPacket.Duration == 1
                        ? UserInterfaceHelper.GenerateInfo(
                            string.Format(Language.Instance.GetMessageFromKey("MUTED_SINGULAR"), blockRepPacket.Reason))
                        : UserInterfaceHelper.GenerateInfo(string.Format(
                            Language.Instance.GetMessageFromKey("MUTED_PLURAL"), blockRepPacket.Reason,
                            blockRepPacket.Duration)));
                    var log = new PenaltyLogDTO
                    {
                        AccountId = character.AccountId,
                        Reason = blockRepPacket.Reason,
                        Penalty = PenaltyType.BlockRep,
                        DateStart = DateTime.Now,
                        DateEnd = DateTime.Now.AddMinutes(blockRepPacket.Duration),
                        AdminName = Session.Character.Name
                    };
                    Character.InsertOrUpdatePenalty(log);
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
                }
                else
                {
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_FOUND"), 10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(BlockRepPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}