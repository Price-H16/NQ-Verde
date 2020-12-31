using System;
using NosTale.Configuration;
using NosTale.Configuration.Utilities;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject.Extension.Inventory
{
    public static class RarifyItemExtension
    {
        #region Methods

        public static void Fail(this ItemInstance e, ClientSession session, RarifyMode mode,
            RarifyProtection protection)
        {
            if (mode != RarifyMode.Drop && session != null)
            {
                var item = session.Character.Inventory.GetItemInstanceById(e.Id);
                switch (protection)
                {
                    case RarifyProtection.BlueAmulet:
                    case RarifyProtection.RedAmulet:
                    case RarifyProtection.HeroicAmulet:
                    case RarifyProtection.RandomHeroicAmulet:
                        session.Character.RemoveBuff(62);
                        var amulet = session.Character.Inventory.LoadBySlotAndType((short) EquipmentType.Amulet,InventoryType.Wear);
                        if (amulet != null)
                        {
                            amulet.DurabilityPoint -= 1;
                            if (amulet.DurabilityPoint <= 0)
                            {
                                session.Character.DeleteItemByItemInstanceId(amulet.Id);
                                session.SendPacket($"info {Language.Instance.GetMessageFromKey("AMULET_DESTROYED")}");
                                session.SendPacket(session.Character.GenerateEquipment());
                            }
                            else
                            {
                                session.Character.AddBuff(new Buff(62, session.Character.Level),session.Character.BattleEntity);
                            }
                        }

                        session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("AMULET_FAIL_SAVED"),11));
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("AMULET_FAIL_SAVED"), 0));
                        return;

                    case RarifyProtection.None:
                        session.Character.DeleteItemByItemInstanceId(e.Id);
                        session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("RARIFY_FAILED"),11));
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("RARIFY_FAILED"), 0));
                        return;
                }

                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("RARIFY_FAILED_ITEM_SAVED"),11));
                session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("RARIFY_FAILED_ITEM_SAVED"), 0));
                session.CurrentMapInstance.Broadcast(session.Character.GenerateEff(3004), session.Character.MapX,session.Character.MapY);
            }
        }

        public static void Rarify(this ItemInstance e, ClientSession player, sbyte rarity, RarifyMode mode,
            RarifyProtection protection)
        {
            if (mode != RarifyMode.Drop) player.Character?.NotifyRarifyResult(rarity);

            e.Rare = rarity;
            e.GenerateHeroicShell(protection);
            e.SetRarityPoint();
        }

        public static void RarifyItem(this ItemInstance e, ClientSession session, RarifyMode mode,
            RarifyProtection protection,
            bool isCommand = false, sbyte forceRare = 0)
        {
            if (forceRare != 0)
            {
                e.Rarify(session, forceRare, mode, protection);
                return;
            }
            var rnd2 = ServerManager.RandomNumber();
            if (ServerManager.Instance.Configuration.EventRareUpEq != 0)
            {
                rnd2 -= (rnd2 / 100) * ServerManager.Instance.Configuration.EventRareUpEq;
            }
            var conf = DependencyContainer.Instance.GetInstance<JsonItemConfiguration>().RarifyChances;

            if (session != null && !session.HasCurrentMapInstance) return;

            double[] rare =
            {
                conf.Raren2, conf.Raren1, conf.Rare0,
                conf.Rare1, conf.Rare2, conf.Rare3,
                conf.Rare4, conf.Rare5, conf.Rare6,
                conf.Rare7, conf.Rare8
            };
            double rnd;
            sbyte[] rareitem = {-2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8};

            if (mode != RarifyMode.Drop || e.Item.ItemType == ItemType.Shell)
            {
                rare[0] = 0;
                rare[1] = 0;
                rare[2] = 0;
                rnd = ServerManager.RandomNumber(0, 80);
            }
            else
            {
                rnd = ServerManager.RandomNumber(0, 1000) / 10D;
            }

            if (protection == RarifyProtection.RedAmulet || protection == RarifyProtection.HeroicAmulet || protection == RarifyProtection.RandomHeroicAmulet)
                for (byte i = 0; i < rare.Length; i++) rare[i] = (byte) (rare[i] * conf.ReducedChanceFactor);

            switch (mode)
            {
                case RarifyMode.Free:
                    break;

                case RarifyMode.Success:
                    if (e.Item.IsHeroic && e.Rare >= 8 || !e.Item.IsHeroic && e.Rare <= 7)
                    {
                        session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("ALREADY_MAX_RARE"), 10));
                        return;
                    }

                    e.Rare += 1;
                    e.SetRarityPoint();
                    var inventory = session?.Character.Inventory.GetItemInstanceById(e.Id);
                    if (inventory != null) session.SendPacket(inventory.GenerateInventoryAdd());
                    return;

                case RarifyMode.Reduced:
                    session.SendPacket(session.Character.GenerateGold());
                    if (e.Rare < 8 || !e.Item.IsHeroic)
                    {
                        session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_MAX_RARITY"),10));
                        return;
                    }

                    e.Rare -= (sbyte) ServerManager.RandomNumber(0, 7);
                    e.GenerateHeroicShell(protection);
                    e.ShellRarity = e.Rare;
                    e.SetRarityPoint();
                    var inv = session.Character.Inventory.GetItemInstanceById(e.Id);
                    session.SendPacket(inv?.GenerateInventoryAdd());
                    session.Character.NotifyRarifyResult(e.Rare);
                    session.SendPacket(session.Character.GenerateEquipment());
                    break;

                case RarifyMode.Normal:
                    if (session.Character.Gold < conf.GoldPrice) 
                        return;

                    if (session.Character.Inventory.CountItem(conf.RarifyItemNeededVnum) < conf.RarifyItemNeededQuantity) 
                        return;

                    if (protection == RarifyProtection.Scroll && !isCommand && session.Character.Inventory.CountItem(conf.ScrollVnum) < 1)
                        return;

                    if ((protection == RarifyProtection.Scroll || protection == RarifyProtection.BlueAmulet || protection == RarifyProtection.RedAmulet) && !isCommand && e.Item.IsHeroic)
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IS_HEROIC"), 0));
                        return;
                    }

                    if ((protection == RarifyProtection.HeroicAmulet || protection == RarifyProtection.RandomHeroicAmulet) && !e.Item.IsHeroic)
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_NOT_HEROIC"), 0));
                        return;
                    }

                    if (e.Item.IsHeroic && e.Rare == 8)
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ALREADY_MAX_RARE"), 0));
                        return;
                    }

                    if (protection == RarifyProtection.Scroll && !isCommand)
                    {
                        session.Character.Inventory.RemoveItemAmount(conf.ScrollVnum);
                        session.SendPacket(session.Character.Inventory.CountItem(conf.ScrollVnum) < 1 ? "shop_end 2" : "shop_end 1");
                    }

                    session.GoldLess(conf.GoldPrice);
                    session.Character.Inventory.RemoveItemAmount(conf.RarifyItemNeededVnum,conf.RarifyItemNeededQuantity);
                    break;

                case RarifyMode.Drop:
                    break;

                case RarifyMode.HeroEquipmentDowngrade:
                {
                    e.Rarify(session, 7, mode, protection);
                    return;
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            var sayfail = false;
            if (e.Item.IsHeroic && protection != RarifyProtection.None)
                if (rnd < rare[10])
                {
                    if (mode != RarifyMode.Drop) session?.Character.NotifyRarifyResult(8);

                    e.Rare = 8;

                    if (e.Item.IsHeroic) e.GenerateHeroicShell(protection);
                    e.SetRarityPoint();
                    var inventory = session?.Character.Inventory.GetItemInstanceById(e.Id);
                    if (inventory != null) session.SendPacket(inventory.GenerateInventoryAdd());
                    return;
                }

            for (byte y = 9; y != 0; y--)
            {
                if (!(rnd < rare[y])) continue;

                e.Rarify(session, rareitem[y], mode, protection);
                sayfail = true;
                break;
            }

            if (!sayfail) e.Fail(session, mode, protection);

            if (mode == RarifyMode.Drop || session == null) return;

            var inventoryb = session.Character.Inventory.GetItemInstanceById(e.Id);
            if (inventoryb != null) session.SendPacket(inventoryb.GenerateInventoryAdd());
        }

        #endregion
    }
}