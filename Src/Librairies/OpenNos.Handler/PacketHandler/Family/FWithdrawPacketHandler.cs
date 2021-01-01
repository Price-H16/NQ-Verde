using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using System;

namespace OpenNos.Handler.PacketHandler.Family
{
    public class FWithdrawPacketHandler : IPacketHandler
    {
        #region Instantiation

        public FWithdrawPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void FamilyWithdraw(FWithdrawPacket fWithdrawPacket)
        {
            lock (Session.Character.Family.Warehouse)
            {
                lock (Session.Character.Inventory)
                {
                    if (DateTime.Now <= Session.Character.LastWithdraw.AddSeconds(2))
                    {
                        return;
                    }

                    Session.Character.LastWithdraw = DateTime.Now;

                    if (fWithdrawPacket == null)
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
                             && Session.Character.Family.MemberAuthorityType == FamilyAuthorityType.ALL
                             || Session.Character.FamilyCharacter.Authority == FamilyAuthority.Familykeeper
                             && Session.Character.Family.ManagerAuthorityType == FamilyAuthorityType.ALL))
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("NO_FAMILY_RIGHT")));
                        return;
                    }

                    if (fWithdrawPacket.Amount < 1)
                    {
                        return;
                    }

                    if (Session.Character.InExchangeOrTrade)
                    {
                        return;
                    }

                    if (Session.Character.HasShopOpened)
                    {
                        return;
                    }

                    ItemInstance familyWarehouseItem = Session.Character.Family.Warehouse.LoadBySlotAndType(fWithdrawPacket.Slot, InventoryType.FamilyWareHouse);

                    if (familyWarehouseItem == null)
                    {
                        return;
                    }

                    if (!Session.Character.Inventory.CanAddItem(familyWarehouseItem.ItemVNum))
                    {
                        return;
                    }

                    if (fWithdrawPacket.Amount > familyWarehouseItem.Amount)
                    {
                        return;
                    }

                    var item2 = familyWarehouseItem.DeepCopy();
                    item2.Id = Guid.NewGuid();
                    item2.Amount = fWithdrawPacket.Amount;

                    familyWarehouseItem.Amount -= fWithdrawPacket.Amount;
                    if (familyWarehouseItem.Amount <= 0)
                    {
                        DAOFactory.ItemInstanceDAO.Delete(familyWarehouseItem.Id);
                        familyWarehouseItem = null;
                    }

                    Session.Character.Inventory.AddToInventory(item2, item2.Item.Type);
                    Session.Character.Family.SendPacket(UserInterfaceHelper.Instance.GenerateFStashRemove(fWithdrawPacket.Slot));
                    if (familyWarehouseItem != null)
                    {
                        DAOFactory.ItemInstanceDAO.InsertOrUpdate(familyWarehouseItem);
                    }

                    Session.Character.Family.InsertFamilyLog(FamilyLogType.WareHouseRemoved, Session.Character.Name, message: $"{item2.ItemVNum}|{fWithdrawPacket.Amount}");
                }
            }
        }

        #endregion
    }
}