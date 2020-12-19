using System.Collections.Generic;
using System.Threading;
using NosTale.Configuration;
using NosTale.Configuration.Utilities;
using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Master.Library.Data;
using OpenNos.Master.Library.Interface;
using OpenNos.SCS.Communication.Scs.Communication;
using OpenNos.SCS.Communication.Scs.Communication.EndPoints.Tcp;
using OpenNos.SCS.Communication.ScsServices.Client;

namespace OpenNos.Master.Library.Client
{
    public class MallServiceClient : IMallService
    {
        #region Instantiation

        public MallServiceClient()
        {
            var a = DependencyContainer.Instance.GetInstance<JsonGameConfiguration>().Server;
            var ip = a.MasterIP;
            var port = a.MasterPort;
            _client = ScsServiceClientBuilder.CreateClient<IMallService>(new ScsTcpEndPoint(ip, port));
            Thread.Sleep(5000);
            while (_client.CommunicationState != CommunicationStates.Connected)
                try
                {
                    _client.Connect();
                }
                catch
                {
                    Logger.Error(Language.Instance.GetMessageFromKey("RETRY_CONNECTION"),
                        memberName: "MallServiceClient");
                    Thread.Sleep(1000);
                }
        }

        #endregion

        #region Members

        private static MallServiceClient _instance;

        private readonly IScsServiceClient<IMallService> _client;

        #endregion

        #region Properties

        public static MallServiceClient Instance => _instance ?? (_instance = new MallServiceClient());

        public CommunicationStates CommunicationState => _client.CommunicationState;

        #endregion

        #region Methods

        public bool Authenticate(string authKey)
        {
            return _client.ServiceProxy.Authenticate(authKey);
        }

        public IEnumerable<CharacterDTO> GetCharacters(long accountId)
        {
            return _client.ServiceProxy.GetCharacters(accountId);
        }

        public void SendItem(long characterId, MallItem item)
        {
            _client.ServiceProxy.SendItem(characterId, item);
        }

        public void SendStaticBonus(long characterId, MallStaticBonus item)
        {
            _client.ServiceProxy.SendStaticBonus(characterId, item);
        }

        public AccountDTO ValidateAccount(string userName, string passHash)
        {
            return _client.ServiceProxy.ValidateAccount(userName, passHash);
        }

        #endregion
    }
}