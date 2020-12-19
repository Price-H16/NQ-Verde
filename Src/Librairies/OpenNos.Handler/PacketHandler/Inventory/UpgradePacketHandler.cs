using System;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension.Inventory;
using OpenNos.GameObject.Extension.Item;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Inventory
{
    public class UpgradePacketHandler : IPacketHandler
    {
        #region Instantiation

        public UpgradePacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Upgrade(UpgradePacket upgradePacket)
        {
            if (upgradePacket == null || Session.Character.ExchangeInfo?.ExchangeList.Count > 0 || Session.Character.Speed == 0 || Session.Character.LastDelay.AddSeconds(5) > DateTime.Now)
            {
                return;
            }

            var inventoryType = upgradePacket.InventoryType;
            var uptype = upgradePacket.UpgradeType;
            var slot = upgradePacket.Slot;
            bool isCarveRemove;
            Session.Character.LastDelay = DateTime.Now;
            if (uptype == 83 || uptype == 84)
            {
                inventoryType = InventoryType.Equipment;
                isCarveRemove = upgradePacket.InventoryType == InventoryType.Etc ? true : false;
                slot = (short)upgradePacket.InventoryType2;
            }
            else
            {
                inventoryType = upgradePacket.InventoryType;
                isCarveRemove = false;
            }
            ItemInstance inventory = Session.Character.Inventory.LoadBySlotAndType(slot, inventoryType);
            if (inventory == null) return;

            var item = Session.Character.Inventory.LoadBySlotAndType(slot, inventoryType);
            var slot2 = upgradePacket.Slot2;
            switch (uptype)
            {
                // Fusion Scroll
                case 53:
                    {
                        inventory = Session.Character.Inventory.LoadBySlotAndType((byte)slot, inventoryType);
                        var secondItem = Session.Character.Inventory.LoadBySlotAndType((byte)slot2, inventoryType);
                        if (inventory == null)
                        {
                            return;
                        }

                        if (secondItem == null)
                        {
                            return;
                        }

                        inventory.FusionItem(Session, secondItem);
                        Session.SendPacket("shop_end 2");
                        Session.SendPacket("shop_end 1");
                    }
                    break;

                // Craft tattoo
                case 81:
                {
                    inventory = Session.Character.Inventory.LoadBySlotAndType(slot, InventoryType.Main);

                    if (inventory == null) return;

                    inventory.CraftTattoo(Session);
                }
                    break;

                // Tattoo Upgrade / Remove
                case 82:
                case 85:
                {
                    var ski = Session.Character.Skills.FirstOrDefault(s => s.SkillVNum == slot);

                    if (ski == null) return;

                    switch (inventoryType)
                    {
                        // Remove Tattoo Inked
                        case (InventoryType) 2:
                            if (uptype != 82) return;
                            ski.RemoveTattoo(Session);
                            break;

                        // Upgrade Tattoo
                        case (InventoryType) 1: // NPC
                        case (InventoryType) 3: // with scroll
                            ski.UpgradeTattoo(Session, uptype == 82 ? false : true);
                            break;

                        default:
                            return;
                    }
                }
                    break;

                case 83: //Runes
                    if (inventory.Item.EquipmentSlot == EquipmentType.MainWeapon)
                    { 
                        if (isCarveRemove)
                        {
                            int hammerCount = Session.Character.Inventory.CountItem(5812);
                            if (hammerCount >= 1)
                            {
                                Session.Character.Inventory.RemoveItemAmount(5812, 1);
                                if (Session.Character.Inventory.CountItem(5812) == hammerCount - 1)
                                {
                                    inventory.RemoveRuneEffets();
                                    inventory.RuneCount = 0;
                                    Session.SendPacket(Session.Character.GenerateSay("Runes removed!", 12));
                                    Session.SendPacket("shop_end 1");
                                    Session.SendPacket(inventory.GenerateEInfo());
                                }
                            }
                        }
                        else
                        {
                            inventory.UpgradeRunes(Session, RuneScrollType.NoScroll);
                        }
                    }
                    else
                    {
                        return;
                    }

                    break;

                case 84: // Scroll with double boost
                    if (inventory.Item.EquipmentSlot == EquipmentType.MainWeapon)
                    {
                        inventory.UpgradeRunes(Session, RuneScrollType.PremiumScroll);
                    }
                    else
                    {
                        return;
                    }

                    break;


                case 86: // Runes Upgrade With Scroll
                    if (inventory.Item.EquipmentSlot == EquipmentType.MainWeapon)
                    {
                        inventory.UpgradeRunes(Session, RuneScrollType.NormalScroll);
                    }
                    else
                    {
                        return;
                    }

                    break;

                case 50: //zenas eggs
                case 51: //erenia eggs
                case 52: //fernon eggs
                {
                    inventory = Session.Character.Inventory.LoadBySlotAndType(slot, InventoryType.Equipment);

                    if (inventory == null) return;

                    inventory.CraftFairy(Session, (byte) (uptype - 50));
                }
                    break;

                // Convert Item to Partner
                case 0:
                {
                    inventory = Session.Character.Inventory.LoadBySlotAndType(slot, inventoryType);
                    if (inventory == null) return;

                    if ((inventory.Item.EquipmentSlot == EquipmentType.Armor
                         || inventory.Item.EquipmentSlot == EquipmentType.MainWeapon
                         || inventory.Item.EquipmentSlot == EquipmentType.SecondaryWeapon)
                        && inventory.Item.ItemType != ItemType.Shell && inventory.Item.Type == InventoryType.Equipment)
                        inventory.ConvertToPartnerEquipment(Session);
                }
                    break;

                // UpgradeItem
                case 20:
                case 43:
                case 1:
                {
                    inventory = Session.Character.Inventory.LoadBySlotAndType(slot, inventoryType);

                    if (inventory == null) return;
                    if ((inventory.Item.EquipmentSlot == EquipmentType.Armor
                         || inventory.Item.EquipmentSlot == EquipmentType.MainWeapon
                         || inventory.Item.EquipmentSlot == EquipmentType.SecondaryWeapon)
                        && inventory.Item.ItemType != ItemType.Shell && inventory.Item.Type == InventoryType.Equipment)
                    {
                        var HasAmulet = FixedUpMode.None;
                        var amulet =
                            Session.Character.Inventory.LoadBySlotAndType((short) EquipmentType.Amulet,
                                InventoryType.Wear);
                        if (amulet?.Item.Effect == 793) HasAmulet = FixedUpMode.HasAmulet;
                        inventory.UpgradeItem(Session, uptype == 43 ? UpgradeMode.Reduced : UpgradeMode.Normal,
                            uptype == 1 ? UpgradeProtection.None : UpgradeProtection.Protected, hasAmulet: HasAmulet);
                    }
                }
                    break;

                // Cellon Item
                case 3:
                {
                    //up_gr 3 0 0 7 1 1 20 99
                    var originalSplit = upgradePacket.OriginalContent.Split(' ');
                    if (originalSplit.Length == 10
                        && byte.TryParse(originalSplit[5], out var firstSlot)
                        && byte.TryParse(originalSplit[8], out var secondSlot))
                    {
                        inventory = Session.Character.Inventory.LoadBySlotAndType(firstSlot, InventoryType.Equipment);
                        if (inventory != null
                            && (inventory.Item.EquipmentSlot == EquipmentType.Necklace
                                || inventory.Item.EquipmentSlot == EquipmentType.Bracelet
                                || inventory.Item.EquipmentSlot == EquipmentType.Ring)
                            && inventory.Item.ItemType != ItemType.Shell &&
                            inventory.Item.Type == InventoryType.Equipment)
                        {
                            var cellon =
                                Session.Character.Inventory.LoadBySlotAndType(secondSlot,
                                    InventoryType.Main);
                            if (cellon?.ItemVNum > 1016 && cellon.ItemVNum < 1027)
                                inventory.OptionItem(Session, cellon);
                        }
                    }
                }
                    break;

                // Rarify item
                case 21:
                case 7:
                {
                    inventory = Session.Character.Inventory.LoadBySlotAndType(slot, inventoryType);
                    if (inventory == null) return;

                    if (inventory.Item.EquipmentSlot != EquipmentType.Armor
                        && inventory.Item.EquipmentSlot != EquipmentType.MainWeapon
                        && inventory.Item.EquipmentSlot != EquipmentType.SecondaryWeapon)
                        return;

                    var mode = RarifyMode.Normal;
                    var protection = RarifyProtection.None;
                    var amulet =
                        Session.Character.Inventory.LoadBySlotAndType((short) EquipmentType.Amulet, InventoryType.Wear);
                    if (amulet != null)
                        switch (amulet.Item.Effect)
                        {
                            case 791:
                                protection = RarifyProtection.RedAmulet;
                                break;

                            case 792:
                                protection = RarifyProtection.BlueAmulet;
                                break;

                            case 794:
                                protection = RarifyProtection.HeroicAmulet;
                                break;

                            case 795:
                                protection = RarifyProtection.RandomHeroicAmulet;
                                break;

                            case 796:
                                if (inventory.Item.IsHeroic) mode = RarifyMode.Success;
                                break;
                        }

                    if (uptype == 7)
                    {
                        inventory.RarifyItem(Session, mode, protection);
                        Session.SendPacket("shop_end 1");
                        return;
                    }

                    inventory.RarifyItem(Session, RarifyMode.Normal, RarifyProtection.Scroll);
                }

                    Session.SendPacket("shop_end 1");

                    break;

                // Summing Res
                case 8:
                {
                    inventory = Session.Character.Inventory.LoadBySlotAndType(slot, inventoryType);
                    if (inventory == null) return;

                    if (upgradePacket.InventoryType2 == null && upgradePacket.Slot2 == null) return;

                    var inventory2 =
                        Session.Character.Inventory.LoadBySlotAndType((byte) upgradePacket.Slot2,
                            (InventoryType) upgradePacket.InventoryType2);

                    if (inventory2 == null) return;

                    if (Equals(inventory, inventory2)) return;

                    inventory.SummingItem(Session, inventory2);
                }
                    break;

                // Upgrade Sp
                case 25:
                case 26:
                case 9:
                {
                    var specialist = Session.Character.Inventory.LoadBySlotAndType(slot, inventoryType);

                    if (specialist == null) return;

                    if (specialist.Rare == -2)
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                            Language.Instance.GetMessageFromKey("CANT_UPGRADE_DESTROYED_SP"), 0));
                        return;
                    }

                    if (uptype == 25)
                        if (specialist.Upgrade > 9)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                string.Format(Language.Instance.GetMessageFromKey("MUST_USE_ITEM"),
                                    ServerManager.GetItem(1364).Name), 0));
                            return;
                        }

                    if (uptype == 26)
                        if (specialist.Upgrade <= 9)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                string.Format(Language.Instance.GetMessageFromKey("MUST_USE_ITEM"),
                                    ServerManager.GetItem(1363).Name), 0));
                            return;
                        }

                    specialist.UpgradeSp(Session,
                        uptype == 9 ? UpgradeProtection.None : UpgradeProtection.Protected);
                }
                    break;

                // Sp Fun Upgrade
                case 35:
                case 38:
                case 42:
                {
                    if (item == null) return;

                    if (item.ItemVNum != 907) return;

                    if (item.ItemVNum != 900) return;

                    if (item.ItemVNum != 4099) return;

                    if (item.Rare != -2)
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(
                                Language.Instance.GetMessageFromKey("CANT_UPGRADE_DESTROYED_SP"), 0));
                        return;
                    }

                    if (item.Item.EquipmentSlot != EquipmentType.Sp) return;

                    var type = uptype == 35 ? 0 : uptype == 38 ? 1 : 2;
                    item.UpgradeSpFun(Session, type);
                }
                    break;

                // Perfect Sp
                case 41:
                {
                    item = Session.Character.Inventory.LoadBySlotAndType(slot, inventoryType);
                    if (item == null) return;

                    if (item.Rare == -2)
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                            Language.Instance.GetMessageFromKey("CANT_UPGRADE_DESTROYED_SP"), 0));
                        return;
                    }

                    for (var i = 0; i < 500; i++)
                        if (item.Item.EquipmentSlot == EquipmentType.Sp)
                            item.PerfectSp(Session);
                }
                    break;
            }
        }

        #endregion
    }
}