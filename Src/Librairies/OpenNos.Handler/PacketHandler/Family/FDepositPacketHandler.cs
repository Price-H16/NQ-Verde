using System;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Family
{
    public class FDepositPacketHandler : IPacketHandler
    {
        #region Instantiation

        public FDepositPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void FamilyDeposit(FDepositPacket fDepositPacket)
        {

            //blocked deposit family warehouse
            return;
            if (DateTime.Now <= Session.Character.LastDeposit.AddSeconds(2))
            {
                return;
            }

            Session.Character.LastDeposit = DateTime.Now;


            if (fDepositPacket == null)
            {
                return;
            }

            if (fDepositPacket.Inventory != InventoryType.Equipment && fDepositPacket.Inventory != InventoryType.Main &&
                fDepositPacket.Inventory != InventoryType.Etc)
            {
                return;
            }

            if (Session.Character.Family?.Warehouse == null)
            {
                return;
            }

            if (!(Session.Character.FamilyCharacter.Authority == FamilyAuthority.Head
                     || Session.Character.FamilyCharacter.Authority == FamilyAuthority.Familydeputy
                     || Session.Character.FamilyCharacter.Authority == FamilyAuthority.Member
                     && Session.Character.Family.MemberAuthorityType != FamilyAuthorityType.NONE
                     || Session.Character.FamilyCharacter.Authority == FamilyAuthority.Familykeeper
                     && Session.Character.Family.ManagerAuthorityType != FamilyAuthorityType.NONE))
            {
                Session.SendPacket(
                    UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("NO_FAMILY_RIGHT")));
                return;
            }

            ItemInstance originalItem = Session.Character.Inventory.LoadBySlotAndType(fDepositPacket.Slot, fDepositPacket.Inventory);

            if (originalItem == null)
            {
                return;
            }

            if (originalItem.Item.ItemType == ItemType.Quest1 || originalItem.Item.ItemType == ItemType.Quest2)
            {
                return;
            }

            if (fDepositPacket.Amount > originalItem.Amount)
            {
                return;
            }

            if (originalItem.IsBound)
            {
                return;
            }

            if (!originalItem.Item.IsTradable || !originalItem.Item.IsSoldable)
            {
                return;
            }
            
            ItemInstance itemInFamilyWarehouse = Session.Character.Family.Warehouse.LoadBySlotAndType(fDepositPacket.NewSlot, InventoryType.FamilyWareHouse);

            if (itemInFamilyWarehouse != null)
            {
                return;
            }

            // check if the destination slot is out of range
            if (fDepositPacket.NewSlot > Session.Character.Family.WarehouseSize)
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
            
            Session.Character.Inventory.MoveItem(fDepositPacket.Inventory, InventoryType.FamilyWareHouse, fDepositPacket.Slot, fDepositPacket.Amount, fDepositPacket.NewSlot, out originalItem, out itemInFamilyWarehouse);

            if (itemInFamilyWarehouse == null)
            {
                return;
            }
            
            DAOFactory.ItemInstanceDAO.InsertOrUpdate(itemInFamilyWarehouse);
            Session.Character.Family.SendPacket(originalItem != null ? originalItem.GenerateInventoryAdd() : UserInterfaceHelper.Instance.GenerateInventoryRemove(fDepositPacket.Inventory, fDepositPacket.Slot));
            Session.Character.Family.SendPacket(itemInFamilyWarehouse.GenerateFStash());
            //Session.Character.Family?.InsertFamilyLog(FamilyLogType.WareHouseAdded, Session.Character.Name, message: $"{itemInFamilyWarehouse.ItemVNum}|{fDepositPacket.Amount}");
            Session.Character.Inventory.DeleteById(itemInFamilyWarehouse.Id);
        }

        #endregion
    }
}