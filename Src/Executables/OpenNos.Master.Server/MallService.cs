using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ChickenAPI.Enums;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.Master.Library.Data;
using OpenNos.Master.Library.Interface;
using OpenNos.SCS.Communication.ScsServices.Service;

namespace OpenNos.Master.Server
{
    internal class MallService : ScsService, IMallService
    {
        #region Methods

        public bool Authenticate(string authKey)
        {
            if (string.IsNullOrWhiteSpace(authKey)) return false;

            if (authKey == ConfigurationManager.AppSettings["MasterAuthKey"])
            {
                MSManager.Instance.AuthentificatedClients.Add(CurrentClient.ClientId);
                return true;
            }

            return false;
        }

        public AuthorityType GetAuthority(long accountId)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)))
                return AuthorityType.Closed;

            return DAOFactory.AccountDAO.LoadById(accountId)?.Authority ?? AuthorityType.Closed;
        }

        public IEnumerable<CharacterDTO> GetCharacters(long accountId)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId))) return null;

            return DAOFactory.CharacterDAO.LoadByAccount(accountId);
        }

        public bool IsAuthenticated()
        {
            return MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId));
        }

        public void SendItem(long characterId, MallItem item)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId))) return;
            var dto = DAOFactory.ItemDAO.LoadById(item.ItemVNum);
            if (dto != null)
            {
                var limit = 999;

                if (dto.Type == InventoryType.Equipment || dto.Type == InventoryType.Miniland) limit = 1;

                do
                {
                    var mailDTO = new MailDTO
                    {
                        AttachmentAmount = (short) (item.Amount > limit ? limit : item.Amount),
                        AttachmentRarity = item.Rare,
                        AttachmentUpgrade = item.Upgrade,
                        AttachmentVNum = item.ItemVNum,
                        Date = DateTime.Now,
                        EqPacket = string.Empty,
                        IsOpened = false,
                        IsSenderCopy = false,
                        Message = string.Empty,
                        ReceiverId = characterId,
                        SenderId = characterId,
                        Title = "NOSMALL",
                        AttachmentLevel = item.Level == 0 ? (byte) 1 : item.Level
                    };

                    DAOFactory.MailDAO.InsertOrUpdate(ref mailDTO);

                    var account =
                        MSManager.Instance.ConnectedAccounts.Find(a => a.CharacterId.Equals(mailDTO.ReceiverId));
                    if (account?.ConnectedWorld != null)
                        account.ConnectedWorld.MailServiceClient.GetClientProxy<IMailClient>().MailSent(mailDTO);

                    item.Amount -= limit;
                } while (item.Amount > 0);
            }
        }

        public void SendStaticBonus(long characterId, MallStaticBonus item)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId))) return;
            var dto = DAOFactory.StaticBonusDAO.LoadByCharacterId(characterId)
                .FirstOrDefault(s => s.StaticBonusType == item.StaticBonus);

            if (dto != null)
                dto.DateEnd.AddSeconds(item.Seconds);
            else
                dto = new StaticBonusDTO
                {
                    CharacterId = characterId,
                    DateEnd = DateTime.Now.AddSeconds(item.Seconds),
                    StaticBonusType = item.StaticBonus
                };

            DAOFactory.StaticBonusDAO.InsertOrUpdate(ref dto);
            var account = MSManager.Instance.ConnectedAccounts.Find(a => a.CharacterId.Equals(characterId));
            if (account?.ConnectedWorld != null)
                account.ConnectedWorld.CommunicationServiceClient.GetClientProxy<ICommunicationClient>()
                    .UpdateStaticBonus(characterId);
        }

        public AccountDTO ValidateAccount(string userName, string passHash)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)) ||
                string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passHash)) return null;

            var account = DAOFactory.AccountDAO.LoadByName(userName);

            if (account?.Password == passHash) return account;
            return null;
        }

        #endregion
    }
}