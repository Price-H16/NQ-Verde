﻿using System;

namespace OpenNos.Core.Networking.Communication.Scs.Communication.Messages
{
    /// <summary>
    ///     Represents a message that is sent and received by server and client. This is the base class
    ///     for all messages.
    /// </summary>
    [Serializable]
    public class ScsMessage : IScsMessage
    {
        #region Methods

        /// <summary>
        ///     Creates a string to represents this object.
        /// </summary>
        /// <returns>A string to represents this object</returns>
        public override string ToString() => string.IsNullOrEmpty(RepliedMessageId)
                ? $"ScsMessage [{MessageId}]"
                : $"ScsMessage [{MessageId}] Replied To [{RepliedMessageId}]";

        #endregion

        #region Instantiation

        /// <summary>
        ///     Creates a new ScsMessage.
        /// </summary>
        public ScsMessage() => MessageId = Guid.NewGuid().ToString();

        /// <summary>
        ///     Creates a new reply ScsMessage.
        /// </summary>
        /// <param name="repliedMessageId">Replied message id if this is a reply for a message.</param>
        public ScsMessage(string repliedMessageId) : this() => RepliedMessageId = repliedMessageId;

        #endregion

        #region Properties

        /// <summary>
        ///     Unique identified for this message. Default value: New GUID. Do not change if you do not
        ///     want to do low level changes such as custom wire protocols.
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        ///     This property is used to indicate that this is a Reply message to a message. It may be
        ///     null if this is not a reply message.
        /// </summary>
        public string RepliedMessageId { get; set; }

        #endregion
    }
}