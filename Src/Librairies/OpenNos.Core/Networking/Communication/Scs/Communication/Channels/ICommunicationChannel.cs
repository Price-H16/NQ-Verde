﻿using System;
using OpenNos.Core.Networking.Communication.Scs.Communication.EndPoints;
using OpenNos.Core.Networking.Communication.Scs.Communication.Messengers;

namespace OpenNos.Core.Networking.Communication.Scs.Communication.Channels
{
    /// <summary>
    ///     Represents a communication channel. A communication channel is used to communicate
    ///     (send/receive messages) with a remote application.
    /// </summary>
    public interface ICommunicationChannel : IMessenger
    {
        #region Events

        /// <summary>
        ///     This event is raised when client disconnected from server.
        /// </summary>
        event EventHandler Disconnected;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the current communication state.
        /// </summary>
        CommunicationStates CommunicationState { get; }

        /// <summary>
        ///     Gets endpoint of remote application.
        /// </summary>
        ScsEndPoint RemoteEndPoint { get; }

        #endregion

        #region Methods

        /// <summary>
        ///     Closes messenger.
        /// </summary>
        void Disconnect();

        /// <summary>
        ///     Starts the communication with remote application.
        /// </summary>
        void Start();

        #endregion
    }
}