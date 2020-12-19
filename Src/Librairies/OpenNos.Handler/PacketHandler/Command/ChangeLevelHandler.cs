using System;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class ChangeLevelHandler : IPacketHandler
    {
        #region Instantiation

        public ChangeLevelHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void ChangeLevel(ChangeLevelPacket changeLevelPacket)
        {
            if (changeLevelPacket != null && !Session.Character.IsSeal)
            {
                Session.AddLogsCmd(changeLevelPacket);
                if (changeLevelPacket.Level > 0)
                {
                    Session.Character.Level = Math.Min(changeLevelPacket.Level,
                        ServerManager.Instance.Configuration.MaxLevel);
                    Session.Character.LevelXp = 0;
                    Session.Character.Hp = (int) Session.Character.HPLoad();
                    Session.Character.Mp = (int) Session.Character.MPLoad();
                    Session.SendPacket(Session.Character.GenerateStat());
                    Session.SendPackets(Session.Character.GenerateStatChar());
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("LEVEL_CHANGED"), 0));
                    Session.SendPacket(Session.Character.GenerateLev());
                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateIn(),
                        ReceiverType.AllExceptMe);
                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateGidx(),
                        ReceiverType.AllExceptMe);
                    Session.CurrentMapInstance?.Broadcast(
                        StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 6),
                        Session.Character.PositionX, Session.Character.PositionY);
                    Session.CurrentMapInstance?.Broadcast(
                        StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 198),
                        Session.Character.PositionX, Session.Character.PositionY);
                    ServerManager.Instance.UpdateGroup(Session.Character.CharacterId);
                    if (Session.Character.Family != null)
                    {
                        ServerManager.Instance.FamilyRefresh(Session.Character.Family.FamilyId);
                        CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                        {
                            DestinationCharacterId = Session.Character.Family.FamilyId,
                            SourceCharacterId = Session.Character.CharacterId,
                            SourceWorldId = ServerManager.Instance.WorldId,
                            Message = "fhis_stc",
                            Type = MessageType.Family
                        });
                    }

                    //Session.Character.LevelRewards(Session.Character.Level);
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("WRONG_VALUE"), 0));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ChangeLevelPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}