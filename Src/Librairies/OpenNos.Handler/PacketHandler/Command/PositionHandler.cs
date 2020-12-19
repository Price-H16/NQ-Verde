﻿using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class PositionHandler : IPacketHandler
    {
        #region Instantiation

        public PositionHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Position(PositionPacket positionPacket)
        {
            Session.AddLogsCmd(positionPacket);
            Session.SendPacket(Session.Character.GenerateSay(
                $"Map:{Session.Character.MapInstance.Map.MapId} - X:{Session.Character.PositionX} - Y:{Session.Character.PositionY} - Dir:{Session.Character.Direction} - Cell:{Session.CurrentMapInstance.Map.JaggedGrid[Session.Character.PositionX][Session.Character.PositionY]?.Value}",
                12));
        }

        #endregion
    }
}