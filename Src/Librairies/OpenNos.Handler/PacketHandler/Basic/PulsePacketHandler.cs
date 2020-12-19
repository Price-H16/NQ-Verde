using System;
using System.Diagnostics;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.Master.Library.Client;

namespace OpenNos.Handler.PacketHandler.Basic
{
    public class PulsePacketHandler : IPacketHandler
    {
        #region Instantiation

        public PulsePacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Pulse(PulsePacket pulsepacket)
        {
            if ((Session.Character.LastPulse.AddMilliseconds(80000) >= DateTime.Now
                && DateTime.Now >= Session.Character.LastPulse.AddMilliseconds(40000)) || Debugger.IsAttached)
                Session.Character.LastPulse = DateTime.Now;
            else
                Session.Disconnect();

            Session.Character.MuteMessage();
            Session.Character.DeleteTimeout();
            CommunicationServiceClient.Instance.PulseAccount(Session.Account.AccountId);
        }

        #endregion
    }
}