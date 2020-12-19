using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using System.Linq;

namespace OpenNos.Handler.PacketHandler.Inventory
{
    public class MviPacketHandler : IPacketHandler
    {
        #region Instantiation

        public MviPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void MoveItem(MviPacket mviPacket)
        {
            if (!Session.Character.VerifiedLock)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CHARACTER_LOCKED_USE_UNLOCK"), 0));
                return;
            }

            if (mviPacket != null)
            {
                if (mviPacket.Amount <= 0 || mviPacket.InventoryType == InventoryType.Bazaar || mviPacket.InventoryType == InventoryType.Wear || mviPacket.InventoryType == InventoryType.FamilyWareHouse ||mviPacket.InventoryType == InventoryType.Warehouse ||mviPacket.Slot == mviPacket.DestinationSlot)
                {
                    return;
                }
                if (mviPacket.InventoryType >= (InventoryType)13 && mviPacket.InventoryType <= (InventoryType)24 && Session.Character.Mates.Count(s => s.MateType == MateType.Partner) < (byte)(mviPacket.InventoryType - 12))
                {
                    return;
                }
                if (mviPacket.InventoryType != InventoryType.Equipment && mviPacket.InventoryType != InventoryType.Main && mviPacket.InventoryType != InventoryType.Etc && mviPacket.InventoryType != InventoryType.Miniland)
                {
                    return;
                }

                if (mviPacket.Amount < 1)
                {
                    return;
                }

                if (mviPacket.Slot == mviPacket.DestinationSlot)
                {
                    return;
                }

                if (mviPacket.InventoryType == InventoryType.Wear)
                {
                    return;
                }


                lock (Session.Character.Inventory)
                {
                    // check if the destination slot is out of range
                    if (mviPacket.DestinationSlot > 48 + ((Session.Character.HaveBackpack() ? 1 : 0) * 12) + ((Session.Character.HaveExtension() ? 1 : 0) * 60))
                    {
                        return;
                    }

                    // check if the character is allowed to move the item
                    if (Session.Character.InExchangeOrTrade)
                    {
                        return;
                    }

                    // actually move the item from source to destination
                    Session.Character.Inventory.MoveItem(mviPacket.InventoryType, mviPacket.InventoryType, mviPacket.Slot, mviPacket.Amount, mviPacket.DestinationSlot, out var previousInventory, out var newInventory);
                    if (newInventory == null)
                    {
                        return;
                    }
                    Session.SendPacket(newInventory.GenerateInventoryAdd());

                    Session.SendPacket(previousInventory != null ? previousInventory.GenerateInventoryAdd() : UserInterfaceHelper.Instance.GenerateInventoryRemove(mviPacket.InventoryType, mviPacket.Slot));
                }
            }
        }

        #endregion
    }
}