using System;
using NosTale.Configuration;
using NosTale.Configuration.Utilities;
using OpenNos.Master.Library.Data;
using OpenNos.Master.Library.Interface;
using OpenNos.SCS.Communication.ScsServices.Service;

namespace OpenNos.Master.Server
{
    internal class ConfigurationService : ScsService, IConfigurationService
    {
        #region Methods

        public bool Authenticate(string authKey, Guid serverId)
        {
            if (string.IsNullOrWhiteSpace(authKey)) return false;

            var a = DependencyContainer.Instance.GetInstance<JsonGameConfiguration>().Server;

            if (authKey == a.MasterAuthKey)
            {
                MSManager.Instance.AuthentificatedClients.Add(CurrentClient.ClientId);

                var ws = MSManager.Instance.WorldServers.Find(s => s.Id == serverId);
                if (ws != null) ws.ConfigurationServiceClient = CurrentClient;
                return true;
            }

            return false;
        }

        public ConfigurationObject GetConfigurationObject()
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId))) return null;
            return MSManager.Instance.ConfigurationObject;
        }

        public void UpdateConfigurationObject(ConfigurationObject configurationObject)
        {
            if (!MSManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId))) return;
            MSManager.Instance.ConfigurationObject = configurationObject;

            foreach (var ws in MSManager.Instance.WorldServers)
                ws.ConfigurationServiceClient.GetClientProxy<IConfigurationClient>()
                    .ConfigurationUpdated(MSManager.Instance.ConfigurationObject);
        }

        #endregion
    }
}