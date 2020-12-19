using NosTale.Packets.Packets.FamilyCommandPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;

namespace OpenNos.Handler.PacketHandler.Family.Command
{
    public class FamilyTitleChangePacketHandler : IPacketHandler
    {
        #region Instantiation

        public FamilyTitleChangePacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void TitleChange(FamilyTitleChangePacket packet)
        {
            if (Session.Character.Family != null && Session.Character.FamilyCharacter != null
                                                 && Session.Character.FamilyCharacter.Authority == FamilyAuthority.Head)
            {
                var packetsplit = packet.Data.Split(' ');
                if (packetsplit.Length != 4) return;

                FamilyCharacterDTO fchar =
                    Session.Character.Family.FamilyCharacters.Find(s => s.Character.Name == packetsplit[2]);
                if (fchar != null && byte.TryParse(packetsplit[3], out var rank))
                {
                    fchar.Rank = (FamilyMemberRank) rank;

                    Logger.LogUserEvent("GUILDCOMMAND", Session.GenerateIdentity(),
                        $"[Title][{Session.Character.Family.FamilyId}]CharacterName: {packetsplit[2]} Title: {fchar.Rank.ToString()}");

                    DAOFactory.FamilyCharacterDAO.InsertOrUpdate(ref fchar);
                    ServerManager.Instance.FamilyRefresh(Session.Character.Family.FamilyId);
                    CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                    {
                        DestinationCharacterId = Session.Character.Family.FamilyId,
                        SourceCharacterId = Session.Character.CharacterId,
                        SourceWorldId = ServerManager.Instance.WorldId,
                        Message = "fhis_stc",
                        Type = MessageType.Family
                    });
                    Session.SendPacket(Session.Character.GenerateFamilyMember());
                    Session.SendPacket(Session.Character.GenerateFamilyMemberMessage());
                }
            }
        }

        #endregion
    }
}