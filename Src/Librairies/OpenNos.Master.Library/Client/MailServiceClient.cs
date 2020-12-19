﻿using System;
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
    public class MailServiceClient : IMailService
    {
        #region Instantiation

        public MailServiceClient()
        {
            var a = DependencyContainer.Instance.GetInstance<JsonGameConfiguration>().Server;
            var ip = a.MasterIP;
            var port = a.MasterPort;
            _mailClient = new MailClient();
            _client = ScsServiceClientBuilder.CreateClient<IMailService>(new ScsTcpEndPoint(ip, port), _mailClient);

            Thread.Sleep(1000);
            while (_client.CommunicationState != CommunicationStates.Connected)
                try
                {
                    _client.Connect();
                }
                catch (Exception)
                {
                    Logger.Error(Language.Instance.GetMessageFromKey("RETRY_CONNECTION"),
                        memberName: nameof(MailServiceClient));
                    Thread.Sleep(1000);
                }
        }

        #endregion

        #region Events

        public event EventHandler MailSent;

        #endregion

        #region Members

        private static MailServiceClient _instance;

        private readonly IScsServiceClient<IMailService> _client;

        private readonly MailClient _mailClient;

        #endregion

        #region Properties

        public static MailServiceClient Instance => _instance ?? (_instance = new MailServiceClient());

        public CommunicationStates CommunicationState => _client.CommunicationState;

        #endregion

        #region Methods

        public bool Authenticate(string authKey, Guid serverId)
        {
            return _client.ServiceProxy.Authenticate(authKey, serverId);
        }

        public void SendMail(MailDTO mail)
        {
            _client.ServiceProxy.SendMail(mail);
        }

        internal void OnMailSent(MailDTO mail)
        {
            MailSent?.Invoke(mail, null);
        }

        #endregion
    }
}