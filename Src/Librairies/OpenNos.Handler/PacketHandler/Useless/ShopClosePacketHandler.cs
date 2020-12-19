﻿using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.GameObject;

namespace OpenNos.Handler.PacketHandler.Useless
{
    public class ShopClosePacketHandler : IPacketHandler
    {
        #region Instantiation

        public ShopClosePacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void ShopClose(ShopClosePacket shopClosePacket)
        {
            // Not needed for now.
        }

        #endregion
    }
}