﻿using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;


namespace OpenNos.Handler.PacketHandler.Basic
{
    public class SitPacketHandler : IPacketHandler
    {
        #region Instantiation

        public SitPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Rest(SitPacket sitpacket)
        {
            if (Session.Character.MeditationDictionary.Count != 0) Session.Character.MeditationDictionary.Clear();

            sitpacket.Users?.ForEach(u =>
            {
                if (u.UserType == 1)
                    Session.Character.Rest();
                else
                    Session.CurrentMapInstance.Broadcast(Session.Character.Mates
                        .Find(s => s.MateTransportId == (int) u.UserId)?.GenerateRest(sitpacket.Users[0] != u));
            });
        }

        #endregion
    }
}