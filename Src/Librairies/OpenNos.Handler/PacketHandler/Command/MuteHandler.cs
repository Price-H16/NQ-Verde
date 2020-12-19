﻿using System;
using NosTale.Extension.Extension.Command;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class MuteHandler : IPacketHandler
    {
        #region Instantiation

        public MuteHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Mute(MutePacket mutePacket)
        {
            if (mutePacket != null)
            {
                Session.AddLogsCmd(mutePacket);
                if (mutePacket.Duration == 0) mutePacket.Duration = 60;

                mutePacket.Reason = mutePacket.Reason?.Trim();
                Session.MuteMethod(mutePacket.CharacterName, mutePacket.Reason, mutePacket.Duration);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(MutePacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}