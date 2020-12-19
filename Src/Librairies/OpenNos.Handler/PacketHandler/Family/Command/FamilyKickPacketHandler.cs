using System;
using System.Linq;
using System.Reactive.Linq;
using NosTale.Packets.Packets.FamilyCommandPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;

namespace OpenNos.Handler.PacketHandler.Family.Command
{
    public class FamilyKickPacketHandler : IPacketHandler
    {
        #region Instantiation

        public FamilyKickPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void FamilyKick(FamilyKickPacket packet)
        {
            if (packet == null) return;

            if (Session.Character.Family?.FamilyCharacters == null || Session.Character.FamilyCharacter == null) return;

            if (Session.Character.FamilyCharacter.Authority == FamilyAuthority.Member
                || Session.Character.FamilyCharacter.Authority == FamilyAuthority.Familykeeper)
            {
                Session.SendPacket(
                    UserInterfaceHelper.GenerateInfo(
                        string.Format(Language.Instance.GetMessageFromKey("NOT_ALLOWED_KICK"))));
                return;
            }

            var characterName = packet.Name;

            Logger.LogUserEvent("GUILDCOMMAND", Session.GenerateIdentity(),
                $"[FamilyKick][{Session.Character.Family.FamilyId}]CharacterName: {characterName}");

            var familyCharacter =
                Session.Character.Family.FamilyCharacters.FirstOrDefault(s => s.Character.Name == characterName);

            if (familyCharacter?.FamilyId != Session.Character.Family.FamilyId) return;

            if (familyCharacter.Authority == FamilyAuthority.Head)
            {
                Session.SendPacket(
                    UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("CANT_KICK_HEAD")));
                return;
            }

            if (familyCharacter.CharacterId == Session.Character.CharacterId)
            {
                Session.SendPacket(
                    UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("CANT_KICK_YOURSELF")));
                return;
            }

            var kickSession = ServerManager.Instance.GetSessionByCharacterId(familyCharacter.CharacterId);

            if (kickSession != null)
            {
                DAOFactory.FamilyCharacterDAO.Delete(familyCharacter.CharacterId);

                Session.Character.Family.InsertFamilyLog(FamilyLogType.FamilyManaged, familyCharacter.Character.Name);

                kickSession.Character.Family = null;
                kickSession.Character.LastFamilyLeave = DateTime.Now.Ticks;

                Observable.Timer(TimeSpan.FromSeconds(5)).Subscribe(o =>
                    ServerManager.Instance.FamilyRefresh(Session.Character.Family.FamilyId));
            }
            else
            {
                if (CommunicationServiceClient.Instance.IsCharacterConnected(ServerManager.Instance.ServerGroup,
                    familyCharacter.CharacterId))
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateInfo(
                        Language.Instance.GetMessageFromKey("CANT_KICK_PLAYER_ONLINE_OTHER_CHANNEL")));
                    return;
                }

                DAOFactory.FamilyCharacterDAO.Delete(familyCharacter.CharacterId);

                Session.Character.Family.InsertFamilyLog(FamilyLogType.FamilyManaged, familyCharacter.Character.Name);

                var familyCharacterDTO = familyCharacter.Character;

                familyCharacterDTO.LastFamilyLeave = DateTime.Now.Ticks;

                DAOFactory.CharacterDAO.InsertOrUpdate(ref familyCharacterDTO);
            }
        }

        #endregion
    }
}