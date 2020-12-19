using System.Linq;
using NosTale.Configuration;
using NosTale.Configuration.Utilities;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject.Extension.Inventory
{
    public static class UpgradeSpExtension
    {
        #region Methods

        public static void UpgradeSp(this ItemInstance e, ClientSession sesion, UpgradeProtection protect)
        {
            var conf = DependencyContainer.Instance.GetInstance<JsonItemConfiguration>().UpgradeSP;

            if (e.Upgrade >= 15)
            {
                return;
            }

            if (!sesion.HasCurrentMapInstance)
            {
                return;
            }

            SpecialistMorphType[] soulSpecialists =
            {
                SpecialistMorphType.Pajama,
                SpecialistMorphType.Warrior,
                SpecialistMorphType.Ninja,
                SpecialistMorphType.Ranger,
                SpecialistMorphType.Assassin,
                SpecialistMorphType.RedMage,
                SpecialistMorphType.HolyMage,
                SpecialistMorphType.Chicken,
                SpecialistMorphType.Jajamaru,
                SpecialistMorphType.Crusader,
                SpecialistMorphType.Berserker,
                SpecialistMorphType.BombArtificer,
                SpecialistMorphType.WildKeeper,
                SpecialistMorphType.IceMage,
                SpecialistMorphType.DarkGunner,
                SpecialistMorphType.Pirate,
                SpecialistMorphType.Drakenfer,
                SpecialistMorphType.MysticalArts,
                SpecialistMorphType.WolfMaster,
                SpecialistMorphType.Wedding,
                SpecialistMorphType.DarkAM
            };

            if (sesion.Character.Inventory.CountItem(conf.FullmoonVnum) < conf.FullMoon[e.Upgrade])
            {
                sesion.SendPacket(sesion.Character.GenerateSay(
                    Language.Instance.GetMessageFromKey(string.Format(
                        Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                        ServerManager.GetItem(conf.FullmoonVnum).Name, conf.FullMoon[e.Upgrade])), 10));
                return;
            }

            if (sesion.Character.Inventory.CountItem(conf.FeatherVnum) < conf.Feather[e.Upgrade])
            {
                sesion.SendPacket(sesion.Character.GenerateSay(
                    Language.Instance.GetMessageFromKey(string.Format(
                        Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                        ServerManager.GetItem(conf.FeatherVnum).Name, conf.Feather[e.Upgrade])), 10));
                return;
            }

            if (sesion.Character.Gold < conf.GoldPrice[e.Upgrade])
            {
                sesion.SendPacket(sesion.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"),
                    10));
                return;
            }

            if (e.Upgrade < 5 ? e.SpLevel < 20 : e.Upgrade < 10 ? e.SpLevel < 40 : e.SpLevel < 50)
            {
                sesion.SendPacket(sesion.Character.GenerateSay(string.Format(
                    Language.Instance.GetMessageFromKey("LVL_REQUIRED"),
                    e.Upgrade < 5 ? 21 : e.Upgrade < 10 ? 41 : 50), 11));
                return;
            }

            if (sesion.Character.Inventory.CountItem(
                    e.Upgrade < 5 ? soulSpecialists.Any(s => s == (SpecialistMorphType) e.Item.Morph)
                        ?
                        conf.GreenSoulVnum
                        : conf.DragonSkinVnum :
                    e.Upgrade < 10 ? soulSpecialists.Any(s => s == (SpecialistMorphType) e.Item.Morph)
                        ?
                        conf.RedSoulVnum
                        : conf.DragonBloodVnum :
                    soulSpecialists.Any(s => s == (SpecialistMorphType) e.Item.Morph) ? conf.BlueSoulVnum :
                    conf.DragonHeartVnum) <
                conf.Soul[e.Upgrade])
            {
                sesion.SendPacket(sesion.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(
                    Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                    ServerManager.GetItem(
                        e.Upgrade < 5 ? soulSpecialists.Any(s => s == (SpecialistMorphType) e.Item.Morph)
                            ?
                            conf.GreenSoulVnum
                            : conf.DragonSkinVnum :
                        e.Upgrade < 10 ? soulSpecialists.Any(s => s == (SpecialistMorphType) e.Item.Morph)
                            ?
                            conf.RedSoulVnum
                            : conf.DragonBloodVnum :
                        soulSpecialists.Any(s => s == (SpecialistMorphType) e.Item.Morph) ? conf.BlueSoulVnum :
                        conf.DragonHeartVnum).Name,
                    conf.Soul[e.Upgrade])), 10));
                return;
            }

            if (protect == UpgradeProtection.Protected)
            {
                if (sesion.Character.Inventory.CountItem(e.Upgrade < 10
                        ? conf.BlueScrollVnum
                        : conf.RedScrollVnum) < 1)
                {
                    sesion.SendPacket(sesion.Character.GenerateSay(
                        Language.Instance.GetMessageFromKey(string.Format(
                            Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                            ServerManager.GetItem(e.Upgrade < 10
                                ? conf.BlueScrollVnum
                                : conf.RedScrollVnum).Name, 1)), 10));
                    return;
                }

                sesion.Character.Inventory.RemoveItemAmount(e.Upgrade < 10
                    ? conf.BlueScrollVnum
                    : conf.RedScrollVnum);
                sesion.SendPacket("shop_end 2");
            }

            sesion.GoldLess(conf.GoldPrice[e.Upgrade]);

            // remove feather and fullmoon before upgrading
            sesion.Character.Inventory.RemoveItemAmount(conf.FeatherVnum, conf.Feather[e.Upgrade]);
            sesion.Character.Inventory.RemoveItemAmount(conf.FullmoonVnum, conf.FullMoon[e.Upgrade]);

            var wearable = sesion.Character.Inventory.GetItemInstanceById(e.Id);
            var inventory = sesion.Character.Inventory.GetItemInstanceById(e.Id);
            var rnd = ServerManager.RandomNumber();
            if (ServerManager.Instance.Configuration.EventSpUp != 0)
            {
                rnd += (rnd / 100) * ServerManager.Instance.Configuration.EventSpUp;
            }
            if (rnd < conf.Destroy[e.Upgrade])
            {
                if (protect == UpgradeProtection.Protected || protect == UpgradeProtection.Event)
                {
                    sesion.CurrentMapInstance.Broadcast(
                        StaticPacketHelper.GenerateEff(UserType.Player, sesion.Character.CharacterId, 3004),
                        sesion.Character.MapX, sesion.Character.MapY);
                    sesion.SendPacket(
                        sesion.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADESP_FAILED_SAVED"),
                            11));
                    sesion.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADESP_FAILED_SAVED"),
                            0));
                }
                else
                {
                    if (e.Upgrade < 5)
                    {
                        sesion.Character.Inventory.RemoveItemAmount(
                                soulSpecialists.Any(s => s == (SpecialistMorphType) e.Item.Morph)
                                        ? conf.GreenSoulVnum
                                        : conf.DragonSkinVnum, conf.Soul[e.Upgrade]);
                    }
                    else if (e.Upgrade < 10)
                    {
                        sesion.Character.Inventory.RemoveItemAmount(
                                soulSpecialists.Any(s => s == (SpecialistMorphType) e.Item.Morph)
                                        ? conf.RedSoulVnum
                                        : conf.DragonBloodVnum, conf.Soul[e.Upgrade]);
                    }
                    else if (e.Upgrade < 15)
                    {
                        sesion.Character.Inventory.RemoveItemAmount(
                                soulSpecialists.Any(s => s == (SpecialistMorphType) e.Item.Morph)
                                        ? conf.BlueSoulVnum
                                        : conf.DragonHeartVnum, conf.Soul[e.Upgrade]);
                    }

                    wearable.Rare = -2;
                    sesion.SendPacket(
                        sesion.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADESP_DESTROYED"), 11));
                    sesion.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADESP_DESTROYED"), 0));
                    sesion.SendPacket(wearable.GenerateInventoryAdd());
                }
            }
            else if (rnd < conf.UpFail[e.Upgrade])
            {
                if (protect == UpgradeProtection.Protected || protect == UpgradeProtection.Event)
                {
                    sesion.CurrentMapInstance.Broadcast(
                        StaticPacketHelper.GenerateEff(UserType.Player, sesion.Character.CharacterId, 3004),
                        sesion.Character.MapX, sesion.Character.MapY);
                }
                else
                {
                    if (e.Upgrade < 5)
                    {
                        sesion.Character.Inventory.RemoveItemAmount(
                                soulSpecialists.Any(s => s == (SpecialistMorphType) e.Item.Morph)
                                        ? conf.GreenSoulVnum
                                        : conf.DragonSkinVnum, conf.Soul[e.Upgrade]);
                    }
                    else if (e.Upgrade < 10)
                    {
                        sesion.Character.Inventory.RemoveItemAmount(
                                soulSpecialists.Any(s => s == (SpecialistMorphType) e.Item.Morph)
                                        ? conf.RedSoulVnum
                                        : conf.DragonBloodVnum, conf.Soul[e.Upgrade]);
                    }
                    else if (e.Upgrade < 15)
                    {
                        sesion.Character.Inventory.RemoveItemAmount(
                                soulSpecialists.Any(s => s == (SpecialistMorphType) e.Item.Morph)
                                        ? conf.BlueSoulVnum
                                        : conf.DragonHeartVnum, conf.Soul[e.Upgrade]);
                    }
                }

                sesion.SendPacket(sesion.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADESP_FAILED"),
                    11));
                sesion.SendPacket(
                    UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADESP_FAILED"), 0));
            }
            else
            {
                if (protect == UpgradeProtection.Protected || protect == UpgradeProtection.Event)
                {
                    sesion.CurrentMapInstance.Broadcast(
                            StaticPacketHelper.GenerateEff(UserType.Player, sesion.Character.CharacterId, 3004),
                            sesion.Character.MapX, sesion.Character.MapY);
                }

                if (e.Upgrade < 5)
                {
                    sesion.Character.Inventory.RemoveItemAmount(
                            soulSpecialists.Any(s => s == (SpecialistMorphType) e.Item.Morph)
                                    ? conf.GreenSoulVnum
                                    : conf.DragonSkinVnum, conf.Soul[e.Upgrade]);
                }
                else if (e.Upgrade < 10)
                {
                    sesion.Character.Inventory.RemoveItemAmount(
                            soulSpecialists.Any(s => s == (SpecialistMorphType) e.Item.Morph)
                                    ? conf.RedSoulVnum
                                    : conf.DragonBloodVnum, conf.Soul[e.Upgrade]);
                }
                else if (e.Upgrade < 15)
                {
                    sesion.Character.Inventory.RemoveItemAmount(
                            soulSpecialists.Any(s => s == (SpecialistMorphType) e.Item.Morph)
                                    ? conf.BlueSoulVnum
                                    : conf.DragonHeartVnum, conf.Soul[e.Upgrade]);
                }

                sesion.CurrentMapInstance.Broadcast(
                               StaticPacketHelper.GenerateEff(UserType.Player, sesion.Character.CharacterId, 3005),
                               sesion.Character.MapX, sesion.Character.MapY);
                sesion.SendPacket(sesion.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADESP_SUCCESS"),
                    12));
                sesion.SendPacket(
                    UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADESP_SUCCESS"), 0));
                wearable.Upgrade++;
                if (wearable.Upgrade > 8)
                {
                    sesion.Character.Family?.InsertFamilyLog(FamilyLogType.ItemUpgraded, sesion.Character.Name, itemVNum: wearable.ItemVNum, upgrade: wearable.Upgrade);

                }

                sesion.SendPacket(wearable.GenerateInventoryAdd());
            }

            sesion.SendPacket(sesion.Character.GenerateGold());
            sesion.SendPacket(sesion.Character.GenerateEq());
            sesion.SendPacket("shop_end 1");
        }

        #endregion
    }
}