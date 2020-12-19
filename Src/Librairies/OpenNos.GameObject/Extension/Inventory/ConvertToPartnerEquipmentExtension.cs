using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;

namespace OpenNos.GameObject.Extension.Inventory
{
    public static class ConvertToPartnerEquipmentExtension
    {
        #region Methods

        public static void ConvertToPartnerEquipment(this ItemInstance e, ClientSession session)
        {
            const int sandVnum = 1027;
            long goldprice = 2000 + e.Item.LevelMinimum * 300;

            if (e.Item.IsHeroic)
            {
                session.SendPacket(
                    UserInterfaceHelper.GenerateInfo(
                        Language.Instance.GetMessageFromKey("CANT_CONVERT_HEROIC")));
                session.SendPacket("shop_end 1");
                return;
            }

            if (session.Character.Gold >= goldprice &&
                session.Character.Inventory.CountItem(sandVnum) >= e.Item.LevelMinimum)
            {
                e.IsPartnerEquipment = true;
                e.ClearShell();
                e.ShellRarity = null;
                DAOFactory.ShellEffectDAO.DeleteByEquipmentSerialId(e.EquipmentSerialId);
                e.BoundCharacterId = null;
                e.HoldingVNum = e.ItemVNum;
                var newItem = e.HardItemCopy();
                switch (e.Item.EquipmentSlot)
                {
                    case EquipmentType.MainWeapon:
                    case EquipmentType.SecondaryWeapon:
                        switch (e.Item.Class)
                        {
                            case 2:
                                newItem.ItemVNum = 990;
                                break;

                            case 4:
                                newItem.ItemVNum = 991;
                                break;

                            case 8:
                                newItem.ItemVNum = 992;
                                break;

                            default:
                                session.SendPacket("shop_end 1");
                                return;
                        }

                        break;

                    case EquipmentType.Armor:
                        switch (e.Item.Class)
                        {
                            case 2:
                                newItem.ItemVNum = 997;
                                break;

                            case 4:
                                newItem.ItemVNum = 996;
                                break;

                            case 8:
                                newItem.ItemVNum = 995;
                                break;

                            default:
                                session.SendPacket("shop_end 1");
                                return;
                        }

                        break;
                }

                session.Character.Inventory.DeleteFromSlotAndType(e.Slot, e.Type);
                session.Character.Inventory.AddToInventory(newItem, InventoryType.Equipment);
                session.Character.Inventory.RemoveItemAmount(sandVnum, e.Item.LevelMinimum);
                session.Character.Gold -= goldprice;
                session.SendPacket(session.Character.GenerateGold());
                session.SendPacket(newItem.GenerateInventoryAdd());
                session.SendPacket("shop_end 1");
            }
        }

        #endregion
    }
}