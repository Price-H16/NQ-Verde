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
    public class BlockXPHandler : IPacketHandler
    {
        #region Instantiation

        public BlockXPHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void BlockExp(BlockExpPacket blockExpPacket)
        {
            if (blockExpPacket != null)
            {
                Session.AddLogsCmd(blockExpPacket);
                if (blockExpPacket.Duration == 0) blockExpPacket.Duration = 60;

                blockExpPacket.Reason = blockExpPacket.Reason?.Trim();
                var character = DAOFactory.CharacterDAO.LoadByName(blockExpPacket.CharacterName);
                if (character != null)
                {
                    var session =
                        ServerManager.Instance.Sessions.FirstOrDefault(s =>
                            s.Character?.Name == blockExpPacket.CharacterName);
                    session?.SendPacket(blockExpPacket.Duration == 1
                        ? UserInterfaceHelper.GenerateInfo(
                            string.Format(Language.Instance.GetMessageFromKey("MUTED_SINGULAR"), blockExpPacket.Reason))
                        : UserInterfaceHelper.GenerateInfo(string.Format(
                            Language.Instance.GetMessageFromKey("MUTED_PLURAL"), blockExpPacket.Reason,
                            blockExpPacket.Duration)));
                    var log = new PenaltyLogDTO
                    {
                        AccountId = character.AccountId,
                        Reason = blockExpPacket.Reason,
                        Penalty = PenaltyType.BlockExp,
                        DateStart = DateTime.Now,
                        DateEnd = DateTime.Now.AddMinutes(blockExpPacket.Duration),
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
                Session.SendPacket(Session.Character.GenerateSay(BlockExpPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}