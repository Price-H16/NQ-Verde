﻿using System;

namespace OpenNos.Core.Networking.Communication.Scs.Communication.Messages
{
    /// <summary>
    ///     This message is used to send/receive ping messages. Ping messages is used to keep connection
    ///     alive between server and client.
    /// </summary>
    [Serializable]
    public sealed class ScsPingMessage : ScsMessage
    {
        #region Methods

        /// <summary>
        ///     Creates a string to represents this object.
        /// </summary>
        /// <returns>A string to represents this object</returns>
        public override string ToString() => string.IsNullOrEmpty(RepliedMessageId)
                ? $"ScsPingMessage [{MessageId}]"
                : $"ScsPingMessage [{MessageId}] Replied To [{RepliedMessageId}]";

        #endregion

        #region Instantiation

        /// <summary>
        ///     Creates a new PingMessage object.
        /// </summary>
        public ScsPingMessage()
        {
        }

        /// <summary>
        ///     Creates a new reply PingMessage object.
        /// </summary>
        /// <param name="repliedMessageId">Replied message id if this is a reply for a message.</param>
        public ScsPingMessage(string repliedMessageId)
            : this() => RepliedMessageId = repliedMessageId;

        #endregion
    }
}