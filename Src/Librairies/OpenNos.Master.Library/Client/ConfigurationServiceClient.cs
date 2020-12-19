using System;
using System.Threading;
using NosTale.Configuration;
using NosTale.Configuration.Utilities;
using OpenNos.Core;
using OpenNos.Master.Library.Data;
using OpenNos.Master.Library.Interface;
using OpenNos.SCS.Communication.Scs.Communication;
using OpenNos.SCS.Communication.Scs.Communication.EndPoints.Tcp;
using OpenNos.SCS.Communication.ScsServices.Client;

namespace OpenNos.Master.Library.Client
{
    public class ConfigurationServiceClient : IConfigurationService
    {
        #region Instantiation

        public ConfigurationServiceClient()
        {
            var a = DependencyContainer.Instance.GetInstance<JsonGameConfiguration>().Server;
            var ip = a.MasterIP;
            var port = a.MasterPort;
            _confClient = new ConfigurationClient();
            _client = ScsServiceClientBuilder.CreateClient<IConfigurationService>(new ScsTcpEndPoint(ip, port),
                _confClient);
            Thread.Sleep(1000);
            while (_client.CommunicationState != CommunicationStates.Connected)
                try
                {
                    _client.Connect();
                }
                catch (Exception)
                {
                    Logger.Error(Language.Instance.GetMessageFromKey("RETRY_CONNECTION"),
                        memberName: nameof(CommunicationServiceClient));
                    Thread.Sleep(1000);
                }
        }

        #endregion

        #region Events

        public event EventHandler ConfigurationUpdate;

        #endregion

        #region Members

        private static ConfigurationServiceClient _instance;

        private readonly IScsServiceClient<IConfigurationService> _client;

        private readonly ConfigurationClient _confClient;

        #endregion

        #region Properties

        public static ConfigurationServiceClient Instance =>
            _instance ?? (_instance = new ConfigurationServiceClient());

        public CommunicationStates CommunicationState => _client.CommunicationState;

        #endregion

        #region Methods

        public bool Authenticate(string authKey, Guid serverId)
        {
            return _client.ServiceProxy.Authenticate(authKey, serverId);
        }

        public ConfigurationObject GetConfigurationObject()
        {
            return _client.ServiceProxy.GetConfigurationObject();
        }

        public void UpdateConfigurationObject(ConfigurationObject configurationObject)
        {
            _client.ServiceProxy.UpdateConfigurationObject(configurationObject);
        }

        internal void OnConfigurationUpdated(ConfigurationObject configurationObject)
        {
            ConfigurationUpdate?.Invoke(configurationObject, null);
        }

        #endregion
    }
}