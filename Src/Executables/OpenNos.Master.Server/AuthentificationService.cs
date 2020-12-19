﻿using NosTale.Configuration;
using NosTale.Configuration.Utilities;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Master.Library.Interface;
using OpenNos.SCS.Communication.ScsServices.Service;

namespace OpenNos.Master.Server
{
    internal class AuthentificationService : ScsService, IAuthentificationService
    {
        #region Methods

        public bool Authenticate(string authKey)
        {
            if (string.IsNullOrWhiteSpace(authKey)) return false;

            var a = DependencyContainer.Instance.GetInstance<JsonGameConfiguration>().Server;

            if (authKey == a.AuthentificationServiceAuthKey)
            {
                MSManager.Instance.AuthentificatedClients.Add(CurrentClient.ClientId);
                return true;
            }

            return false;
        }

        public AccountDTO ValidateAccount(string userName, string passHash)
        {
            if ( /*!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)) || */
                string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passHash)) return null;

            var account = DAOFactory.AccountDAO.LoadByName(userName);

            if (account?.Password == passHash) return account;
            return null;
        }

        public CharacterDTO ValidateAccountAndCharacter(string userName, string characterName, string passHash)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)) ||
                string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(characterName) ||
                string.IsNullOrEmpty(passHash)) return null;

            var account = DAOFactory.AccountDAO.LoadByName(userName);

            if (account?.Password == passHash)
            {
                var character = DAOFactory.CharacterDAO.LoadByName(characterName);
                if (character?.AccountId == account.AccountId) return character;
                return null;
            }

            return null;
        }

        #endregion
    }
}