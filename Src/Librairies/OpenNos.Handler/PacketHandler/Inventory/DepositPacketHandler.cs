using System.Linq;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Inventory
{
    public class DepositPacketHandler : IPacketHandler
    {
        #region Instantiation

        public DepositPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Deposit(DepositPacket depositPacket)
        {
            if (depositPacket == null)
            {
                return;
            }

            if (depositPacket.Inventory == InventoryType.Bazaar
                || depositPacket.Inventory == InventoryType.FamilyWareHouse
                || depositPacket.Inventory == InventoryType.Miniland
                || depositPacket.Inventory == InventoryType.Wear
                || depositPacket.Inventory == InventoryType.PetWarehouse
                || depositPacket.Inventory == InventoryType.Warehouse
                || depositPacket.Inventory == InventoryType.FirstPartnerInventory
                || depositPacket.Inventory == InventoryType.SecondPartnerInventory
                || depositPacket.Inventory == InventoryType.ThirdPartnerInventory
                || depositPacket.Inventory == InventoryType.FourthPartnerInventory
                || depositPacket.Inventory == InventoryType.FifthPartnerInventory
                || depositPacket.Inventory == InventoryType.SixthPartnerInventory
                || depositPacket.Inventory == InventoryType.SeventhPartnerInventory
                || depositPacket.Inventory == InventoryType.EighthPartnerInventory
                || depositPacket.Inventory == InventoryType.NinthPartnerInventory
                || depositPacket.Inventory == InventoryType.TenthPartnerInventory
                )
            {
                ServerManager.Instance.Kick(Session.Character.Name);
                Logger.LogUserEvent("WAREHOUSE_CHEAT_TRY", Session.GenerateIdentity(), $"Packet string: {depositPacket.OriginalContent.ToString()}");
                return;
            }

            ItemInstance originalItem = Session.Character.Inventory.LoadBySlotAndType(depositPacket.Slot, depositPacket.Inventory);

            if (originalItem == null)
            {
                return;
            }

            if (originalItem.Item.ItemType == ItemType.Quest1 || originalItem.Item.ItemType == ItemType.Quest2)
            {
                return;
            }

            if (depositPacket.Amount > originalItem.Amount)
            {
                return;
            }

            ItemInstance anotherItem = Session.Character.Inventory.LoadBySlotAndType(depositPacket.NewSlot, depositPacket.PartnerBackpack ? InventoryType.PetWarehouse : InventoryType.Warehouse);

            if (anotherItem != null)
            {
                return;
            }

            // check if the destination slot is out of range
            if (depositPacket.NewSlot >= (depositPacket.PartnerBackpack ? 
                Session.Character.StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.PetBackPack) ? 50 : 0 : Session.Character.WareHouseSize))
            {
                return;
            }

            // check if the character is allowed to move the item
            if (Session.Character.InExchangeOrTrade)
            {
                return;
            }

            ItemInstance item = Session.Character.Inventory.LoadBySlotAndType(depositPacket.Slot, depositPacket.Inventory);
            ItemInstance itemdest = Session.Character.Inventory.LoadBySlotAndType(depositPacket.NewSlot, depositPacket.PartnerBackpack ? InventoryType.PetWarehouse : InventoryType.Warehouse);
            if (item.Item.IsWarehouseable)
            {
                Session.SendPacket("msg 4 You can't do this");
                return;
            }
            if (Session.Character.HasShopOpened)
            {
                return;
            }

            if (depositPacket.PartnerBackpack)
            {
                AddToPartnerBackpack(originalItem, depositPacket, ref anotherItem);
                return;
            }

            // actually move the item from source to destination
            Session.Character.Inventory.MoveItem(depositPacket.Inventory, InventoryType.Warehouse, depositPacket.Slot,
                depositPacket.Amount, depositPacket.NewSlot, out originalItem, out anotherItem);
            Logger.LogUserEvent("STASH_DEPOSIT", Session.GenerateIdentity(),$"[Deposit]OldIIId: {item?.Id} NewIIId: {itemdest?.Id} Amount: {depositPacket.Amount} PartnerBackpack: {depositPacket.PartnerBackpack}");
            Logger.LogUserEvent("WAREHOUSE_DEPOSIT_LOG_PACKET", Session.GenerateIdentity(), $"Packet string: {depositPacket.OriginalContent.ToString()}");

            if (anotherItem == null)
            {
                return;
            }
            
            Session.SendPacket(UserInterfaceHelper.Instance.GenerateInventoryRemove(depositPacket.Inventory, depositPacket.Slot));
            Session.SendPacket(anotherItem.GenerateStash());
        }

        private void AddToPartnerBackpack(ItemInstance originalItem, DepositPacket packet, ref ItemInstance partnerItem)
        {
            Session.Character.Inventory.MoveItem(originalItem.Type, InventoryType.PetWarehouse, originalItem.Slot, 
                packet.Amount, packet.NewSlot, out originalItem, out partnerItem);

            if (partnerItem == null)
            {
                return;
            }
            
            Session.SendPacket(UserInterfaceHelper.Instance.GenerateInventoryRemove(packet.Inventory, packet.Slot));
            Session.SendPacket(partnerItem.GeneratePStash());
        }

        #endregion
    }
}