using System;
using NosTale.Configuration;
using NosTale.Configuration.Utilities;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Master.Library.Interface;
using OpenNos.SCS.Communication.ScsServices.Service;

namespace OpenNos.Master.Server
{
    internal class MailService : ScsService, IMailService
    {
        #region Methods

        public bool Authenticate(string authKey, Guid serverId)
        {
            if (string.IsNullOrWhiteSpace(authKey))
            {
                return false;
            }
            var a = DependencyContainer.Instance.GetInstance<JsonGameConfiguration>().Server;

            if (authKey == a.MasterAuthKey)
            {
                MSManager.Instance.AuthentificatedClients.Add(CurrentClient.ClientId);

                var ws = MSManager.Instance.WorldServers.Find(s => s.Id == serverId);
                if (ws != null)
                {
                    ws.MailServiceClient = CurrentClient;
                }
                return true;
            }

            return false;
        }

        public void SendMail(MailDTO mail)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId))) return;

            DAOFactory.MailDAO.InsertOrUpdate(ref mail);

            if (mail.IsSenderCopy)
            {
                var account = MSManager.Instance.ConnectedAccounts.Find(a => a.CharacterId.Equals(mail.SenderId));
                if (account?.ConnectedWorld != null)
                {
                    account.ConnectedWorld.MailServiceClient.GetClientProxy<IMailClient>().MailSent(mail);
                }
                   
            }
            else
            {
                var account = MSManager.Instance.ConnectedAccounts.Find(a => a.CharacterId.Equals(mail.ReceiverId));
                if (account?.ConnectedWorld != null)
                {
                    account.ConnectedWorld.MailServiceClient.GetClientProxy<IMailClient>().MailSent(mail);
                }
                    
            }
        }

        #endregion
    }
}