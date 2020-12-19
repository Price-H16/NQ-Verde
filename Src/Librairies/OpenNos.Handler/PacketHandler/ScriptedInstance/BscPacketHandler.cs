﻿using System.Linq;
using NosTale.Packets.Packets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.ScriptedInstance
{
    public class BscPacketHandler : IPacketHandler
    {
        #region Instantiation

        public BscPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void ButtonCancel(BscPacket packet)
        {
            switch (packet.Type)
            {
                case 2:
                    var arenamember = ServerManager.Instance.ArenaMembers.ToList()
                        .FirstOrDefault(s => s.Session == Session);
                    if (arenamember?.GroupId != null)
                        if (packet.Option != 1)
                        {
                            Session.SendPacket(
                                $"qna #bsc^2^1 {Language.Instance.GetMessageFromKey("ARENA_PENALTY_NOTICE")}");
                            return;
                        }

                    Session.Character.LeaveTalentArena();
                    break;
            }
        }

        #endregion
    }
}