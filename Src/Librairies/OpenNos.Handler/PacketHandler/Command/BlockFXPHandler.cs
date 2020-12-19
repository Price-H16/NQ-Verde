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
    public class BlockFXPHandler : IPacketHandler
    {
        #region Instantiation

        public BlockFXPHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void BlockFExp(BlockFExpPacket blockFExpPacket)
        {
            if (blockFExpPacket != null)
            {
                Session.AddLogsCmd(blockFExpPacket);
                if (blockFExpPacket.Duration == 0) blockFExpPacket.Duration = 60;

                blockFExpPacket.Reason = blockFExpPacket.Reason?.Trim();
                var character = DAOFactory.CharacterDAO.LoadByName(blockFExpPacket.CharacterName);
                if (character != null)
                {
                    var session =
                        ServerManager.Instance.Sessions.FirstOrDefault(s =>
                            s.Character?.Name == blockFExpPacket.CharacterName);
                    session?.SendPacket(blockFExpPacket.Duration == 1
                        ? UserInterfaceHelper.GenerateInfo(
                            string.Format(Language.Instance.GetMessageFromKey("MUTED_SINGULAR"),
                                blockFExpPacket.Reason))
                        : UserInterfaceHelper.GenerateInfo(string.Format(
                            Language.Instance.GetMessageFromKey("MUTED_PLURAL"), blockFExpPacket.Reason,
                            blockFExpPacket.Duration)));
                    var log = new PenaltyLogDTO
                    {
                        AccountId = character.AccountId,
                        Reason = blockFExpPacket.Reason,
                        Penalty = PenaltyType.BlockFExp,
                        DateStart = DateTime.Now,
                        DateEnd = DateTime.Now.AddMinutes(blockFExpPacket.Duration),
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
                Session.SendPacket(Session.Character.GenerateSay(BlockFExpPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}