using System;
using System.Threading;
using NosTale.Configuration;
using NosTale.Configuration.Utilities;
using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Master.Library.Interface;
using OpenNos.SCS.Communication.Scs.Communication;
using OpenNos.SCS.Communication.Scs.Communication.EndPoints.Tcp;
using OpenNos.SCS.Communication.ScsServices.Client;

namespace OpenNos.Master.Library.Client
{
    public class AuthentificationServiceClient : IAuthentificationService
    {
        #region Instantiation

        public AuthentificationServiceClient()
        {
            var a = DependencyContainer.Instance.GetInstance<JsonGameConfiguration>().Server;
            var ip = a.MasterIP;
            var port = a.MasterPort;
            _client = ScsServiceClientBuilder.CreateClient<IAuthentificationService>(new ScsTcpEndPoint(ip, port));
            Thread.Sleep(1000);
            while (_client.CommunicationState != CommunicationStates.Connected)
                try
                {
                    _client.Connect();
                }
                catch (Exception)
                {
                    Logger.Error(Language.Instance.GetMessageFromKey("RETRY_CONNECTION"),
                        memberName: nameof(AuthentificationServiceClient));
                    Thread.Sleep(1000);
                }
        }

        #endregion

        #region Members

        private static AuthentificationServiceClient _instance;

        private readonly IScsServiceClient<IAuthentificationService> _client;

        #endregion

        #region Properties

        public static AuthentificationServiceClient Instance =>
            _instance ?? (_instance = new AuthentificationServiceClient());

        public CommunicationStates CommunicationState => _client.CommunicationState;

        #endregion

        #region Methods

        public bool Authenticate(string authKey)
        {
            return _client.ServiceProxy.Authenticate(authKey);
        }

        public AccountDTO ValidateAccount(string userName, string passHash)
        {
            return _client.ServiceProxy.ValidateAccount(userName, passHash);
        }

        public CharacterDTO ValidateAccountAndCharacter(string userName, string characterName, string passHash)
        {
            return _client.ServiceProxy.ValidateAccountAndCharacter(userName, characterName, passHash);
        }

        #endregion
    }
}