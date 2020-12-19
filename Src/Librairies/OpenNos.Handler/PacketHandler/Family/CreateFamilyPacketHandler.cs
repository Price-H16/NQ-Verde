using System;
using System.Reactive.Linq;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;

namespace OpenNos.Handler.PacketHandler.Family
{
    public class CreateFamilyPacketHandler : IPacketHandler
    {
        #region Instantiation

        public CreateFamilyPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods
        public void CreateFamily(CreateFamilyPacket createFamilyPacket)
        {
            if (Session.Character.Group?.GroupType == GroupType.Group && Session.Character.Group.SessionCount == 3)
            {
                foreach (var session in Session.Character.Group.Sessions.GetAllItems())
                    if (session.Character.Family != null || session.Character.FamilyCharacter != null)
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("PARTY_MEMBER_IN_FAMILY")));
                        return;
                    }
                    else if (session.Character.LastFamilyLeave > DateTime.Now.AddDays(-1).Ticks)
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("PARTY_MEMBER_HAS_PENALTY")));
                        return;
                    }

                if (Session.Character.Gold < 500000)
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY")));
                    return;
                }

                var name = createFamilyPacket.CharacterName;
                if (DAOFactory.FamilyDAO.LoadByName(name) != null)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("FAMILY_NAME_ALREADY_USED")));
                    return;
                }

                Session.Character.Gold -= 500000;
                Session.SendPacket(Session.Character.GenerateGold());
                var family = new FamilyDTO
                {
                    Name = name,
                    FamilyExperience = 0,
                    FamilyLevel = 1,
                    FamilyMessage = $"Welcome to {name}!",
                    FamilyFaction = Session.Character.Faction != FactionType.None ? (byte) Session.Character.Faction : (byte) ServerManager.RandomNumber(1, 2), // maybe doesn't work properly
                    MaxSize = 100,
                    WarehouseSize = 0 
                };
                DAOFactory.FamilyDAO.InsertOrUpdate(ref family);
                Logger.LogUserEvent("GUILDCREATE", Session.GenerateIdentity(), $"[FamilyCreate][{family.FamilyId}]");
                DiscordWebhookHelper.DiscordEventFamily($"[FamilyCreate][{family.FamilyId}]");
                ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("FAMILY_FOUNDED"), name), 0));
                foreach (var session in Session.Character.Group.Sessions.GetAllItems())
                {
                    session.Character.ChangeFaction(FactionType.None);
                    var familyCharacter = new FamilyCharacterDTO
                    {
                        CharacterId = session.Character.CharacterId,
                        DailyMessage = "",
                        Experience = 0,
                        Authority = Session.Character.CharacterId == session.Character.CharacterId ? FamilyAuthority.Head : FamilyAuthority.Familydeputy,
                        FamilyId = family.FamilyId,
                        Rank = 0
                    };
                    DAOFactory.FamilyCharacterDAO.InsertOrUpdate(ref familyCharacter);
                }

                ServerManager.Instance.FamilyRefresh(family.FamilyId);
                CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                {
                    DestinationCharacterId = family.FamilyId,
                    SourceCharacterId = Session.Character.CharacterId,
                    SourceWorldId = ServerManager.Instance.WorldId,
                    Message = "fhis_stc",
                    Type = MessageType.Family
                });
                Observable.Timer(TimeSpan.FromSeconds(5)).Subscribe(o =>
                    ServerManager.Instance.FamilyRefresh(family.FamilyId));
                Observable.Timer(TimeSpan.FromSeconds(10)).Subscribe(o =>
                    ServerManager.Instance.FamilyRefresh(family.FamilyId));
            }
        }

        #endregion
    }
}