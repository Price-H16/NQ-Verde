﻿using System;

namespace OpenNos.Core.Networking.Communication.Scs.Communication.Channels
{
    /// <summary>
    ///     This class provides base functionality for communication listener Classs.
    /// </summary>
    public abstract class ConnectionListenerBase : IConnectionListener
    {
        #region Events

        /// <summary>
        ///     This event is raised when a new communication channel is connected.
        /// </summary>
        public event EventHandler<CommunicationChannelEventArgs> CommunicationChannelConnected;

        #endregion

        #region Methods

        /// <summary>
        ///     Starts listening incoming connections.
        /// </summary>
        public abstract void Start();

        /// <summary>
        ///     Stops listening incoming connections.
        /// </summary>
        public abstract void Stop();

        /// <summary>
        ///     Raises CommunicationChannelConnected event.
        /// </summary>
        /// <param name="client"></param>
        protected virtual void OnCommunicationChannelConnected(ICommunicationChannel client)
        {
            CommunicationChannelConnected?.Invoke(this, new CommunicationChannelEventArgs(client));
        }

        #endregion
    }
}