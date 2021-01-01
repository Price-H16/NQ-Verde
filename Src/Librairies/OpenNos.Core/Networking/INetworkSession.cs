using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace OpenNos.Core.Networking
{
    public interface INetworkSession
    {
        #region Events

        event EventHandler<string> PacketReceived;

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        long ClientId { get; }

        /// <summary>
        /// </summary>
        IPAddress IpAddress { get; }

        bool IsConnected { get; }

        bool IsDisposing { get; set; }

        // todo urgent rework
        int SessionId { get; set; }

        #endregion

        #region Methods

        void DisconnectClient();

        void SendPacket(string packet);

        Task SendPacketAsync(string packet);

        Task SendPacketAsyncFormat(string packet, params object[] param);

        void SendPacketFormat(string packet, params object[] param);

        void SendPackets(IEnumerable<string> packets);

        Task SendPacketsAsync(IEnumerable<string> packets);

        #endregion
    }
}