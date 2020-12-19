﻿using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class HairStyleHandler : IPacketHandler
    {
        #region Instantiation

        public HairStyleHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Hairstyle(HairStylePacket hairStylePacket)
        {
            if (hairStylePacket != null)
            {
                Session.AddLogsCmd(hairStylePacket);
                Session.Character.HairStyle = hairStylePacket.HairStyle;
                Session.SendPacket(Session.Character.GenerateEq());
                Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateIn());
                Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateGidx());
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(HairStylePacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}