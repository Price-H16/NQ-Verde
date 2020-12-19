using System.Linq;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Inventory
{
    public class ReposPacketHandler : IPacketHandler
    {
        #region Instantiation

        public ReposPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Repos(ReposPacket reposPacket)
        {
            if (reposPacket == null)
            {
                return;
            }
            
            Logger.LogUserEvent("STASH_REPOS", Session.GenerateIdentity(), $"[ItemReposition]OldSlot: {reposPacket.OldSlot} NewSlot: {reposPacket.NewSlot} Amount: {reposPacket.Amount} PartnerBackpack: {reposPacket.PartnerBackpack}");

            if (reposPacket.Amount < 1)
            {
                return;
            }

            if (reposPacket.OldSlot == reposPacket.NewSlot)
            {
                return;
            }

            if (reposPacket.Amount == 0)
            {
                return;
            }
            // check if the destination slot is out of range
            if (reposPacket.NewSlot >= (reposPacket.PartnerBackpack ? Session.Character.StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.PetBackPack) ? 50 : 0 : Session.Character.WareHouseSize))
            {
                return;
            }

            if (reposPacket.Amount <= 0)
            {
                // Dupe.
                return;
            }

            ItemInstance itemToMove = Session.Character.Inventory.LoadBySlotAndType(reposPacket.OldSlot, reposPacket.PartnerBackpack ? InventoryType.PetWarehouse : InventoryType.Warehouse);

            if (itemToMove == null)
            {
                return;
            }

            if (reposPacket.Amount > itemToMove.Amount)
            {
                return;
            }

            // check if the character is allowed to move the item
            if (Session.Character.InExchangeOrTrade)
            {
                return;
            }

            if (Session.Character.HasShopOpened)
            {
                return;
            }

            // actually move the item from source to destination
            Session.Character.Inventory.MoveItem(reposPacket.PartnerBackpack ? InventoryType.PetWarehouse : InventoryType.Warehouse, reposPacket.PartnerBackpack ? InventoryType.PetWarehouse : InventoryType.Warehouse, reposPacket.OldSlot, reposPacket.Amount, reposPacket.NewSlot, out itemToMove, out ItemInstance newInventory);

            if (newInventory == null)
            {
                return;
            }

            Session.SendPacket(reposPacket.PartnerBackpack ? newInventory.GeneratePStash() : newInventory.GenerateStash());
            Session.SendPacket(itemToMove != null ? reposPacket.PartnerBackpack ? itemToMove.GeneratePStash() : itemToMove.GenerateStash() : reposPacket.PartnerBackpack ? UserInterfaceHelper.Instance.GeneratePStashRemove(reposPacket.OldSlot) : UserInterfaceHelper.Instance.GenerateStashRemove(reposPacket.OldSlot));
        }

        #endregion
    }
}