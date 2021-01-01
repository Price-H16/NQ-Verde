using System;
using System.Collections.Generic;
using System.Linq;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;

namespace OpenNos.Handler.BasicPacket.CharScreen
{
    public class DeleteCharacterPacketHandler : IPacketHandler
    {
        #region Instantiation

        public DeleteCharacterPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        private ClientSession Session { get; }
        public DateTime LastDelete { get; private set; }

        #endregion

        #region Methods

        public void DeleteCharacter(CharacterDeletePacket characterDeletePacket)
        {
            if (Session.HasCurrentMapInstance)
            {
                return;
            }

            if (characterDeletePacket.Password == null)
            {
                return;
            }

            Logger.LogUserEvent("DELETECHARACTER", Session.GenerateIdentity(), $"[DeleteCharacter]Name: {characterDeletePacket.Slot}");
            LastDelete = DateTime.Now.AddDays(1);
            var account = DAOFactory.AccountDAO.LoadById(Session.Account.AccountId);
            if (account == null)
            {
                return;
            }

            if (account.Password.ToLower() == CryptographyBase.Sha512(characterDeletePacket.Password))
            {
                var character = DAOFactory.CharacterDAO.LoadBySlot(account.AccountId, characterDeletePacket.Slot);
                if (character == null)
                {
                    return;
                }

                // Remove all relations from deleted character
                var relationshipList = ServerManager.Instance.CharacterRelations.Where(s => s.CharacterId == character.CharacterId || s.RelatedCharacterId == character.CharacterId).ToList();

                foreach (var relation in relationshipList)
                {
                    DeleteRelation(character.CharacterId, relationshipList, relation.RelatedCharacterId, relation.RelationType);
                }

                DAOFactory.GeneralLogDAO.SetCharIdNull(Convert.ToInt64(character.CharacterId));
                DAOFactory.CharacterDAO.DeleteByPrimaryKey(account.AccountId, characterDeletePacket.Slot);
                new EntryPointPacketHandler(Session).LoadCharacters(new OpenNosEntryPointPacket
                {
                    PacketData = string.Empty
                }); ;

                FamilyCharacterDTO familyCharacter = DAOFactory.FamilyCharacterDAO.LoadByCharacterId(character.CharacterId);
                if (familyCharacter == null)
                {
                    new EntryPointPacketHandler(Session).LoadCharacters(new OpenNosEntryPointPacket
                    {
                        PacketData = string.Empty
                    }); ;
                    return;
                }

                // REMOVE FROM FAMILY
                DAOFactory.FamilyCharacterDAO.Delete(character.CharacterId);
                ServerManager.Instance.FamilyRefresh(familyCharacter.FamilyId);
            }
            else
            {
                Session.SendPacket($"info {Language.Instance.GetMessageFromKey("BAD_PASSWORD")}");
            }
        }

        private static void DeleteRelation(long mainCharacterId, List<CharacterRelationDTO> relations, long characterId, CharacterRelationType relationType)
        {
            CharacterRelationDTO chara = relations.Find(s => (s.RelatedCharacterId == characterId || s.CharacterId == characterId) && s.RelationType == relationType);
            if (chara != null)
            {
                long id = chara.CharacterRelationId;
                CharacterDTO charac = DAOFactory.CharacterDAO.LoadById(characterId);
                DAOFactory.CharacterRelationDAO.Delete(id);
                ServerManager.Instance.RelationRefresh(id);

                if (charac != null)
                {
                    List<CharacterRelationDTO> lst = ServerManager.Instance.CharacterRelations.Where(s => s.CharacterId == characterId || s.RelatedCharacterId == characterId).ToList();
                    string result = "finit";

                    foreach (CharacterRelationDTO relation in lst.Where(c => c.RelationType == CharacterRelationType.Friend || c.RelationType == CharacterRelationType.Spouse))
                    {
                        long id2 = relation.RelatedCharacterId == charac.CharacterId ? relation.CharacterId : relation.RelatedCharacterId;
                        bool isOnline = CommunicationServiceClient.Instance.IsCharacterConnected(ServerManager.Instance.ServerGroup, id2);
                        result += $" {id2}|{(short)relation.RelationType}|{(isOnline ? 1 : 0)}|{DAOFactory.CharacterDAO.LoadById(id2).Name}";
                    }

                    int? sentChannelId = CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                    {
                        DestinationCharacterId = charac.CharacterId,
                        SourceCharacterId = mainCharacterId,
                        SourceWorldId = ServerManager.Instance.WorldId,
                        Message = result,
                        Type = MessageType.PrivateChat
                    });
                }
            }
        }

        #endregion
    }
}