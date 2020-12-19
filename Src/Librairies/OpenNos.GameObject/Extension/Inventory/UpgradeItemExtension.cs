using NosTale.Configuration;
using NosTale.Configuration.Utilities;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject.Extension.Inventory
{
    public static class UpgradeItemExtension
    {
        #region Methods

        public static void UpgradeItem(this ItemInstance e, ClientSession session, UpgradeMode mode,
            UpgradeProtection protection, bool isCommand = false, FixedUpMode hasAmulet = FixedUpMode.None)
        {
            var conf = DependencyContainer.Instance.GetInstance<JsonItemConfiguration>().UpgradeItem;
            if (!session.HasCurrentMapInstance) return;

            if (e.Upgrade >= conf.MaximumUpgrade) return;

            var upfail = e.Rare >= 8 ? conf.UpFailR8 : conf.UpFail;
            var upfix = e.Rare >= 8 ? conf.UpFixR8 : conf.UpFix;
            var goldprice = e.Rare >= 8 ? conf.GoldPriceR8 : conf.GoldPrice;
            var cella = e.Rare >= 8 ? conf.CellaAmountR8 : conf.CellaAmount;
            var gem = e.Rare >= 8 ? conf.GemAmountR8 : conf.GemAmount;

            if (hasAmulet == FixedUpMode.HasAmulet && e.IsFixed)
                upfix = new short[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

            if (e.IsFixed && hasAmulet == FixedUpMode.None)
            {
                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("ITEM_IS_FIXED"),
                    10));
                session.SendPacket("shop_end 2");
                return;
            }

            if (e.IsFixed && hasAmulet == FixedUpMode.HasAmulet) e.IsFixed = !e.IsFixed;

            switch (mode)
            {
                case UpgradeMode.Free:
                    break;

                case UpgradeMode.Reduced:

                    // TODO: Reduced Item Amount
                    if (session.Character.Gold < (long) (goldprice[e.Upgrade] * conf.ReducedPriceFactor))
                    {
                        session.SendPacket(
                            session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                        return;
                    }

                    if (session.Character.Inventory.CountItem(conf.CellaVnum) <
                        cella[e.Upgrade] * conf.ReducedPriceFactor)
                    {
                        session.SendPacket(session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                ServerManager.GetItem(conf.CellaVnum).Name, cella[e.Upgrade] * conf.ReducedPriceFactor),
                            10));
                        return;
                    }

                    if (protection == UpgradeProtection.Protected && !isCommand &&
                        session.Character.Inventory.CountItem(conf.GoldScrollVnum) < 1)
                    {
                        session.SendPacket(session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                ServerManager.GetItem(conf.GoldScrollVnum).Name,
                                cella[e.Upgrade] * conf.ReducedPriceFactor), 10));
                        return;
                    }

                    if (session.Character.Inventory.CountItem(e.Upgrade < 5 ? conf.GemVnum : conf.GemFullVnum) <
                        gem[e.Upgrade])
                    {
                        session.SendPacket(session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                ServerManager.GetItem(conf.GemVnum).Name, gem[e.Upgrade]), 10));
                        return;
                    }

                    session.Character.Inventory.RemoveItemAmount(e.Upgrade < 5 ? conf.GemVnum : conf.GemFullVnum,
                        gem[e.Upgrade]);

                    if (protection == UpgradeProtection.Protected && !isCommand)
                    {
                        session.Character.Inventory.RemoveItemAmount(conf.GoldScrollVnum);
                        session.SendPacket(session.Character.Inventory.CountItem(conf.GoldScrollVnum) < 1
                            ? "shop_end 2"
                            : "shop_end 1");
                    }

                    if (hasAmulet == FixedUpMode.HasAmulet && e.IsFixed)
                    {
                        var amulet =
                            session.Character.Inventory.LoadBySlotAndType(
                                (short) EquipmentType.Amulet, InventoryType.Wear);
                        amulet.DurabilityPoint -= 1;
                        if (amulet.DurabilityPoint <= 0)
                        {
                            session.Character.DeleteItemByItemInstanceId(amulet.Id);
                            session.SendPacket($"info {Language.Instance.GetMessageFromKey("AMULET_DESTROYED")}");
                            session.SendPacket(session.Character.GenerateEquipment());
                        }
                    }

                    session.GoldLess((long) (goldprice[e.Upgrade] * conf.ReducedPriceFactor));
                    session.Character.Inventory.RemoveItemAmount(conf.CellaVnum,
                        (int) (cella[e.Upgrade] * conf.ReducedPriceFactor));
                    break;

                case UpgradeMode.Normal:

                    // TODO: Normal Item Amount
                    if (session.Character.Inventory.CountItem(conf.CellaVnum) < cella[e.Upgrade]) return;

                    if (session.Character.Gold < goldprice[e.Upgrade])
                    {
                        session.SendPacket(
                            session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                        return;
                    }

                    if (protection == UpgradeProtection.Protected && !isCommand &&
                        session.Character.Inventory.CountItem(conf.NormalScrollVnum) < 1)
                    {
                        session.SendPacket(session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                ServerManager.GetItem(conf.NormalScrollVnum).Name, 1), 10));
                        return;
                    }

                    if (session.Character.Inventory.CountItem(e.Upgrade < 5 ? conf.GemVnum : conf.GemFullVnum) <
                        gem[e.Upgrade])
                    {
                        session.SendPacket(session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                ServerManager.GetItem(e.Upgrade < 5 ? conf.GemVnum : conf.GemFullVnum).Name,
                                gem[e.Upgrade]), 10));
                        return;
                    }

                    session.Character.Inventory.RemoveItemAmount(e.Upgrade < 5 ? conf.GemVnum : conf.GemFullVnum,
                        gem[e.Upgrade]);

                    if (protection == UpgradeProtection.Protected && !isCommand)
                    {
                        session.Character.Inventory.RemoveItemAmount(conf.NormalScrollVnum);
                        session.SendPacket(session.Character.Inventory.CountItem(conf.NormalScrollVnum) < 1
                            ? "shop_end 2"
                            : "shop_end 1");
                    }

                    if (hasAmulet == FixedUpMode.HasAmulet && e.IsFixed)
                    {
                        var amulet =
                            session.Character.Inventory.LoadBySlotAndType(
                                (short) EquipmentType.Amulet, InventoryType.Wear);
                        amulet.DurabilityPoint -= 1;
                        if (amulet.DurabilityPoint <= 0)
                        {
                            session.Character.DeleteItemByItemInstanceId(amulet.Id);
                            session.SendPacket($"info {Language.Instance.GetMessageFromKey("AMULET_DESTROYED")}");
                            session.SendPacket(session.Character.GenerateEquipment());
                        }
                    }

                    session.GoldLess(goldprice[e.Upgrade]);
                    session.Character.Inventory.RemoveItemAmount(conf.CellaVnum, cella[e.Upgrade]);
                    break;
            }

            var wearable = session.Character.Inventory.GetItemInstanceById(e.Id);

            var rnd = ServerManager.RandomNumber();
            if (ServerManager.Instance.Configuration.EventLvlUpEq != 0)
            {
                rnd += (rnd / 100) * ServerManager.Instance.Configuration.EventLvlUpEq;
            }
            if (e.Rare == 8)
            {
                if (rnd < upfail[e.Upgrade])
                {
                    if (protection == UpgradeProtection.None)
                    {
                        session.SendPacket(
                            session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_FAILED"), 11));
                        session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(
                                Language.Instance.GetMessageFromKey("UPGRADE_FAILED"), 0));
                        session.Character.DeleteItemByItemInstanceId(e.Id);
                    }
                    else
                    {
                        session.CurrentMapInstance.Broadcast(session.Character.GenerateEff(3004),
                            session.Character.MapX, session.Character.MapY);
                        session.SendPacket(
                            session.Character.GenerateSay(Language.Instance.GetMessageFromKey("SCROLL_PROTECT_USED"),
                                11));
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(
                            Language.Instance.GetMessageFromKey("UPGRADE_FAILED_ITEM_SAVED"), 0));
                    }
                }
                else if (rnd < upfix[e.Upgrade])
                {
                    session.CurrentMapInstance.Broadcast(session.Character.GenerateEff(3004), session.Character.MapX,
                        session.Character.MapY);
                    wearable.IsFixed = true;
                    session.SendPacket(
                        session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_FIXED"), 11));
                    session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_FIXED"),
                            0));
                }
                else
                {
                    session.CurrentMapInstance.Broadcast(session.Character.GenerateEff(3005), session.Character.MapX, session.Character.MapY);

                    session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"), 12));

                    session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"), 0));

                    wearable.Upgrade++;
                    if (wearable.Upgrade > 7)
                    {
                        session.Character.Family?.InsertFamilyLog(FamilyLogType.ItemUpgraded, session.Character.Name, itemVNum: wearable.ItemVNum, upgrade: wearable.Upgrade);

                    }                     
                    session.SendPacket(wearable.GenerateInventoryAdd());
                }
            }
            else
            {
                if (rnd < upfix[e.Upgrade])
                {
                    session.CurrentMapInstance.Broadcast(session.Character.GenerateEff(3004), session.Character.MapX,
                        session.Character.MapY);
                    wearable.IsFixed = true;
                    session.SendPacket(
                        session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_FIXED"), 11));
                    session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_FIXED"),
                            0));
                }
                else if (rnd < upfail[e.Upgrade])
                {
                    if (protection == UpgradeProtection.None)
                    {
                        session.SendPacket(
                            session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_FAILED"), 11));
                        session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(
                                Language.Instance.GetMessageFromKey("UPGRADE_FAILED"), 0));
                        session.Character.DeleteItemByItemInstanceId(e.Id);
                    }
                    else
                    {
                        session.CurrentMapInstance.Broadcast(session.Character.GenerateEff(3004),
                            session.Character.MapX, session.Character.MapY);
                        session.SendPacket(
                            session.Character.GenerateSay(Language.Instance.GetMessageFromKey("SCROLL_PROTECT_USED"),
                                11));
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(
                            Language.Instance.GetMessageFromKey("UPGRADE_FAILED_ITEM_SAVED"), 0));
                    }
                }
                else
                {
                    session.CurrentMapInstance.Broadcast(session.Character.GenerateEff(3005), session.Character.MapX, session.Character.MapY);

                    session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"), 12));

                    session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"), 0));

                    wearable.Upgrade++;
                    if (wearable.Upgrade > 7)
                    {
                        session.Character.Family?.InsertFamilyLog(FamilyLogType.ItemUpgraded, session.Character.Name, itemVNum: wearable.ItemVNum, upgrade: wearable.Upgrade);
                    }                      
                    session.SendPacket(wearable.GenerateInventoryAdd());
                }
            }
            session.SendPacket("shop_end 1");
        }

        #endregion
    }
}