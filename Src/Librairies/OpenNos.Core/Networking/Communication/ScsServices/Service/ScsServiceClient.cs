﻿using System;
using System.Runtime.Remoting.Proxies;
using OpenNos.Core.Networking.Communication.Scs.Communication;
using OpenNos.Core.Networking.Communication.Scs.Communication.EndPoints;
using OpenNos.Core.Networking.Communication.Scs.Communication.Messengers;
using OpenNos.Core.Networking.Communication.Scs.Server;
using OpenNos.Core.Networking.Communication.ScsServices.Communication;

namespace OpenNos.Core.Networking.Communication.ScsServices.Service
{
    /// <summary>
    ///     Implements IScsServiceClient. It is used to manage and monitor a service client.
    /// </summary>
    public class ScsServiceClient : IScsServiceClient
    {
        #region Instantiation

        /// <summary>
        ///     Creates a new ScsServiceClient object.
        /// </summary>
        /// <param name="serverClient">Reference to underlying IScsServerClient object</param>
        /// <param name="requestReplyMessenger">RequestReplyMessenger to send messages</param>
        public ScsServiceClient(IScsServerClient serverClient,
            RequestReplyMessenger<IScsServerClient> requestReplyMessenger)
        {
            _serverClient = serverClient;
            _serverClient.Disconnected += Client_Disconnected;
            _requestReplyMessenger = requestReplyMessenger;
        }

        #endregion

        #region Events

        /// <summary>
        ///     This event is raised when this client is disconnected from server.
        /// </summary>
        public event EventHandler Disconnected;

        #endregion

        #region Members

        /// <summary>
        ///     This object is used to send messages to client.
        /// </summary>
        private readonly RequestReplyMessenger<IScsServerClient> _requestReplyMessenger;

        /// <summary>
        ///     Reference to underlying IScsServerClient object.
        /// </summary>
        private readonly IScsServerClient _serverClient;

        /// <summary>
        ///     Last created proxy object to invoke remote medhods.
        /// </summary>
        private RealProxy _realProxy;

        #endregion

        #region Properties

        /// <summary>
        ///     Unique identifier for this client.
        /// </summary>
        public long ClientId => _serverClient.ClientId;

        /// <summary>
        ///     Gets the communication state of the Client.
        /// </summary>
        public CommunicationStates CommunicationState => _serverClient.CommunicationState;

        /// <summary>
        ///     Gets endpoint of remote application.
        /// </summary>
        public ScsEndPoint RemoteEndPoint => _serverClient.RemoteEndPoint;

        #endregion

        #region Methods

        /// <summary>
        ///     Closes client connection.
        /// </summary>
        public void Disconnect()
        {
            _serverClient.Disconnect();
        }

        /// <summary>
        ///     Gets the client proxy interface that provides calling client methods remotely.
        /// </summary>
        /// <typeparam name="T">Type of client interface</typeparam>
        /// <returns>Client interface</returns>
        public T GetClientProxy<T>() where T : class
        {
            _realProxy = new RemoteInvokeProxy<T, IScsServerClient>(_requestReplyMessenger);
            return (T) _realProxy.GetTransparentProxy();
        }

        /// <summary>
        ///     Handles disconnect event of _serverClient object.
        /// </summary>
        /// <param name="sender">Source of event</param>
        /// <param name="e">Event arguments</param>
        private void Client_Disconnected(object sender, EventArgs e)
        {
            _requestReplyMessenger.Stop();
            OnDisconnected();
        }

        /// <summary>
        ///     Raises Disconnected event.
        /// </summary>
        private void OnDisconnected()
        {
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}