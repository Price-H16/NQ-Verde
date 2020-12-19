﻿using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Extension.Inventory;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class RarifyHandler : IPacketHandler
    {
        #region Instantiation

        public RarifyHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Rarify(RarifyPacket rarifyPacket)
        {
            if (rarifyPacket != null)
            {
                Session.AddLogsCmd(rarifyPacket);
                if (rarifyPacket.Slot >= 0)
                {
                    var wearableInstance = Session.Character.Inventory.LoadBySlotAndType(rarifyPacket.Slot, 0);
                    wearableInstance?.RarifyItem(Session, rarifyPacket.Mode, rarifyPacket.Protection);
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(RarifyPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}