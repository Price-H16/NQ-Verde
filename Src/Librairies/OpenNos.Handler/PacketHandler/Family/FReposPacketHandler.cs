using System;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Family
{
    public class FReposPacketHandler : IPacketHandler
    {
        #region Instantiation

        public FReposPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void FamilyRepos(FReposPacket fReposPacket)
        {
            if (DateTime.Now <= Session.Character.LastRepos.AddSeconds(2))
            {
                return;
            }

            Session.Character.LastRepos = DateTime.Now;


            if (fReposPacket == null)
            {
                return;
            }

            if (fReposPacket.OldSlot == fReposPacket.NewSlot)
            {
                return;
            }

            if (fReposPacket.Amount < 1)
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

            // check if the character is allowed to move the item
            if (Session.Character.InExchangeOrTrade)
            {
                return;
            }

            if (Session.Character.HasShopOpened)
            {
                return;
            }

            if (fReposPacket.NewSlot > Session.Character.Family.WarehouseSize)
            {
                return;
            }

            ItemInstance sourceItem = Session.Character.Family.Warehouse.LoadBySlotAndType(fReposPacket.OldSlot, InventoryType.FamilyWareHouse);

            if (sourceItem == null)
            {
                return;
            }

            if (fReposPacket.Amount > sourceItem.Amount && sourceItem.Type != InventoryType.Equipment)
            {
                return;
            }

            ItemInstance destinationInventory = Session.Character.Family.Warehouse.LoadBySlotAndType(fReposPacket.NewSlot, InventoryType.FamilyWareHouse);

            if (destinationInventory == null)
            {
                destinationInventory = sourceItem.DeepCopy();
                sourceItem.Amount -= fReposPacket.Amount;
                destinationInventory.Amount = fReposPacket.Amount;
                destinationInventory.Slot = fReposPacket.NewSlot;
                if (sourceItem.Amount > 0)
                {
                    destinationInventory.Id = Guid.NewGuid();
                }
                else
                {
                    sourceItem = null;
                }
            }
            else
            {
                if (destinationInventory.ItemVNum == sourceItem.ItemVNum && sourceItem.Item.Type != InventoryType.Equipment)
                {
                    if (destinationInventory.Amount + fReposPacket.Amount > 999)
                    {
                        int saveItemCount = destinationInventory.Amount;
                        destinationInventory.Amount = 999;
                        sourceItem.Amount = (short) (saveItemCount + sourceItem.Amount - 999);
                    }
                    else
                    {
                        destinationInventory.Amount += fReposPacket.Amount;
                        sourceItem.Amount -= fReposPacket.Amount;
                        if (sourceItem.Amount <= 0)
                        {
                            DAOFactory.ItemInstanceDAO.Delete(sourceItem.Id);
                            sourceItem = null;
                        }
                    }
                }
                else
                {
                    destinationInventory.Slot = fReposPacket.OldSlot;
                    sourceItem.Slot = fReposPacket.NewSlot;
                }
            }

            if (sourceItem?.Amount > 0)
            {
                DAOFactory.ItemInstanceDAO.InsertOrUpdate(sourceItem);
            }

            if (destinationInventory?.Amount > 0)
            {
                DAOFactory.ItemInstanceDAO.InsertOrUpdate(destinationInventory);
            }

            Session.Character.Family.SendPacket(destinationInventory.GenerateFStash());
            Session.Character.Family.SendPacket(sourceItem != null ? sourceItem.GenerateFStash()
                : UserInterfaceHelper.Instance.GenerateFStashRemove(fReposPacket.OldSlot));

            ServerManager.Instance.FamilyRefresh(Session.Character.Family.FamilyId);
        }

        #endregion
    }
}