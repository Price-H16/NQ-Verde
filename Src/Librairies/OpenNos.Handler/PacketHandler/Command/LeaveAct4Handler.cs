﻿using System;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.Master.Library.Client;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class LeaveAct4Handler : IPacketHandler
    {
        #region Instantiation

        public LeaveAct4Handler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void LeaveAct4(LeaveAct4Packet leaveAct4Packet)
        {
            if (leaveAct4Packet != null)
            {
                Session.AddLogsCmd(leaveAct4Packet);
                if (Session.Character.Channel.ChannelId == 51)
                {
                    var connection =
                        CommunicationServiceClient.Instance.RetrieveOriginWorld(Session.Character.AccountId);
                    if (string.IsNullOrWhiteSpace(connection)) return;
                    Session.Character.MapId = 145;
                    Session.Character.MapX = 51;
                    Session.Character.MapY = 41;
                    var port = Convert.ToInt32(connection.Split(':')[1]);
                    Session.Character.ChangeChannel(connection.Split(':')[0], port, 3);
                }
            }

            Session.Character.GenerateSay(LeaveAct4Packet.ReturnHelp(), 10);
        }

        #endregion
    }
}