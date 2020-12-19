using System;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class CloneItemHandler : IPacketHandler
    {
        #region Instantiation

        public CloneItemHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void CloneItem(CloneItemPacket cloneItemPacket)
        {
            if (cloneItemPacket != null)
            {
                Session.AddLogsCmd(cloneItemPacket);
                var item =
                    Session.Character.Inventory.LoadBySlotAndType(cloneItemPacket.Slot, InventoryType.Equipment);
                if (item != null)
                {
                    item = item.DeepCopy();
                    item.Id = Guid.NewGuid();
                    Session.Character.Inventory.AddToInventory(item);
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(CloneItemPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}