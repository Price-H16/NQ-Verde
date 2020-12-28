using System.Linq;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Npc
{
    public class PdtsePacketHandler : IPacketHandler
    {
        #region Instantiation

        public PdtsePacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        /// <summary>
        /// pdtse packet
        /// </summary>
        /// <param name="pdtsePacket"></param>
        public void Pdtse(PdtsePacket pdtsePacket)
        {
            if (!Session.HasCurrentMapInstance)
            {
                return;
            }

            short vNum = pdtsePacket.VNum;
            Recipe recipe = ServerManager.Instance.GetAllRecipes().Find(s => s.ItemVNum == vNum);
            if (Session.CurrentMapInstance.GetNpc(Session.Character.LastNpcMonsterId)?.Recipes.Find(s => s.ItemVNum == vNum) is Recipe recipeNpc)
            {
                recipe = recipeNpc;
            }
            if (ServerManager.Instance.GetRecipesByItemVNum(Session.Character.LastItemVNum)?.Find(s => s.ItemVNum == vNum) is Recipe recipeScroll)
            {
                recipe = recipeScroll;
            }
            if (pdtsePacket.Type == 1)
            {
                if (recipe?.Amount > 0)
                {
                    string recipePacket = $"m_list 3 {recipe.Amount}";
                    foreach (RecipeItemDTO ite in recipe.Items.Where(s =>
                        s.ItemVNum != Session.Character.LastItemVNum || Session.Character.LastItemVNum == 0))
                    {
                        if (ite.Amount > 0)
                        {
                            recipePacket += $" {ite.ItemVNum} {ite.Amount}";
                        }
                    }

                    recipePacket += " -1";
                    Session.SendPacket(recipePacket);
                }
            }
            else if (recipe != null)
            {
                // sequential
                //pdtse 0 4955 0 0 1
                // random
                //pdtse 0 4955 0 0 2
                if (recipe.Items.Count < 1 || recipe.Amount <= 0)
                {
                    return;
                }
                if (recipe.Items.Any(ite => Session.Character.Inventory.CountItem(ite.ItemVNum) < ite.Amount))
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CRAFT_NOT_POSSIBLE"), 0));
                    return;
                }

                if (Session.Character.LastItemVNum != 0)
                {
                    if (!ServerManager.Instance.ItemHasRecipe(Session.Character.LastItemVNum))
                    {
                        return;
                    }
                    short npcVNum = 0;
                    switch (Session.Character.LastItemVNum)
                    {
                        case 1375:
                            npcVNum = 956;
                            break;
                        case 1376:
                            npcVNum = 957;
                            break;
                        case 1437:
                            npcVNum = 959;
                            break;
                    }
                    if (npcVNum != 0 && ((Session.Character.BattleEntity.IsCampfire(npcVNum) && Session.CurrentMapInstance.GetNpc(Session.Character.LastNpcMonsterId) == null) || Session.CurrentMapInstance.GetNpc(Session.Character.LastNpcMonsterId)?.NpcVNum != npcVNum))
                    {
                        return;
                    }

                    Session.Character.LastItemVNum = 0;
                }
                else if (!ServerManager.Instance.MapNpcHasRecipe(Session.Character.LastNpcMonsterId))
                {
                    return;
                }

                switch (recipe.ItemVNum)
                {
                    case 1805:
                        if (Session.Character.MapId == 1)
                        {
                            int dist = Map.GetDistance(
                                new MapCell { X = Session.Character.PositionX, Y = Session.Character.PositionY },
                                new MapCell { X = 120, Y = 56 });
                            if (dist < 5)
                            {
                                GameObject.Event.PTS.GeneratePTS(recipe.ItemVNum, Session);
                                foreach (RecipeItemDTO ite in recipe.Items)
                                {
                                    Session.Character.Inventory.RemoveItemAmount(1805, 1);
                                }
                            }
                            else
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateInfo("Return to Nosville"));
                            }
                        }

                        break;

                    /*case 1824:
                        if (Session.Character.MapId == 5)
                        {
                            int dist = Map.GetDistance(
                                new MapCell { X = Session.Character.PositionX, Y = Session.Character.PositionY },
                                new MapCell { X = 120, Y = 56 });
                            if (dist < 5)
                            {
                                GameObject.Event.PTS.GeneratePTS(recipe.ItemVNum, Session);
                                foreach (RecipeItemDTO ite in recipe.Items)
                                {
                                    Session.Character.Inventory.RemoveItemAmount(1824, 1);
                                }
                            }
                            else
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateInfo("Return to Nosville"));
                            }
                        }

                        break;*/

                    case 2802:
                        if (Session.Character.Inventory.CountItem(recipe.ItemVNum) >= 5)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ALREADY_HAVE_MAX_AMOUNT"), 0));
                            return;
                        }
                        break;
                    case 5935:
                        if (Session.Character.Inventory.CountItem(recipe.ItemVNum) >= 3)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ALREADY_HAVE_MAX_AMOUNT"), 0));
                            return;
                        }
                        break;
                    case 5936:
                    case 5937:
                    case 5938:
                    case 5939:
                        if (Session.Character.Inventory.CountItem(recipe.ItemVNum) >= 10)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ALREADY_HAVE_MAX_AMOUNT"), 0));
                            return;
                        }
                        break;
                    case 5940:
                        if (Session.Character.Inventory.CountItem(recipe.ItemVNum) >= 4)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ALREADY_HAVE_MAX_AMOUNT"), 0));
                            return;
                        }
                        break;
                    case 5942:
                    case 5943:
                    case 5944:
                    case 5945:
                    case 5946:
                        if (Session.Character.Inventory.CountItem(recipe.ItemVNum) >= 1)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ALREADY_HAVE_MAX_AMOUNT"), 0));
                            return;
                        }
                        break;
                }

                Item item = ServerManager.GetItem(recipe.ItemVNum);
                if (item == null)
                {
                    return;
                }

                sbyte rare = 0;
                if (item.EquipmentSlot == EquipmentType.Armor
                    || item.EquipmentSlot == EquipmentType.MainWeapon
                    || item.EquipmentSlot == EquipmentType.SecondaryWeapon)
                {
                    byte ra = (byte)ServerManager.RandomNumber();

                    for (int i = 0; i < ItemHelper.BuyCraftRareRate.Length; i++)
                    {
                        if (ra <= ItemHelper.BuyCraftRareRate[i])
                        {
                            rare = (sbyte)i;
                        }
                    }
                }

                ItemInstance inv = Session.Character.Inventory.AddNewToInventory(recipe.ItemVNum, recipe.Amount, Rare: rare)
                    .FirstOrDefault();
                if (inv != null)
                {
                    if (inv.Item.EquipmentSlot == EquipmentType.Armor
                        || inv.Item.EquipmentSlot == EquipmentType.MainWeapon
                        || inv.Item.EquipmentSlot == EquipmentType.SecondaryWeapon)
                    {
                        inv.SetRarityPoint();
                        if (inv.Item.IsHeroic)
                        {
                            inv.GenerateHeroicShell(RarifyProtection.None, true);
                            inv.BoundCharacterId = Session.Character.CharacterId;
                        }
                    }

                    foreach (RecipeItemDTO ite in recipe.Items)
                    {
                        Session.Character.Inventory.RemoveItemAmount(ite.ItemVNum, ite.Amount);
                    }                  

                    // pdti {WindowType} {inv.ItemVNum} {recipe.Amount} {Unknown} {inv.Upgrade} {inv.Rare}
                    Session.SendPacket($"pdti 11 {inv.ItemVNum} {recipe.Amount} 29 {inv.Upgrade} {inv.Rare}");
                    Session.SendPacket(UserInterfaceHelper.GenerateGuri(19, 1, Session.Character.CharacterId, 1324));
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        string.Format(Language.Instance.GetMessageFromKey("CRAFTED_OBJECT"), inv.Item.Name,
                            recipe.Amount), 0));
                    Session.Character.IncrementQuests(QuestType.Product, inv.ItemVNum, recipe.Amount);
                }
                else
                {
                    Session.SendPacket("shop_end 0");
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(
                            Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"), 0));
                }
            }
        }

        #endregion
    }
}