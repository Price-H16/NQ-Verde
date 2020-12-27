using System;
using System.Collections.Generic;
using System.Linq;
using ChickenAPI.Enums.Game.Character;
using NosTale.Extension.Extension.Packet;
using NosTale.Packets.Packets.ServerPackets;
using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extensions;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Npc
{
    public class BuyPacketHandler : IPacketHandler
    {

        #region Properties

        public BuyPacketHandler(ClientSession session)
        {
            Session = session;
        }
        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void BuyShop(BuyPacket buyPacket)
        {
            if (Session.Character.InExchangeOrTrade)
            {
                return;
            }

            short amount = buyPacket.Amount;

            switch (buyPacket.Type)
            {
                case BuyShopType.CharacterShop:
                    if (!Session.HasCurrentMapInstance)
                    {
                        return;
                    }

                    KeyValuePair<long, MapShop> shop =
                        Session.CurrentMapInstance.UserShops.FirstOrDefault(mapshop =>
                            mapshop.Value.OwnerId.Equals(buyPacket.OwnerId));
                    PersonalShopItem item = shop.Value?.Items.Find(i => i.ShopSlot.Equals(buyPacket.Slot));
                    ClientSession sess = ServerManager.Instance.GetSessionByCharacterId(shop.Value?.OwnerId ?? 0);
                    if (sess == null || item == null || amount <= 0 || amount > 9999)
                    {
                        return;
                    }

                    Logger.LogUserEvent("ITEM_BUY_PLAYERSHOP", Session.GenerateIdentity(),
                        $"From: {buyPacket.OwnerId} IIId: {item.ItemInstance.Id} ItemVNum: {item.ItemInstance.ItemVNum} Amount: {buyPacket.Amount} PricePer: {item.Price}");

                    if (amount > item.SellAmount)
                    {
                        amount = item.SellAmount;
                    }

                    if ((item.Price * amount)
                        + sess.Character.Gold
                        > ServerManager.Instance.Configuration.MaxGold)
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateShopMemo(3,
                            Language.Instance.GetMessageFromKey("MAX_GOLD")));
                        return;
                    }

                    if (item.Price * amount >= Session.Character.Gold)
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateShopMemo(3,
                            Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY")));
                        return;
                    }

                    // check if the item has been removed successfully from previous owner and remove it
                    if (BuyValidate(Session, shop, buyPacket.Slot, amount))
                    {
                        Session.Character.Gold -= item.Price * amount;
                        Session.SendPacket(Session.Character.GenerateGold());

                        KeyValuePair<long, MapShop> shop2 =
                            Session.CurrentMapInstance.UserShops.FirstOrDefault(s =>
                                s.Value.OwnerId.Equals(buyPacket.OwnerId));
                        LoadShopItem(buyPacket.OwnerId, shop2);
                    }
                    else
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"),
                                0));
                    }

                    break;


                case BuyShopType.ItemShop:
                    if (!Session.HasCurrentMapInstance)
                    {
                        return;
                    }

                    MapNpc npc =
                        Session.CurrentMapInstance.Npcs.Find(n => n.MapNpcId.Equals((int)buyPacket.OwnerId));
                    if (npc != null)
                    {
                        int dist = Map.GetDistance(
                            new MapCell { X = Session.Character.PositionX, Y = Session.Character.PositionY },
                            new MapCell { X = npc.MapX, Y = npc.MapY });
                        if (npc.Shop == null || dist > 5)
                        {
                            return;
                        }

                        if (npc.Shop.ShopSkills.Count > 0)
                        {
                            if (!npc.Shop.ShopSkills.Exists(s => s.SkillVNum == buyPacket.Slot))
                            {
                                return;
                            }

                            // skill shop
                            if (Session.Character.UseSp)
                            {
                                Session.SendPacket(
                                    UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("REMOVE_SP"),
                                        0));
                                return;
                            }

                            if (Session.Character.Skills.Any(s =>
                                s.LastUse.AddMilliseconds(s.Skill.Cooldown * 100) > DateTime.Now))
                            {
                                Session.SendPacket(
                                    UserInterfaceHelper.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("SKILL_NEED_COOLDOWN"), 0));
                                return;
                            }

                            Skill skillinfo = ServerManager.GetSkill(buyPacket.Slot);
                            if (Session.Character.Skills.Any(s => s.SkillVNum == buyPacket.Slot) || skillinfo == null)
                            {
                                return;
                            }

                            Logger.LogUserEvent("SKILL_BUY", Session.GenerateIdentity(),
                                $"SkillVNum: {skillinfo.SkillVNum} Price: {skillinfo.Price}");

                            if (Session.Character.Gold < skillinfo.Price)
                            {
                                Session.SendPacket(
                                    UserInterfaceHelper.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 0));
                            }
                            else if (Session.Character.GetCP() < skillinfo.CPCost)
                            {
                                Session.SendPacket(
                                    UserInterfaceHelper.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("NOT_ENOUGH_CP"), 0));
                            }
                            else
                            {
                                if (skillinfo.SkillVNum < 200)
                                {
                                    int skillMiniumLevel = 0;
                                    if (skillinfo.MinimumSwordmanLevel == 0 && skillinfo.MinimumArcherLevel == 0
                                                                            && skillinfo.MinimumMagicianLevel == 0)
                                    {
                                        skillMiniumLevel = skillinfo.MinimumAdventurerLevel;
                                    }
                                    else
                                    {
                                        switch (Session.Character.Class)
                                        {
                                            case CharacterClassType.Adventurer:
                                                skillMiniumLevel = skillinfo.MinimumAdventurerLevel;
                                                break;

                                            case CharacterClassType.Swordsman:
                                                skillMiniumLevel = skillinfo.MinimumSwordmanLevel;
                                                break;

                                            case CharacterClassType.Archer:
                                                skillMiniumLevel = skillinfo.MinimumArcherLevel;
                                                break;

                                            case CharacterClassType.Magician:
                                                if (skillinfo.MinimumMagicianLevel > 0)
                                                {
                                                    skillMiniumLevel = skillinfo.MinimumMagicianLevel;
                                                }
                                                else
                                                {
                                                    skillMiniumLevel = skillinfo.MinimumAdventurerLevel;
                                                }
                                                break;
                                        }
                                    }

                                    if (skillMiniumLevel == 0)
                                    {
                                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                            Language.Instance.GetMessageFromKey("SKILL_CANT_LEARN"), 0));
                                        return;
                                    }

                                    if (Session.Character.Level < skillMiniumLevel)
                                    {
                                        Session.SendPacket(
                                            UserInterfaceHelper.GenerateMsg(
                                                Language.Instance.GetMessageFromKey("LOW_LVL"), 0));
                                        return;
                                    }

                                    foreach (CharacterSkill skill in Session.Character.Skills.GetAllItems())
                                    {
                                        if (skillinfo.CastId == skill.Skill.CastId && skill.Skill.SkillVNum < 200)
                                        {
                                            Session.Character.Skills.Remove(skill.SkillVNum);
                                        }
                                    }
                                }
                                else
                                {
                                    if ((byte)Session.Character.Class != skillinfo.Class)
                                    {
                                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                            Language.Instance.GetMessageFromKey("SKILL_CANT_LEARN"), 0));
                                        return;
                                    }

                                    if (Session.Character.JobLevel < skillinfo.LevelMinimum)
                                    {
                                        Session.SendPacket(
                                            UserInterfaceHelper.GenerateMsg(
                                                Language.Instance.GetMessageFromKey("LOW_JOB_LVL"), 0));
                                        return;
                                    }

                                    if (skillinfo.UpgradeSkill != 0)
                                    {
                                        CharacterSkill oldupgrade = Session.Character.Skills.FirstOrDefault(s =>
                                            s.Skill.UpgradeSkill == skillinfo.UpgradeSkill
                                            && s.Skill.UpgradeType == skillinfo.UpgradeType &&
                                            s.Skill.UpgradeSkill != 0);
                                        if (oldupgrade != null)
                                        {
                                            Session.Character.Skills.Remove(oldupgrade.SkillVNum);
                                        }
                                    }
                                }



                                Session.Character.Skills[buyPacket.Slot] = new CharacterSkill
                                {
                                    SkillVNum = buyPacket.Slot,
                                    CharacterId = Session.Character.CharacterId
                                };

                                Session.Character.Gold -= skillinfo.Price;
                                Session.SendPacket(Session.Character.GenerateGold());
                                Session.SendPacket(Session.Character.GenerateSki());
                                Session.SendPackets(Session.Character.GenerateQuicklist());
                                Session.SendPacket(
                                    UserInterfaceHelper.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("SKILL_LEARNED"), 0));
                                Session.SendPacket(Session.Character.GenerateLev());
                                Session.SendPackets(Session.Character.GenerateStatChar());
                            }
                        }
                        else if (npc.Shop.ShopItems.Count > 0)
                        {
                            // npc shop
                            ShopItemDTO shopItem = npc.Shop.ShopItems.Find(it => it.Slot == buyPacket.Slot);
                            if (shopItem == null || amount <= 0 || amount > 999)
                            {
                                return;
                            }

                            Item iteminfo = ServerManager.GetItem(shopItem.ItemVNum);
                            long price = iteminfo.Price * amount;
                            long reputprice = iteminfo.ReputPrice * amount;
                            double percent;
                            switch (Session.Character.GetDignityIco())
                            {
                                case 3:
                                    percent = 1.10;
                                    break;

                                case 4:
                                    percent = 1.20;
                                    break;

                                case 5:
                                case 6:
                                    percent = 1.5;
                                    break;

                                default:
                                    percent = 1;
                                    break;
                            }

                            Logger.LogUserEvent("ITEM_BUY_NPCSHOP", Session.GenerateIdentity(),
                                $"From: {npc.MapNpcId} ItemVNum: {iteminfo.VNum} Amount: {buyPacket.Amount} PricePer: {price * percent} ");

                            sbyte rare = shopItem.Rare;
                            if (iteminfo.Type == 0)
                            {
                                amount = 1;
                            }

                            if (iteminfo.ReputPrice == 0)
                            {
                                if (price < 0 || price * percent > Session.Character.Gold)
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateShopMemo(3,
                                        Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY")));
                                    return;
                                }
                            }
                            else
                            {
                                if (reputprice <= 0 || reputprice > Session.Character.Reputation)
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateShopMemo(3,
                                        Language.Instance.GetMessageFromKey("NOT_ENOUGH_REPUT")));
                                    return;
                                }

                                byte ra = (byte)ServerManager.RandomNumber();

                                if (iteminfo.ReputPrice != 0)
                                {
                                    for (int i = 0; i < ItemHelper.BuyCraftRareRate.Length; i++)
                                    {
                                        if (ra <= ItemHelper.BuyCraftRareRate[i])
                                        {
                                            rare = (sbyte)i;
                                        }
                                    }
                                }
                            }

                            List<ItemInstance> newItems = Session.Character.Inventory.AddNewToInventory(
                                shopItem.ItemVNum, amount, Rare: rare, Upgrade: shopItem.Upgrade,
                                Design: shopItem.Color);
                            if (newItems.Count == 0)
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateShopMemo(3,
                                    Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE")));
                                return;
                            }

                            if (newItems.Count > 0)
                            {
                                foreach (ItemInstance itemInst in newItems)
                                {
                                    switch (itemInst.Item.EquipmentSlot)
                                    {
                                        case EquipmentType.Armor:
                                        case EquipmentType.MainWeapon:
                                        case EquipmentType.SecondaryWeapon:
                                            itemInst.SetRarityPoint();
                                            if (iteminfo.ReputPrice > 0)
                                            {
                                                itemInst.BoundCharacterId = Session.Character.CharacterId;
                                            }
                                            break;

                                        case EquipmentType.Boots:
                                        case EquipmentType.Gloves:
                                            itemInst.FireResistance =
                                                (short)(itemInst.Item.FireResistance * shopItem.Upgrade);
                                            itemInst.DarkResistance =
                                                (short)(itemInst.Item.DarkResistance * shopItem.Upgrade);
                                            itemInst.LightResistance =
                                                (short)(itemInst.Item.LightResistance * shopItem.Upgrade);
                                            itemInst.WaterResistance =
                                                (short)(itemInst.Item.WaterResistance * shopItem.Upgrade);
                                            break;
                                    }
                                }

                                if (iteminfo.ReputPrice == 0)
                                {
                                    if (price == 0)
                                    {
                                        Session.SendPacket(Session.Character.GenerateSay($"This item is Price: 0! Please contact some Admin", 10));
                                        return;
                                    }
                                    if (Session.Character.MapInstance.MapInstanceType == MapInstanceType.ShopShip)
                                    {
                                        if (Session.Character.ItemShopShip == 10)
                                        {
                                            Session.SendPacket(Session.Character.GenerateSay($"You buy 10/10 items, cant buy more!", 10));
                                            return;
                                        }

                                        if ((Session.Character.ItemShopShip + amount) > 10)
                                        {
                                            Session.SendPacket(Session.Character.GenerateSay($"Remember, you can buy only 10 items", 10));
                                            return;
                                        }

                                        Session.SendPacket(Session.Character.GenerateSay($"You buy {Session.Character.ItemShopShip}/10 items, cant buy more!", 10));
                                        Session.Character.ItemShopShip += amount;
                                    }
                                    Session.SendPacket(UserInterfaceHelper.GenerateShopMemo(1,
                                        string.Format(Language.Instance.GetMessageFromKey("BUY_ITEM_VALID"),
                                            iteminfo.Name, amount)));
                                    Session.Character.Gold -= (long)(price * percent);
                                    Session.SendPacket(Session.Character.GenerateGold());
                                }
                                else
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateShopMemo(1,
                                        string.Format(Language.Instance.GetMessageFromKey("BUY_ITEM_VALID"),
                                            iteminfo.Name, amount)));
                                    Session.Character.Reputation -= reputprice;
                                    Session.SendPacket(Session.Character.GenerateFd());
                                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateIn(InEffect: 1), ReceiverType.AllExceptMe);
                                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateGidx(), ReceiverType.AllExceptMe);
                                    Session.SendPacket(
                                        Session.Character.GenerateSay(
                                            string.Format(Language.Instance.GetMessageFromKey("REPUT_DECREASED"), reputprice), 11));
                                }
                                //FAMILYEXTENSIONS
                                if (iteminfo.VNum > 9678 && iteminfo.VNum < 9698)
                                {
                                    if (Session.Character.Family?.FamilyLevel >= FamilySystemHelper.GetMissValues(iteminfo.VNum)[1])
                                    {
                                        if (Session.Character.FamilyCharacter.Authority == FamilyAuthority.Head)
                                        {
                                            if (Session.Character.Family.FamilySkillMissions.Any(s => s.ItemVNum == iteminfo.VNum))
                                            {
                                                Session.SendPacket(UserInterfaceHelper.GenerateInfo("You can't buy this!"));
                                                return;
                                            }
                                            else if (Session.Character.Family.FamilySkillMissions.Any(s => s.ItemVNum == iteminfo.VNum - 1) || FamilySystemHelper.IsBase(iteminfo.VNum))
                                            {
                                                if (!(Session.Character.Gold < price))
                                                {
                                                    Session.Character.Gold -= price;
                                                    Session.SendPacket(Session.Character.GenerateGold());
                                                    Session.Character.Family.AddStaticExtension(iteminfo.VNum);
                                                    Session.Character.Family.SendPacket(UserInterfaceHelper.GenerateMsg("Your Family successfully purchased " + iteminfo.Name, 10));
                                                    ServerManager.Instance.FamilyRefresh(Session.Character.Family.FamilyId);
                                                    GenerateNInvForExtensions(npc.MapNpcId);
                                                }
                                                return;
                                            }
                                            else
                                            {
                                                Session.SendPacket(UserInterfaceHelper.GenerateInfo("You need to buy previous extension of the same type!"));
                                            }

                                        }
                                        else
                                            Session.SendPacket(UserInterfaceHelper.GenerateInfo("You're not family head!"));
                                    }
                                    else
                                        Session.SendPacket(UserInterfaceHelper.GenerateInfo("Too low family level!"));
                                    return;
                                }
                            }
                            else
                            {
                                Session.SendPacket(
                                    UserInterfaceHelper.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"), 0));
                            }
                        }
                    }

                    break;
            }
        }

        public bool BuyValidate(ClientSession clientSession, KeyValuePair<long, MapShop> shop, short slot, long amount)
        {
            if (!clientSession.HasCurrentMapInstance)
            {
                return false;
            }

            PersonalShopItem shopitem = clientSession.CurrentMapInstance.UserShops[shop.Key].Items
                .Find(i => i.ShopSlot.Equals(slot));
            if (shopitem == null)
            {
                return false;
            }

            Guid id = shopitem.ItemInstance.Id;

            ClientSession shopOwnerSession = ServerManager.Instance.GetSessionByCharacterId(shop.Value.OwnerId);
            if (shopOwnerSession == null)
            {
                return false;
            }

            if (amount > shopitem.SellAmount)
            {
                amount = shopitem.SellAmount;
            }

            ItemInstance sellerItem = shopOwnerSession.Character.Inventory.GetItemInstanceById(id);
            if (sellerItem == null || sellerItem.Amount < amount)
            {
                return false;
            }

            List<ItemInstance> inv = shopitem.ItemInstance.Type == InventoryType.Equipment
                ? clientSession.Character.Inventory.AddToInventory(shopitem.ItemInstance)
                : clientSession.Character.Inventory.AddNewToInventory(shopitem.ItemInstance.ItemVNum, (short)amount,
                    shopitem.ItemInstance.Type);

            if (inv.Count == 0)
            {
                return false;
            }

            shopOwnerSession.Character.Gold += shopitem.Price * amount;
            shopOwnerSession.SendPacket(shopOwnerSession.Character.GenerateGold());
            shopOwnerSession.SendPacket(UserInterfaceHelper.GenerateShopMemo(1,
                string.Format(Language.Instance.GetMessageFromKey("BUY_ITEM"), Session.Character.Name,
                    shopitem.ItemInstance.Item.Name, amount)));
            clientSession.CurrentMapInstance.UserShops[shop.Key].Sell += shopitem.Price * amount;

            if (shopitem.ItemInstance.Type != InventoryType.Equipment)
            {
                // remove sold amount of items
                shopOwnerSession.Character.Inventory.RemoveItemFromInventory(id, (short)amount);

                // remove sold amount from sellamount
                shopitem.SellAmount -= (short)amount;
            }
            else
            {
                // remove equipment
                shopOwnerSession.Character.Inventory.Remove(shopitem.ItemInstance.Id);

                // send empty slot to owners inventory
                shopOwnerSession.SendPacket(
                    UserInterfaceHelper.Instance.GenerateInventoryRemove(shopitem.ItemInstance.Type,
                        shopitem.ItemInstance.Slot));

                // remove the sell amount
                shopitem.SellAmount = 0;
            }

            // remove item from shop if the amount the user wanted to sell has been sold
            if (shopitem.SellAmount == 0)
            {
                clientSession.CurrentMapInstance.UserShops[shop.Key].Items.Remove(shopitem);
            }

            // update currently sold item
            shopOwnerSession.SendPacket($"sell_list {shop.Value.Sell} {slot}.{amount}.{shopitem.SellAmount}");

            // end shop
            if (!clientSession.CurrentMapInstance.UserShops[shop.Key].Items.Any(s => s.SellAmount > 0))
            {
                shopOwnerSession.Character.CloseShop();
            }

            return true;
        }

        public void GenerateNInvForExtensions(int id)
        {
            MapNpc mapnpc = Session.CurrentMapInstance.Npcs.Find(n => n.MapNpcId.Equals(id));
            if (mapnpc?.Shop == null)
            {
                return;
            }

            string shoplist = "";
            foreach (ShopItemDTO item in mapnpc.Shop.ShopItems.Where(s => s.Type.Equals(0)))
            {
                if (mapnpc.Shop.ShopType == 48)
                {
                    var mv = FamilySystemHelper.GetMissValues(item.ItemVNum);
                    if (mv == null) continue;

                    shoplist += $" {item.ItemVNum}|{(mv[1] > Session.Character.Family?.FamilyLevel ? 2 : Session.Character.Family?.FamilySkillMissions.Any(s => s.ItemVNum == item.ItemVNum) ?? false ? 1 : 0)}|{item.Slot}";
                }
            }

            Session.SendPacket($"n_inv 2 {mapnpc.MapNpcId} 0 {shoplist}");
            Session.SendPacket($"wopen 37 0");



        }

        private void LoadShopItem(long owner, KeyValuePair<long, MapShop> shop)
        {
            string packetToSend = $"n_inv 1 {owner} 0 0";

            if (shop.Value?.Items != null)
            {
                foreach (PersonalShopItem item in shop.Value.Items)
                {
                    if (item != null)
                    {
                        if (item.ItemInstance.Item.Type == InventoryType.Equipment)
                        {
                            packetToSend +=
                                $" 0.{item.ShopSlot}.{item.ItemInstance.ItemVNum}.{item.ItemInstance.Rare}.{item.ItemInstance.Upgrade}.{item.Price}";
                        }
                        else
                        {
                            packetToSend +=
                                $" {(byte)item.ItemInstance.Item.Type}.{item.ShopSlot}.{item.ItemInstance.ItemVNum}.{item.SellAmount}.{item.Price}.-1";
                        }
                    }
                    else
                    {
                        packetToSend += " -1";
                    }
                }
            }

#warning Check this
            packetToSend +=
                " -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1";

            Session.SendPacket(packetToSend);
        }

        #endregion
    }
}