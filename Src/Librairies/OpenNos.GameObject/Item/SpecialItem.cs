using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using ChickenAPI.Enums.Game.BCard;
using ChickenAPI.Enums.Game.Buffs;
using OpenNos.Core;
using OpenNos.Core.Extensions;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Extension.Inventory;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;

namespace OpenNos.GameObject
{
    public class SpecialItem : Item
    {
        #region Instantiation

        public SpecialItem(ItemDTO item) : base(item)
        {
        }

        #endregion

        #region Methods

        public override void Use(ClientSession session, ref ItemInstance inv, byte Option = 0,
            string[] packetsplit = null)
        {
            var itemDesign = inv.Design;

            #region BoxItem

            var boxItemDTOs = ServerManager.Instance.BoxItems.Where(boxItem =>
                boxItem.OriginalItemVNum == VNum && boxItem.OriginalItemDesign == itemDesign).ToList();

            if (boxItemDTOs.Any())
            {
                session.Character.Inventory.RemoveItemFromInventory(inv.Id);

                foreach (var boxItemDTO in boxItemDTOs)
                {
                    if (ServerManager.RandomNumber() < boxItemDTO.Probability)
                    {
                        session.Character.GiftAdd(boxItemDTO.ItemGeneratedVNum, boxItemDTO.ItemGeneratedAmount,
                                boxItemDTO.ItemGeneratedRare, boxItemDTO.ItemGeneratedUpgrade,
                                boxItemDTO.ItemGeneratedDesign);
                    }
                }

                return;
            }

            #endregion

            if (session.CurrentMapInstance.MapInstanceType == MapInstanceType.RainbowBattleInstance)
            {
                return;
            }

            if (session.Character.IsVehicled && Effect != 1000)
            {
                if (VNum == 5119 || VNum == 9071) // Speed Booster
                {
                    if (!session.Character.Buff.Any(s => s.Card.CardId == 336))
                    {
                        session.Character.VehicleItem.BCards.ForEach(s =>
                            s.ApplyBCards(session.Character.BattleEntity, session.Character.BattleEntity));
                        session.CurrentMapInstance.Broadcast($"eff 1 {session.Character.CharacterId} 885");
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                }
                else
                {
                    session.SendPacket(
                        session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_DO_VEHICLED"), 10));
                }

                return;
            }



            if (VNum == 5511)
            {
                session.Character.GeneralLogs.Where(s => s.LogType == "InstanceEntry" && (short.Parse(s.LogData) == 16 || short.Parse(s.LogData) == 17) && s.Timestamp.Date == DateTime.Today).ToList().ForEach(s =>
                {
                    s.LogType = "NulledInstanceEntry";
                    DAOFactory.GeneralLogDAO.InsertOrUpdate(ref s);
                });
                session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                return;
            }

            if (session.CurrentMapInstance?.MapInstanceType != MapInstanceType.TalentArenaMapInstance
            && (VNum == 5936 || VNum == 5937 || VNum == 5938 || VNum == 5939 || VNum == 5940 || VNum == 5942 || VNum == 5943 || VNum == 5944 || VNum == 5945 || VNum == 5946))
            {
                return;
            }

            if (BCards.Count > 0 && Effect != 1000)
            {
                if (BCards.Any(s => s.Type == (byte)BCardType.CardType.Buff && s.SubType == 11 && new Buff((short)s.SecondData, session.Character.Level).Card.BCards.Any(newbuff => session.Character.Buff.GetAllItems().Any(b => b.Card.BCards.Any(buff => buff.CardId != newbuff.CardId && (buff.Type == 33 && buff.SubType == 51 && (newbuff.Type == 33 || newbuff.Type == 58) || newbuff.Type == 33 && newbuff.SubType == 51 && (buff.Type == 33 || buff.Type == 58) || buff.Type == 33 && (buff.SubType == 11 || buff.SubType == 31) && newbuff.Type == 58 && newbuff.SubType == 11 || buff.Type == 33 && (buff.SubType == 21 || buff.SubType == 41) && newbuff.Type == 58 && newbuff.SubType == 31 || newbuff.Type == 33 && (newbuff.SubType == 11 || newbuff.SubType == 31) && buff.Type == 58 && buff.SubType == 11 || newbuff.Type == 33 && (newbuff.SubType == 21 || newbuff.SubType == 41) && buff.Type == 58 && buff.SubType == 31 || buff.Type == 33 && newbuff.Type == 33 && buff.SubType == newbuff.SubType || buff.Type == 58 && newbuff.Type == 58 && buff.SubType == newbuff.SubType))))))
                {
                    return;
                }

                BCards.ForEach(c => c.ApplyBCards(session.Character.BattleEntity, session.Character.BattleEntity));
                session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                return;
            }

            switch (Effect)
            {

                //seal mini game
                case 1717:
                    switch (EffectValue)
                    {
                        case 1: // King 
                            break;

                        case 2: // sheep
                            session.SendPacket(UserInterfaceHelper.GenerateSay("Sheep game will start in a few seconds", 10));
                            EventHelper.GenerateEvent(EventType.SHEEPGAME);
                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                            break;

                        case 3: // meteor
                            session.SendPacket(UserInterfaceHelper.GenerateSay("Meteorite game will start in a few seconds", 10));
                            EventHelper.GenerateEvent(EventType.METEORITEGAME);
                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                            break;
                    }
                    Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(observer =>
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateBSInfo(2, 4, 0, 0));
                    });
                    break;


                case 1337:
                {
                    // Pick the first item in inventory
                    var item = session.Character.Inventory.LoadBySlotAndType(0, InventoryType.Equipment);
                    item.DownGradeRarity(session);

                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                }
                break;

                case 605:
                    if (session.Character.StaticBonusList.All(s => s.StaticBonusType != StaticBonusType.Extension))
                    {
                        session.Character.StaticBonusList.Add(new StaticBonusDTO
                        {
                            CharacterId = session.Character.CharacterId,
                            DateEnd = DateTime.Now.AddDays(VNum == 5795 ? 30 : 60),
                            StaticBonusType = StaticBonusType.Extension
                        });
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateExts());
                        session.SendPacket(session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                    }

                    break;

                case 604:
                    session.Character.StaticBonusList.Add(new StaticBonusDTO
                    {
                        CharacterId = session.Character.CharacterId,
                        DateEnd = DateTime.Now.AddYears(1),
                        StaticBonusType = StaticBonusType.Extension
                    });
                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    session.SendPacket(session.Character.GenerateExts());
                    session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                    break;

                case 767:
                    if (session.Character.IsChangeName)
                    {
                        // Already Have Set the value
                        session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CHANGE_NAME_1"), 11));
                        return;
                    }
                    session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CHANGE_NAME_0"), 11));
                    session.Character.IsChangeName = true;
                    break;

                // Honour Medals
                case 69:
                    {
                        session.Character.Reputation += ReputPrice;
                        session.SendPacket(session.Character.GenerateFd());
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("REPUT_INCREASE"), ReputPrice), 11));
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateIn(InEffect: 1), ReceiverType.AllExceptMe);
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateGidx(), ReceiverType.AllExceptMe);
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        break;
                    }


                // TimeSpace Stones
                case 140:
                    if (ServerManager.Instance.ChannelId == 51 || session.Character.MapInstance.MapInstanceType == MapInstanceType.ArenaInstance
                       || session.Character.MapInstance.MapInstanceType == MapInstanceType.CaligorInstance
                       || session.Character.MapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance)
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_PERMITTED"), 0));
                        return;
                    }
                    if (session.CurrentMapInstance.MapInstanceType == MapInstanceType.BaseMapInstance)
                    {
                        if (ServerManager.Instance.TimeSpaces.FirstOrDefault(s => s.Id == EffectValue) is ScriptedInstance timeSpace)
                        {
                            session.Character.EnterInstance(timeSpace);
                            //session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        }

                    }                          
                    break;

                // SP Potions
                case 150:
                case 151:
                {
                    if (session.Character.SpAdditionPoint >= 1000000)
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SP_POINTS_FULL"), 0));
                        break;
                    }

                    session.Character.SpAdditionPoint += EffectValue;

                    if (session.Character.SpAdditionPoint > 1000000)
                    {
                        session.Character.SpAdditionPoint = 1000000;
                    }

                    session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        string.Format(Language.Instance.GetMessageFromKey("SP_POINTSADDED"), EffectValue), 0));
                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    session.SendPacket(session.Character.GenerateSpPoint());
                }
                    break;

                // Specialist Medal
                case 204:
                {
                    if (session.Character.SpPoint >= 10000 && session.Character.SpAdditionPoint >= 1000000)
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SP_POINTS_FULL"), 0));
                        break;
                    }

                    session.Character.SpPoint += EffectValue;

                    if (session.Character.SpPoint > 10000)
                    {
                        session.Character.SpPoint = 10000;
                    }

                    session.Character.SpAdditionPoint += EffectValue * 3;

                    if (session.Character.SpAdditionPoint > 1000000)
                    {
                        session.Character.SpAdditionPoint = 1000000;
                    }

                    session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("SP_POINTSADDEDBOTH"), EffectValue, EffectValue * 3), 0));
                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    session.SendPacket(session.Character.GenerateSpPoint());
                }
                    break;

                // Raid Seals
                case 301:
                    var raidSeal = session.Character.Inventory.LoadBySlotAndType(inv.Slot, InventoryType.Main);

                    if (session.Character.MapInstance.MapInstanceType == MapInstanceType.ArenaInstance) // KekW
                    {
                        session.SendPacket(session.Character.GenerateSay("You can't do this.", 11));
                        return;
                    }

                    if (raidSeal != null)
                    {
                        var raid = ServerManager.Instance.Raids.FirstOrDefault(s => s.Id == raidSeal.Item.EffectValue)?.Copy();


                        if (raid != null)
                        {
                            if (ServerManager.Instance.ChannelId == 51 || session.CurrentMapInstance.MapInstanceType != MapInstanceType.BaseMapInstance)
                            {
                                return;
                            }

                            if (ServerManager.Instance.IsCharacterMemberOfGroup(session.Character.CharacterId))
                            {
                                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("RAID_OPEN_GROUP"), 12));
                                return;
                            }

                            var entries = raid.DailyEntries - session.Character.GeneralLogs.CountLinq(s => s.LogType == "InstanceEntry" && short.Parse(s.LogData) == raid.Id && s.Timestamp.Date == DateTime.Today);

                            if (raid.DailyEntries > 0 && entries <= 0)
                            {
                                session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("INSTANCE_NO_MORE_ENTRIES"), 0));
                                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("INSTANCE_NO_MORE_ENTRIES"), 10));

                                return;
                            }

                            if (session.Character.Level < raid.LevelMinimum)
                            {
                                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("RAID_LEVEL_INCORRECT"), 10));

                                return;
                            }

                            var group = new Group
                            {
                                GroupType = raid.IsGiantTeam ? GroupType.GiantTeam : GroupType.BigTeam,
                                Raid = raid
                            };

                            switch (raid.Id)
                            {
                                //fernon
                                case 25:
                                    group.GroupType = GroupType.BigTeam;
                                    break;

                                case 34:
                                case 20:
                                    group.GroupType = GroupType.GiantTeam;
                                    break;
                            }

                            if (group.JoinGroup(session))
                            {
                                ServerManager.Instance.AddGroup(group);
                                session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                    string.Format(Language.Instance.GetMessageFromKey("RAID_LEADER"),
                                        session.Character.Name), 0));
                                session.SendPacket(session.Character.GenerateSay(
                                    string.Format(Language.Instance.GetMessageFromKey("RAID_LEADER"),
                                        session.Character.Name), 10));
                                session.SendPacket(session.Character.GenerateRaid(2));
                                session.SendPacket(session.Character.GenerateRaid(0));
                                session.SendPacket(session.Character.GenerateRaid(1));
                                session.SendPacket(group.GenerateRdlst());
                                session.Character.Inventory.RemoveItemFromInventory(raidSeal.Id);
                            }

                            if (raid.Label == "Draco")
                            {
                                var amulet = session.Character.Inventory.LoadBySlotAndType((byte)EquipmentType.Amulet, InventoryType.Wear);

                                if (amulet != null)
                                {
                                    if (amulet.ItemVNum != 4503)
                                    {
                                        session.SendPacket($"info {Language.Instance.GetMessageFromKey("NO_AMULET_DRACO")}");
                                        return;
                                    }
                                }
                                else
                                {
                                    session.SendPacket($"info {Language.Instance.GetMessageFromKey("NO_AMULET_DRACO")}");
                                    return;
                                }
                            }

                            if (raid.Label == "Glacerus")
                            {
                                var amulet = session.Character.Inventory.LoadBySlotAndType((byte)EquipmentType.Amulet, InventoryType.Wear);

                                if (amulet != null)
                                {
                                    if (amulet.ItemVNum != 4504)
                                    {
                                        session.SendPacket($"info {Language.Instance.GetMessageFromKey("NO_AMULET_GLACE")}");
                                        return;
                                    }
                                }
                                else
                                {
                                    session.SendPacket($"info {Language.Instance.GetMessageFromKey("NO_AMULET_GLACE")}");
                                    return;
                                }
                            }
                        }
#pragma warning disable 4014
                        DiscordWebhookHelper.DiscordEventRaid($"{session.Character.Name} Opened a seal of Raid {raid.Label}");
                    }

                    break;

                // Partner Suits/Skins
                case 305:
                    var mate = session.Character.Mates.Find(s => s.MateTransportId == int.Parse(packetsplit[3]));
                    if (mate != null && EffectValue == mate.NpcMonsterVNum && mate.Skin == 0)
                    {
                        mate.Skin = Morph;
                        session.SendPacket(mate.GenerateCMode(mate.Skin));
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    break;

                //suction Funnel (Quest Item / QuestId = 1724)
                case 400:
                    if (session.Character == null || session.Character.Quests.All(q => q.QuestId != 1724))
                    {
                        break;
                    }

                    if (session.Character.Quests.FirstOrDefault(q => q.QuestId == 1724) is CharacterQuest kenkoQuest)
                    {
                        var kenko = session.CurrentMapInstance?.Monsters.FirstOrDefault(m => m.MapMonsterId == session.Character.LastNpcMonsterId && m.MonsterVNum > 144 && m.MonsterVNum < 154);

                        if (kenko == null || session.Character.Inventory.CountItem(1174) > 0)
                        {
                            break;
                        }

                        if (session.Character.LastFunnelUse.AddSeconds(30) <= DateTime.Now)
                        {
                            if (kenko.CurrentHp / kenko.MaxHp * 100 < 30)
                            {
                                if (ServerManager.RandomNumber() < 30)
                                {
                                    kenko.SetDeathStatement();
                                    session.Character.MapInstance.Broadcast(StaticPacketHelper.Out(UserType.Monster, kenko.MapMonsterId));
                                    session.Character.Inventory.AddNewToInventory(1174); // Kenko Bead
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("KENKO_CATCHED"), 0));
                                }
                                else
                                {
                                    session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("QUEST_CATCH_FAIL"), 0));
                                }
                                session.Character.LastFunnelUse = DateTime.Now;
                            }
                            else
                            {
                                session.SendPacket( UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("HP_TOO_HIGH"), 0));
                            }
                        }
                    }

                    break;

                // Fairy Booster
                case 250:
                    if (!session.Character.Buff.ContainsKey(131))
                    {
                        session.Character.AddStaticBuff(new StaticBuffDTO {CardId = 131});
                        session.CurrentMapInstance?.Broadcast(session.Character.GeneratePairy());
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), inv.Item.Name), 0));
                        session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3014), session.Character.PositionX, session.Character.PositionY);
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    else
                    {
                        session.SendPacket( UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));
                    }
                    break;

                // Improved Fairy Booster
                case 251:
                    if (!session.Character.Buff.ContainsKey(4045))
                    {
                        session.Character.AddStaticBuff(new StaticBuffDTO { CardId = 4045 });
                        session.CurrentMapInstance?.Broadcast(session.Character.GeneratePairy());
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), inv.Item.Name), 0));
                        session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3014), session.Character.PositionX, session.Character.PositionY);
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));

                    }
                    break;

                // Rainbow Pearl/Magic Eraser
                case 666:
                    if (EffectValue == 1 && byte.TryParse(packetsplit[9], out var islot))
                    {
                        var wearInstance = session.Character.Inventory.LoadBySlotAndType(islot, InventoryType.Equipment);

                        if (wearInstance != null &&
                            (wearInstance.Item.ItemType == ItemType.Weapon || wearInstance.Item.ItemType == ItemType.Armor) && wearInstance.ShellEffects.Count != 0 && !wearInstance.Item.IsHeroic)
                        {
                            wearInstance.ShellEffects.Clear();
                            wearInstance.ShellRarity = null;
                            DAOFactory.ShellEffectDAO.DeleteByEquipmentSerialId(wearInstance.EquipmentSerialId);
                            if (wearInstance.EquipmentSerialId == Guid.Empty)
                            {
                                wearInstance.EquipmentSerialId = Guid.NewGuid();
                            }

                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                            session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("OPTION_DELETE"), 0));
                        }
                    }
                    else
                    {
                        session.SendPacket("guri 18 0");
                    }

                    break;

                // Atk/Def/HP/Exp potions  //so i dont have card for that wait a sec
                case 6600:
                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    break;

                // Golden potion
                case 6601:
                    if (!session.Character.Buff.ContainsKey(4061))
                    {
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.Character.AddBuff(new Buff(4061, session.Character.Level), session.Character.BattleEntity);
                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));

                    }
                    break;

                case 6602: // Cleansing Pot
                    var AllowedMaps = new List<MapInstanceType> 
                    {
                        MapInstanceType.ArenaInstance,
                        MapInstanceType.TalentArenaMapInstance
                    };
                    if (AllowedMaps.Contains(session.CurrentMapInstance.MapInstanceType))
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_PERMITTED"), 0));
                        return;
                    }              
                    if (session.Character.LastPotionUse.AddSeconds(30) <= DateTime.Now)
                    {
                        session.Character.AddBuff(new Buff(4065, session.Character.Level), session.Character.BattleEntity);
                        session.Character.DisableBuffs(BuffType.Bad);
                        session.Character.LastPotionUse = DateTime.Now;
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("WAIT_30_SECONDS"), 0));
                    }                                              
                    break;

                // Ancelloan's Blessing
                case 208:
                    if (!session.Character.Buff.ContainsKey(121) && !session.Character.Buff.ContainsKey(4044))
                    {
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.Character.AddStaticBuff(new StaticBuffDTO { CardId = 121 });
                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));

                    }
                    break;

                // Improved Ancelloan's Blessing
                case 206:
                    if (!session.Character.Buff.ContainsKey(4044) && !session.Character.Buff.ContainsKey(121))
                    {
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.Character.AddStaticBuff(new StaticBuffDTO { CardId = 4044 });
                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));
                    }
                    break;
             

                case 214: // Buff Potion
                    var MapAllowed = new List<MapInstanceType>
                    {
                        MapInstanceType.BaseMapInstance,
                        MapInstanceType.TimeSpaceInstance,
                        MapInstanceType.RaidInstance
                    };

                    if (!MapAllowed.Contains(session.CurrentMapInstance.MapInstanceType))
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_PERMITTED"), 0));
                        return;
                    }

                    var buff = new List<Buff>
                    {
                        new Buff(155, session.Character.Level),
                        new Buff(153, session.Character.Level),
                        new Buff(151, session.Character.Level),
                        new Buff(116, session.Character.Level),
                        new Buff(117, session.Character.Level),
                        new Buff(139, session.Character.Level),
                        new Buff(89, session.Character.Level),
                        new Buff(91, session.Character.Level)
                    };

                    foreach (var b in buff)
                    {
                        session.Character.AddBuff(b, session.Character.BattleEntity);
                    }

                    session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 7567), session.Character.PositionX, session.Character.PositionY);
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("BUFFED_UP"), 0));
                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    break;

                case 2457:
                    switch (EffectValue)
                    {
                        case 0:
                            //add delay
                            if (session.Character.MapId > 0)
                            {
                                int dist = Map.GetDistance(
                                    new MapCell { X = session.Character.PositionX, Y = session.Character.PositionY },
                                    new MapCell { X = 120, Y = 56 });

                                int dist1 = Map.GetDistance(
                                    new MapCell { X = session.Character.PositionX, Y = session.Character.PositionY },
                                    new MapCell { X = 120, Y = 56 });

                                if (dist < 6)
                                {
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    session.SendPacket(session.Character.GenerateEff(820));
                                    session.SendPacket(session.Character.GenerateEff(821));
                                    session.SendPacket(session.Character.GenerateEff(6008));
                                }
                                if (dist < 3 && session.Character.MapId == 1)
                                {
                                    session.SendPacket(session.Character.GenerateEff(822));
                                    Event.PTS.GeneratePTS(1805, session);
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                }
                                if (dist < 1 && session.Character.MapId == 5)
                                {
                                    Event.PTS.GeneratePTS(1824, session);
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                }
                                if (dist < 15)
                                {
                                    session.SendPacket(session.Character.GenerateEff(820));
                                    session.SendPacket(session.Character.GenerateEff(6008));
                                }
                                else
                                {
                                    //say 1 521919 10 Aucun signal ne peut être reçu, car la distance est trop élevée.
                                    session.SendPacket(session.Character.GenerateEff(820));
                                    session.SendPacket(session.Character.GenerateEff(6009));
                                }
                            }
                            break;
                    }
                    break;


                //////btk register
                //case 1227:
                //    if (ServerManager.Instance.CanRegisterRainbowBattle == true)
                //    {
                //        if (session.Character.Family != null)
                //        {
                //            if (session.Character.Family.FamilyCharacters.Where(s => s.CharacterId == session.Character.CharacterId).First().Authority == FamilyAuthority.Head || session.Character.Family.FamilyCharacters.Where(s => s.CharacterId == session.Character.CharacterId).First().Authority == FamilyAuthority.Familykeeper)
                //            {
                //                if (ServerManager.Instance.IsCharacterMemberOfGroup(session.Character.CharacterId))
                //                {
                //                    session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("RAINBOWBATTLE_OPEN_GROUP"), 12));
                //                    return;
                //                }
                //                Group group = new Group
                //                {
                //                    GroupType = GroupType.BigTeam
                //                };
                //                group.JoinGroup(session.Character.CharacterId);
                //                ServerManager.Instance.AddGroup(group);
                //                session.SendPacket(session.Character.GenerateFbt(2));
                //                session.SendPacket(session.Character.GenerateFbt(0));
                //                session.SendPacket(session.Character.GenerateFbt(1));
                //                session.SendPacket(group.GenerateFblst());
                //                session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("RAINBOWBATTLE_LEADER"), session.Character.Name), 0));
                //                session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("RAINBOWBATTLE_LEADER"), session.Character.Name), 10));

                //                ServerManager.Instance.RainbowBattleMembers.Add(new RainbowBattleMember
                //                {
                //                    RainbowBattleType = EventType.RAINBOWBATTLE,
                //                    Session = session,
                //                    GroupId = group.GroupId,
                //                });

                //                ItemInstance RainbowBattleSeal = session.Character.Inventory.LoadBySlotAndType(inv.Slot, InventoryType.Main);
                //                session.Character.Inventory.RemoveItemFromInventory(RainbowBattleSeal.Id);
                //            }
                //        }
                //    }
                //    break;


                // Prevent usage of items
                case 215:
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_PERMITTED"), 0));
                        return;

                    }

                // Valentine Buff
                case 209:
                    if (!session.Character.Buff.ContainsKey(109))
                    {
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);

                        //session.Character.AddStaticBuff(new StaticBuffDTO { CardId = 109 });
                        session.Character.AddBuff(new Buff(109, session.Character.Level),
                            session.Character.BattleEntity);
                    }
                    else
                    {
                        session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));
                    }

                    break;

                // Valentine Buff, but stronger
                case 299:
                    if (!session.Character.Buff.ContainsKey(109) || !session.Character.Buff.ContainsKey(244))
                    {
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.Character.AddBuff(new Buff(244, session.Character.Level), session.Character.BattleEntity);

                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));
                    }
                    break;

                // Guardian Angel's Blessing
                case 210:
                    if (!session.Character.Buff.ContainsKey(122))
                    {
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.Character.AddStaticBuff(new StaticBuffDTO {CardId = 122});
                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));
                    }

                    break;

                case 2081:
                    if (!session.Character.Buff.ContainsKey(146))
                    {
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.Character.AddStaticBuff(new StaticBuffDTO {CardId = 146});
                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));
                    }
                    break;

                // Divorce letter
                case 6969:
                    if (session.Character.Group != null)
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ALLOWED_IN_GROUP"), 0));
                        return;
                    }

                    var rel = session.Character.CharacterRelations.FirstOrDefault(s => s.RelationType == CharacterRelationType.Spouse);
                    if (rel != null)
                    {
                        session.Character.DeleteRelation(
                            rel.CharacterId == session.Character.CharacterId ? rel.RelatedCharacterId : rel.CharacterId,
                            CharacterRelationType.Spouse);
                        session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("DIVORCED")));
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }

                    break;

                // Cupid's arrow
                case 34:
                    if (packetsplit != null && packetsplit.Length > 3)
                    {
                        if (long.TryParse(packetsplit[3], out var characterId))
                        {
                            if (session.Character.CharacterId == characterId)
                            {
                                return;
                            }

                            if (session.Character.CharacterRelations.Any(s =>
                                    s.RelationType == CharacterRelationType.Spouse))
                            {
                                session.SendPacket($"info {Language.Instance.GetMessageFromKey("ALREADY_MARRIED")}");
                                return;
                            }

                            if (session.Character.Group != null)
                            {
                                session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("NOT_ALLOWED_IN_GROUP"), 0));
                                return;
                            }

                            if (!session.Character.IsFriendOfCharacter(characterId))
                            {
                                session.SendPacket($"info {Language.Instance.GetMessageFromKey("MUST_BE_FRIENDS")}");
                                return;
                            }

                            var otherSession = ServerManager.Instance.GetSessionByCharacterId(characterId);
                            if (otherSession != null)
                            {
                                if (otherSession.Character.Group != null)
                                {
                                    session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                            Language.Instance.GetMessageFromKey("OTHER_PLAYER_IN_GROUP"), 0));
                                    return;
                                }

                                otherSession.SendPacket(UserInterfaceHelper.GenerateDialog(
                                        $"#fins^34^{session.Character.CharacterId} #fins^69^{session.Character.CharacterId} {string.Format(Language.Instance.GetMessageFromKey("MARRY_REQUEST"), session.Character.Name)}"));
                                session.Character.MarryRequestCharacters.Add(characterId);
                                session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                            }
                        }
                    }

                    break;

                case 100: // Miniland Signpost
                {
                    if (session.Character.BattleEntity.GetOwnedNpcs()
                        .Any(s => session.Character.BattleEntity.IsSignpost(s.NpcVNum)))
                    {
                        return;
                    }

                    if (session.CurrentMapInstance.MapInstanceType == MapInstanceType.BaseMapInstance &&
                        new short[] {1, 145}.Contains(session.CurrentMapInstance.Map.MapId))
                    {
                        var signPost = new MapNpc
                        {
                            NpcVNum = (short) EffectValue,
                            MapX = session.Character.PositionX,
                            MapY = session.Character.PositionY,
                            MapId = session.CurrentMapInstance.Map.MapId,
                            ShouldRespawn = false,
                            IsMoving = false,
                            MapNpcId = session.CurrentMapInstance.GetNextNpcId(),
                            Owner = session.Character.BattleEntity,
                            Dialog = 10000,
                            Position = 2,
                            Name = $"{session.Character.Name}'s^[Miniland]"
                        };
                        switch (EffectValue)
                        {
                            case 1428:
                            case 1499:
                            case 1519:
                                signPost.AliveTime = 3600;
                                break;

                            default:
                                signPost.AliveTime = 1800;
                                break;
                        }

                        signPost.Initialize(session.CurrentMapInstance);
                        session.CurrentMapInstance.AddNPC(signPost);
                        session.CurrentMapInstance.Broadcast(signPost.GenerateIn());
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                }
                    break;

                case 550: // Campfire and other craft npcs
                {
                    if (session.CurrentMapInstance.MapInstanceType == MapInstanceType.BaseMapInstance)
                    {
                        short dialog = 10023;
                        switch (EffectValue)
                        {
                            case 956:
                                dialog = 10023;
                                break;

                            case 957:
                                dialog = 10024;
                                break;

                            case 959:
                                dialog = 10026;
                                break;
                        }

                        var campfire = new MapNpc
                        {
                            NpcVNum = (short) EffectValue,
                            MapX = session.Character.PositionX,
                            MapY = session.Character.PositionY,
                            MapId = session.CurrentMapInstance.Map.MapId,
                            ShouldRespawn = false,
                            IsMoving = false,
                            MapNpcId = session.CurrentMapInstance.GetNextNpcId(),
                            Owner = session.Character.BattleEntity,
                            Dialog = dialog,
                            Position = 2
                        };
                        campfire.AliveTime = 180;
                        campfire.Initialize(session.CurrentMapInstance);
                        session.CurrentMapInstance.AddNPC(campfire);
                        session.CurrentMapInstance.Broadcast(campfire.GenerateIn());
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                }
                    break;

                // Faction Egg
                case 570:
                    if (session.CurrentMapInstance?.MapInstanceType == MapInstanceType.Act4Instance)
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("MUST_BE_IN_CLASSIC_MAP"), 0));
                        return;
                    }

                    if (EffectValue < 3)
                    {
                        if (session.Character.Faction == (FactionType)EffectValue)
                        {
                            session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SAME_FACTION"), 0));
                            return;
                        }
                        if (session.Character.LastFactionChange > DateTime.Now.AddDays(-1).Ticks)
                        {
                            session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CHANGE_NOT_PERMITTED"), 0));
                            return;
                        }
                        session.SendPacket(session.Character.Family == null
                            ? $"qna #guri^750^{EffectValue} {Language.Instance.GetMessageFromKey($"ASK_CHANGE_FACTION{EffectValue}")}"
                            : UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("IN_FAMILY"),
                                0));
                    }
                    else
                    {
                        if (session.Character.Family == null)
                        {
                            session.SendPacket(
                                UserInterfaceHelper.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("NO_FAMILY"), 0));
                            return;
                        }

                        if ((session.Character.Family.FamilyFaction / 2) == EffectValue)
                        {
                            session.SendPacket(
                                UserInterfaceHelper.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("SAME_FACTION"), 0));
                            return;
                        }

                        session.SendPacket(session.Character.Family != null
                            ? $"qna #guri^750^{EffectValue} {Language.Instance.GetMessageFromKey($"ASK_CHANGE_FACTION{EffectValue}")}"
                            : UserInterfaceHelper.GenerateMsg(
                                Language.Instance.GetMessageFromKey("NOT_IN_FAMILY"),
                                0));
                    }

                    break;
                //case 570:
                //    if (session.Character.Faction == (FactionType) EffectValue)
                //    {
                //        return;
                //    }
                //    if (session.CurrentMapInstance?.MapInstanceType == MapInstanceType.Act4Instance)
                //    {
                //        session.SendPacket(
                //            UserInterfaceHelper.GenerateMsg(
                //                Language.Instance.GetMessageFromKey("MUST_BE_IN_CLASSIC_MAP"), 0));
                //        return;
                //    }
                //    if (EffectValue < 3)
                //    {
                //        session.SendPacket(session.Character.Family == null
                //                ? $"qna #guri^750^{EffectValue} {Language.Instance.GetMessageFromKey($"ASK_CHANGE_FACTION{EffectValue}")}"
                //                : UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("IN_FAMILY"),
                //                        0));
                //    }
                //    else
                //    {
                //        session.SendPacket(session.Character.Family != null
                //                ? $"qna #guri^750^{EffectValue} {Language.Instance.GetMessageFromKey($"ASK_CHANGE_FACTION{EffectValue}")}"
                //                : UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_FAMILY"),
                //                        0));
                //    }
                //    break;

                // SP Wings
                case 650:
                    var SpecialistInstance =
                        session.Character.Inventory.LoadBySlotAndType((byte) EquipmentType.Sp, InventoryType.Wear);
                    if (session.Character.UseSp && SpecialistInstance != null && !session.Character.IsSeal)
                    {
                        if (Option == 0)
                        {
                            session.SendPacket(
                                $"qna #u_i^1^{session.Character.CharacterId}^{(byte) inv.Type}^{inv.Slot}^3 {Language.Instance.GetMessageFromKey("ASK_WINGS_CHANGE")}");
                        }
                        else
                        {
                            CharacterHelper.RemoveSpecialistWingsBuff(session);
                            SpecialistInstance.Design = (byte) EffectValue;
                            session.Character.MorphUpgrade2 = EffectValue;
                            session.CurrentMapInstance?.Broadcast(session.Character.GenerateCMode());
                            session.SendPacket(session.Character.GenerateStat());
                            session.SendPackets(session.Character.GenerateStatChar());
                            CharacterHelper.AddSpecialistWingsBuff(session);
                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        }
                    }
                    else
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_SP"),
                            0));
                    }

                    break;

                // Self-Introduction
                case 203:
                    if (!session.Character.IsVehicled && Option == 0)
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateGuri(10, 2, session.Character.CharacterId, 1));
                    }
                    break;

                // Magic Lamp
                case 651:
                    if (session.Character.Inventory.All(i => i.Type != InventoryType.Wear))
                    {
                        if (Option == 0)
                        {
                            session.SendPacket(
                                $"qna #u_i^1^{session.Character.CharacterId}^{(byte) inv.Type}^{inv.Slot}^3 {Language.Instance.GetMessageFromKey("ASK_USE")}");
                        }
                        else
                        {
                            session.Character.ChangeSex();
                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        }
                    }
                    else
                    {
                        session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("EQ_NOT_EMPTY"), 0));
                    }

                    break;

                // Vehicles
                case 1000:
                    if (EffectValue != 0
                        || session.CurrentMapInstance?.MapInstanceType == MapInstanceType.EventGameInstance
                        || session.CurrentMapInstance?.MapInstanceType == MapInstanceType.TalentArenaMapInstance
                        || session.CurrentMapInstance?.MapInstanceType == MapInstanceType.IceBreakerInstance
                        || session.Character.IsSeal || session.Character.IsMorphed)
                    {
                        return;
                    }

                    var morph = Morph;
                    var speed = Speed;
                    if (Morph < 0)
                    {
                        switch (VNum)
                        {
                            case 5923:
                                morph = 2513;
                                speed = 14;
                                break;
                        }
                    }

                    if (morph > 0)
                    {
                        if (Option == 0 && !session.Character.IsVehicled)
                        {
                            if (session.Character.Buff.Any(s => s.Card.BuffType == BuffType.Bad))
                            {
                                session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("CANT_TRASFORM_WITH_DEBUFFS"),
                                    0));
                                return;
                            }

                            if (session.Character.IsSitting)
                            {
                                session.Character.IsSitting = false;
                                session.CurrentMapInstance?.Broadcast(session.Character.GenerateRest());
                            }

                            session.Character.LastDelay = DateTime.Now;
                            session.SendPacket(UserInterfaceHelper.GenerateDelay(3000, 3,
                                $"#u_i^1^{session.Character.CharacterId}^{(byte) inv.Type}^{inv.Slot}^2"));
                        }
                        else
                        {
                            if (!session.Character.IsVehicled && Option != 0)
                            {
                                var delay = DateTime.Now.AddSeconds(-4);
                                if (session.Character.LastDelay > delay &&
                                    session.Character.LastDelay < delay.AddSeconds(2))
                                {
                                    session.Character.IsVehicled = true;
                                    session.Character.VehicleSpeed = speed;
                                    session.Character.VehicleItem = this;
                                    session.Character.LoadSpeed();
                                    session.Character.MorphUpgrade = 0;
                                    session.Character.MorphUpgrade2 = 0;
                                    session.Character.Morph = morph + (byte) session.Character.Gender;
                                    session.CurrentMapInstance?.Broadcast(
                                        StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId,
                                            196), session.Character.PositionX, session.Character.PositionY);
                                    session.CurrentMapInstance?.Broadcast(session.Character.GenerateCMode());
                                    session.SendPacket(session.Character.GenerateCond());
                                    session.Character.LastSpeedChange = DateTime.Now;
                                    session.Character.Mates.Where(s => s.IsTeamMember).ToList()
                                        .ForEach(s => session.CurrentMapInstance?.Broadcast(s.GenerateOut()));
                                    if (Morph < 0)
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                }
                            }
                            else if (session.Character.IsVehicled)
                            {
                                session.Character.RemoveVehicle();
                                foreach (var teamMate in session.Character.Mates.Where(m => m.IsTeamMember))
                                {
                                    teamMate.PositionX =
                                        (short) (session.Character.PositionX +
                                                 (teamMate.MateType == MateType.Partner ? -1 : 1));
                                    teamMate.PositionY = (short) (session.Character.PositionY + 1);
                                    if (session.Character.MapInstance.Map.IsBlockedZone(teamMate.PositionX,
                                        teamMate.PositionY))
                                    {
                                        teamMate.PositionX = session.Character.PositionX;
                                        teamMate.PositionY = session.Character.PositionY;
                                    }

                                    teamMate.UpdateBushFire();
                                    foreach (var sess in session.CurrentMapInstance.Sessions.Where(s =>
                                        s.Character != null))
                                    {
                                        if (ServerManager.Instance.ChannelId != 51 ||
                                            session.Character.Faction        == sess.Character.Faction)
                                        {
                                            sess.SendPacket(teamMate.GenerateIn(false,
                                                    ServerManager.Instance.ChannelId == 51));
                                        }
                                        else
                                        {
                                            sess.SendPacket(teamMate.GenerateIn(true,
                                                    ServerManager.Instance.ChannelId == 51, sess.Account.Authority));
                                        }
                                    }
                                }

                                session.SendPacket(session.Character.GeneratePinit());
                                session.Character.Mates.ForEach(s => session.SendPacket(s.GenerateScPacket()));
                                session.SendPackets(session.Character.GeneratePst());
                            }
                        }
                    }

                    break;

                // Sealed Vessel
                case 1002:
                    if (session?.Character?.MapId != 9999)
                    {
                        session.SendPacket(UserInterfaceHelper.GenerateInfo("WARNING: You must be in the allowed map to use the vessels!"));
                        return;
                    }
                       
                    {
                        int type, secondaryType, inventoryType, slot;
                        if (packetsplit != null && int.TryParse(packetsplit[2], out type) &&
                            int.TryParse(packetsplit[3], out secondaryType) &&
                            int.TryParse(packetsplit[4], out inventoryType) && int.TryParse(packetsplit[5], out slot))
                        {
                            int packetType;
                            switch (EffectValue)
                            {
                                case 69:
                                    if (int.TryParse(packetsplit[6], out packetType))
                                    {
                                        switch (packetType)
                                        {
                                            case 0:
                                                session.SendPacket(UserInterfaceHelper.GenerateDelay(5000, 7,
                                                        $"#u_i^{type}^{secondaryType}^{inventoryType}^{slot}^1"));
                                                break;

                                            case 1:
                                                var rnd = ServerManager.RandomNumber(0, 1000);
                                                if (rnd < 5)
                                                {
                                                    short[] vnums =
                                                    {
                                                        5560, 5591, 4099, 907, 1160, 4705, 4706, 4707, 4708, 4709, 4710,
                                                        4711, 4712, 4713, 4714,
                                                        4715, 4716
                                                };
                                                    byte[] counts = { 1, 1, 1, 1, 10, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
                                                    var item = ServerManager.RandomNumber(0, 17);
                                                    session.Character.GiftAdd(vnums[item], counts[item]);
                                                }
                                                else if (rnd < 30)
                                                {
                                                    short[] vnums = { 361, 362, 363, 366, 367, 368, 371, 372, 373 };
                                                    session.Character.GiftAdd(vnums[ServerManager.RandomNumber(0, 9)], 1);
                                                }
                                                else
                                                {
                                                    short[] vnums =
                                                    {
                                                        1161, 2282, 1030, 1244, 1218, 5369, 1012, 1363, 1364, 2160, 2173,
                                                        5959, 5983, 2514,
                                                        2515, 2516, 2517, 2518, 2519, 2520, 2521, 1685, 1686, 5087, 5203,
                                                        2418, 2310, 2303,
                                                        2169, 2280, 5892, 5893, 5894, 5895, 5896, 5897, 5898, 5899, 5332,
                                                        5105, 2161, 2162
                                                };
                                                    byte[] counts =
                                                    {
                                                        10, 10, 20, 5, 1, 1, 99, 1, 1, 5, 5, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2,
                                                        1, 1, 1, 1, 5, 20,
                                                        20, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
                                                };
                                                    var item = ServerManager.RandomNumber(0, 42);
                                                    session.Character.GiftAdd(vnums[item], counts[item]);
                                                }

                                                session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                                break;

                                            case 2: // Santa vessels
                                                var random = ServerManager.RandomNumber();
                                                if (random <= 5)
                                                {
                                                    short[] vnums =
                                                    {
                                                        4075, 4076, 5209, 5211, 5070
                                                };
                                                    byte[] counts = { 5, 5, 40, 40 };
                                                    var item = ServerManager.RandomNumber(0, 5);
                                                    session.Character.GiftAdd(vnums[item], counts[item]);
                                                }

                                                session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                                break;
                                        }
                                    }

                                    break;

                                default:
                                    if (int.TryParse(packetsplit[6], out packetType))
                                    {
                                        if (!session.Character.VerifiedLock)
                                        {
                                            session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ACTION_NOT_POSSIBLE_USE_UNLOCK"), 0));
                                            return;
                                        }

                                        if (session.Character.MapInstance.Map.MapTypes.Any(s =>
                                            s.MapTypeId == (short)MapTypeEnum.Act4))
                                        {
                                            return;
                                        }

                                        switch (packetType)
                                        {
                                            case 0:
                                                session.SendPacket(UserInterfaceHelper.GenerateDelay(5000, 7,
                                                    $"#u_i^{type}^{secondaryType}^{inventoryType}^{slot}^1"));
                                                break;

                                            case 1:
                                                if (session.HasCurrentMapInstance &&
                                                    (session.Character.MapInstance == session.Character.Miniland ||
                                                     session.CurrentMapInstance.MapInstanceType ==
                                                     MapInstanceType.BaseMapInstance) &&
                                                    (session.Character.LastVessel.AddSeconds(1) <= DateTime.Now ||
                                                     session.Character.StaticBonusList.Any(s =>
                                                         s.StaticBonusType == StaticBonusType.FastVessels)))
                                                {
                                                    short[] vnums =
                                                    {
                                                    1386, 1387, 1388, 1389, 1390, 1391, 1392, 1393, 1394, 1395, 1396,
                                                    1397, 1398, 1399, 1400, 1401, 1402, 1403, 1404, 1405
                                                };
                                                    var vnum = vnums[ServerManager.RandomNumber(0, 20)];

                                                    var npcmonster = ServerManager.GetNpcMonster(vnum);
                                                    if (npcmonster == null)
                                                    {
                                                        return;
                                                    }

                                                    var monster = new MapMonster
                                                    {
                                                        MonsterVNum = vnum,
                                                        MapX = session.Character.PositionX,
                                                        MapY = session.Character.PositionY,
                                                        MapId = session.Character.MapInstance.Map.MapId,
                                                        Position = session.Character.Direction,
                                                        IsMoving = true,
                                                        MapMonsterId = session.CurrentMapInstance.GetNextMonsterId(),
                                                        ShouldRespawn = false,
                                                        IsVessel = true
                                                    };
                                                    monster.Initialize(session.CurrentMapInstance);
                                                    session.CurrentMapInstance.AddMonster(monster);
                                                    session.CurrentMapInstance.Broadcast(monster.GenerateIn());
                                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                                    session.Character.LastVessel = DateTime.Now;
                                                }

                                                break;

                                            case 2: // monsters in --> xmas sealed vessel
                                                if (session.HasCurrentMapInstance &&
                                                    (session.Character.MapInstance == session.Character.Miniland ||
                                                     session.CurrentMapInstance.MapInstanceType ==
                                                     MapInstanceType.BaseMapInstance) &&
                                                    (session.Character.LastVessel.AddSeconds(1) <= DateTime.Now ||
                                                     session.Character.StaticBonusList.Any(s =>
                                                         s.StaticBonusType == StaticBonusType.FastVessels)))
                                                {
                                                    short[] vnums =
                                                        {532, 535, 751, 1424, 2046, 2047, 2055, 2056, 2057, 2058};
                                                    var vnum = vnums[ServerManager.RandomNumber(0, 10)];

                                                    var npcmonster = ServerManager.GetNpcMonster(vnum);
                                                    if (npcmonster == null)
                                                    {
                                                        return;
                                                    }

                                                    var monster = new MapMonster
                                                    {
                                                        MonsterVNum = vnum,
                                                        MapX = session.Character.PositionX,
                                                        MapY = session.Character.PositionY,
                                                        MapId = session.Character.MapInstance.Map.MapId,
                                                        Position = session.Character.Direction,
                                                        IsMoving = true,
                                                        MapMonsterId = session.CurrentMapInstance.GetNextMonsterId(),
                                                        ShouldRespawn = false,
                                                        IsVessel = true
                                                    };
                                                    monster.Initialize(session.CurrentMapInstance);
                                                    session.CurrentMapInstance.AddMonster(monster);
                                                    session.CurrentMapInstance.Broadcast(monster.GenerateIn());
                                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                                    session.Character.LastVessel = DateTime.Now;
                                                }

                                                break;
                                        }
                                    }

                                    break;
                            }
                        }

                        break;
                    }

                //Medal of Erenia
                case 9999:
                    if (!session.Character.StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.MedalOfErenia))
                    {
                        session.Character.StaticBonusList.Add(new StaticBonusDTO
                        {
                            CharacterId = session.Character.CharacterId,
                            DateEnd = DateTime.Now.AddDays(EffectValue),
                            StaticBonusType = StaticBonusType.MedalOfErenia
                        });
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                    }
                    else
                    {
                        session.SendPacket(session.Character.GenerateSay("This Item is already in use!", 11));
                    }
                    break;

                // Golden Bazaar Medal
                case 1003:
                    if (!session.Character.StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.BazaarMedalGold || s.StaticBonusType == StaticBonusType.BazaarMedalSilver))
                    {
                        session.Character.StaticBonusList.Add(new StaticBonusDTO
                        {
                            CharacterId = session.Character.CharacterId,
                            DateEnd = DateTime.Now.AddDays(EffectValue),
                            StaticBonusType = StaticBonusType.BazaarMedalGold
                        });
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                    }
                    break;

                // Silver Bazaar Medal
                case 1004:
                    if (!session.Character.StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.BazaarMedalGold || s.StaticBonusType == StaticBonusType.BazaarMedalGold))
                    {
                        session.Character.StaticBonusList.Add(new StaticBonusDTO
                        {
                            CharacterId = session.Character.CharacterId,
                            DateEnd = DateTime.Now.AddDays(EffectValue),
                            StaticBonusType = StaticBonusType.BazaarMedalSilver
                        });
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                    }
                    break;

                // Pet Slot Expansion
                case 1006:
                    if (Option == 0)
                    {
                        session.SendPacket(
                            $"qna #u_i^1^{session.Character.CharacterId}^{(byte) inv.Type}^{inv.Slot}^2 {Language.Instance.GetMessageFromKey("ASK_PET_MAX")}");
                    }
                    else if (inv.Item?.IsSoldable == true && session.Character.MaxMateCount < 90 ||
                             session.Character.MaxMateCount < 30)
                    {
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.Character.MaxMateCount += 10;
                        session.SendPacket(
                            session.Character.GenerateSay(Language.Instance.GetMessageFromKey("GET_PET_PLACES"), 10));
                        session.SendPacket(session.Character.GenerateScpStc());
                    }

                    break;

                // Permanent Backpack Expansion
                case 601:
                    if (session.Character.StaticBonusList.All(s => s.StaticBonusType != StaticBonusType.BackPack))
                    {
                        session.Character.StaticBonusList.Add(new StaticBonusDTO
                        {
                            CharacterId = session.Character.CharacterId,
                            DateEnd = DateTime.Now.AddYears(15),
                            StaticBonusType = StaticBonusType.BackPack
                        });
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateExts());
                        session.SendPacket(session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                    }

                    break;

                // Permanent Partner's Backpack
                case 602:
                    if (session.Character.StaticBonusList.All(s => s.StaticBonusType != StaticBonusType.PetBackPack))
                    {
                        session.Character.StaticBonusList.Add(new StaticBonusDTO
                        {
                            CharacterId = session.Character.CharacterId,
                            DateEnd = DateTime.Now.AddYears(15),
                            StaticBonusType = StaticBonusType.PetBackPack
                        });
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateExts());
                        session.SendPacket(session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                    }

                    break;

                // Permanent Pet Basket
                case 603:
                    if (session.Character.StaticBonusList.All(s => s.StaticBonusType != StaticBonusType.PetBasket))
                    {
                        session.Character.StaticBonusList.Add(new StaticBonusDTO
                        {
                            CharacterId = session.Character.CharacterId,
                            DateEnd = DateTime.Now.AddYears(15),
                            StaticBonusType = StaticBonusType.PetBasket
                        });
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateExts());
                        session.SendPacket("ib 1278 1");
                        session.SendPacket(session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                    }

                    break;

                // Pet Basket
                case 1007:
                    if (session.Character.StaticBonusList.All(s => s.StaticBonusType != StaticBonusType.PetBasket))
                    {
                        session.Character.StaticBonusList.Add(new StaticBonusDTO
                        {
                            CharacterId = session.Character.CharacterId,
                            DateEnd = DateTime.Now.AddDays(EffectValue),
                            StaticBonusType = StaticBonusType.PetBasket
                        });
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateExts());
                        session.SendPacket("ib 1278 1");
                        session.SendPacket(session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                    }

                    break;

                // Partner's Backpack
                case 1008:
                    if (session.Character.StaticBonusList.All(s => s.StaticBonusType != StaticBonusType.PetBackPack))
                    {
                        session.Character.StaticBonusList.Add(new StaticBonusDTO
                        {
                            CharacterId = session.Character.CharacterId,
                            DateEnd = DateTime.Now.AddDays(EffectValue),
                            StaticBonusType = StaticBonusType.PetBackPack
                        });
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateExts());
                        session.SendPacket(session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                    }

                    break;

                // Backpack Expansion
                case 1009:
                    if (session.Character.StaticBonusList.All(s => s.StaticBonusType != StaticBonusType.BackPack))
                    {
                        session.Character.StaticBonusList.Add(new StaticBonusDTO
                        {
                            CharacterId = session.Character.CharacterId,
                            DateEnd = DateTime.Now.AddDays(EffectValue),
                            StaticBonusType = StaticBonusType.BackPack
                        });
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateExts());
                        session.SendPacket(session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                    }

                    break;

                // Sealed Tarot Card
                case 1005:
                    session.Character.GiftAdd((short) (VNum - Effect), 1);
                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    break;

                // Tarot Card Game
                case 1894:
                    if (EffectValue == 0)
                    {
                        for (var i = 0; i < 5; i++)
                        {
                            session.Character.GiftAdd((short) (Effect + ServerManager.RandomNumber(0, 10)), 1);
                        }

                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }

                    break;

                // Sealed Tarot Card
                case 2152:
                    session.Character.GiftAdd((short) (VNum + Effect), 1);
                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    break;

                // Transformation scrolls
                case 1001:
                    if (session.Character.IsMorphed)
                    {
                        session.Character.IsMorphed = false;
                        session.Character.Morph = 0;
                        session.CurrentMapInstance?.Broadcast(session.Character.GenerateCMode());
                    }
                    else if (!session.Character.UseSp && !session.Character.IsVehicled)
                    {
                        if (Option == 0)
                        {
                            session.Character.LastDelay = DateTime.Now;
                            session.SendPacket(UserInterfaceHelper.GenerateDelay(3000, 3, $"#u_i^1^{session.Character.CharacterId}^{(byte)inv.Type}^{inv.Slot}^1"));
                        }
                        else
                        {
                            int[] possibleTransforms = null;

                            switch (EffectValue)
                            {
                                case 1: // Halloween
                                    possibleTransforms = new[]
                                    {
                                        404, //Torturador pellizcador
                                        405, //Torturador enrollador
                                        406, //Torturador de acero
                                        446, //Guerrero yak
                                        447, //Mago yak
                                        441, //Guerrero de la muerte
                                        276, //Rey polvareda
                                        324, //Princesa Catrisha
                                        248, //Bruja oscura
                                        249, //Bruja de sangre
                                        438, //Bruja blanca fuerte
                                        236, //Guerrero esqueleto
                                        245, //Sombra nocturna
                                        439, //Guerrero esqueleto resucitado
                                        272, //Arquero calavera
                                        274, //Guerrero calavera
                                        2691 //Frankenstein
                                    };
                                    break;

                                case 2: // Ice Costume
                                    possibleTransforms = new int[]
                                    {
                                    543,
                                    544,
                                    545,
                                    552,
                                    666,
                                    668,
                                    727,
                                    753,
                                    754,
                                    755,
                                    };
                                    break;

                                case 3: // Bushtail Costume
                                case 4:
                                    possibleTransforms = new int[]
                                    {
                                        156,
                                    };
                                    break;
                            }

                            if (possibleTransforms != null)
                            {
                                session.Character.IsMorphed = true;
                                session.Character.Morph = 1000 + possibleTransforms[ServerManager.RandomNumber(0, possibleTransforms.Length)];
                                session.CurrentMapInstance?.Broadcast(session.Character.GenerateCMode());
                                if (VNum != 1914)
                                {
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                }
                            }
                        }
                    }
                    break;

                //Max Perfections
                case 3500:
                {
                    var sp = session.Character.Inventory.LoadBySlotAndType((byte) EquipmentType.Sp, InventoryType.Wear);
                    if (!session.Character.UseSp && sp != null)
                    {
                        if (EffectValue == 0)
                        {
                            PerfectionHelper.Instance.SpeedPerfection(session, sp, inv);
                        }
                    }
                }
                    break;

                //Remove Perfection
                case 3501:
                {
                    var sp = session.Character.Inventory.LoadBySlotAndType((byte) EquipmentType.Sp, InventoryType.Wear);
                    if (!session.Character.UseSp && sp != null)
                    {
                        if (EffectValue == 0)
                        {
                            PerfectionHelper.Instance.RemovePerfection(session, sp, inv);
                        }
                    }
                }
                break;


                case 3530: // Change Class
                {
                    if (Option == 0)
                    {
                        session.SendPacket($"qna #u_i^1^{session.Character.CharacterId}^{(byte)inv.Type}^{inv.Slot}^3 Do you really want to change your class?");

                    }
                    else
                    {
                        session.ChangeClass(inv);
                    }
                }
                break;

                case 3531:
                    if (session.Character.Level == 99 && session.Character.Class != 0)
                    {
                        session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MAX_LEVEL_REACHED"), 10));
                        return;
                    }
                    else
                    {
                        session.Character.Level++;
                        session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("LEVEL_UP"), 10));

                    }
                    break;

                // FAST UPGRADE SP
                case 3535:
                    var specialistInstance = session.Character.Inventory.LoadBySlotAndType((byte)EquipmentType.Sp, InventoryType.Wear);

                    if (specialistInstance == null)
                    {
                        return;
                    }

                    if (specialistInstance.Upgrade >= 15)
                    {
                        return;
                    }

                {
                    byte[] upfail = {20, 25, 30, 40, 50, 60, 65, 70, 75, 80, 90, 93, 95, 97, 99};
                    byte[] destroy = {0, 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 70};
                    int[] goldprice =
                    {
                        200000, 200000, 200000, 200000, 200000, 500000, 500000, 500000, 500000, 500000, 1000000,
                        1000000, 1000000, 1000000, 1000000
                    };
                    byte[] feather = {3, 5, 8, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 70};
                    byte[] fullmoon = {1, 3, 5, 7, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30};
                    byte[] soul = {2, 4, 6, 8, 10, 1, 2, 3, 4, 5, 1, 2, 3, 4, 5};

                    const short featherVnum = 2282;
                    const short fullmoonVnum = 1030;
                    const short greenSoulVnum = 2283;
                    const short redSoulVnum = 2284;
                    const short blueSoulVnum = 2285;
                    const short dragonSkinVnum = 2511;
                    const short dragonBloodVnum = 2512;
                    const short dragonHeartVnum = 2513;
                    const short blueScrollVnum = 1363;
                    const short redScrollVnum = 1364;

                    var fallimento = 0;
                    short itemToRemove = 2283;

                    if (specialistInstance.SpLevel < 99)
                    {
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("REQUIRED_SPLVL_99"), 99), 11));
                        return;
                    }

                    while (specialistInstance.Upgrade < 15)
                    {
                        if (session.Character.Gold < goldprice[specialistInstance.Upgrade])
                        {
                            session.SendPacket(
                                session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"),
                                    10));
                            break;
                        }

                        if (session.Character.Inventory.CountItem(featherVnum) < feather[specialistInstance.Upgrade])
                        {
                            session.SendPacket(session.Character.GenerateSay(
                                Language.Instance.GetMessageFromKey(string.Format(
                                    Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                    ServerManager.GetItem(featherVnum).Name, feather[specialistInstance.Upgrade])),
                                10));
                            break;
                        }

                        if (session.Character.Inventory.CountItem(fullmoonVnum) < fullmoon[specialistInstance.Upgrade])
                        {
                            session.SendPacket(session.Character.GenerateSay(
                                Language.Instance.GetMessageFromKey(string.Format(
                                    Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                    ServerManager.GetItem(featherVnum).Name, fullmoon[specialistInstance.Upgrade])),
                                10));
                            break;
                        }

                        if (specialistInstance.Item.Morph <= 16)
                        {
                            if (specialistInstance.Upgrade < 5)
                            {
                                if (session.Character.Inventory.CountItem(greenSoulVnum) < soul[specialistInstance.Upgrade])
                                {
                                    session.SendPacket(session.Character.GenerateSay(
                                        Language.Instance.GetMessageFromKey(string.Format(
                                            Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                            ServerManager.GetItem(greenSoulVnum).Name,
                                            soul[specialistInstance.Upgrade])), 10));
                                    break;
                                }

                                if (session.Character.Inventory.CountItem(blueScrollVnum) == 0)
                                {
                                    session.SendPacket(session.Character.GenerateSay(
                                        Language.Instance.GetMessageFromKey(string.Format(
                                            Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                            ServerManager.GetItem(blueScrollVnum).Name, 1)), 10));
                                    break;
                                }

                                itemToRemove = greenSoulVnum;
                                session.Character.Inventory.RemoveItemAmount(blueScrollVnum);
                            }
                            else if (specialistInstance.Upgrade < 10)
                            {
                                if (session.Character.Inventory.CountItem(redSoulVnum) < soul[specialistInstance.Upgrade])
                                {
                                    session.SendPacket(session.Character.GenerateSay(
                                        Language.Instance.GetMessageFromKey(string.Format(
                                            Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                            ServerManager.GetItem(redSoulVnum).Name, soul[specialistInstance.Upgrade])),
                                        10));
                                    break;
                                }

                                if (session.Character.Inventory.CountItem(blueScrollVnum) == 0)
                                {
                                    session.SendPacket(session.Character.GenerateSay(
                                        Language.Instance.GetMessageFromKey(string.Format(
                                            Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                            ServerManager.GetItem(blueScrollVnum).Name, 1)), 10));
                                    break;
                                }

                                itemToRemove = redSoulVnum;
                                session.Character.Inventory.RemoveItemAmount(blueScrollVnum);
                            }
                            else
                            {
                                if (session.Character.Inventory.CountItem(blueSoulVnum) < soul[specialistInstance.Upgrade])
                                {
                                    session.SendPacket(session.Character.GenerateSay(
                                        Language.Instance.GetMessageFromKey(string.Format(
                                            Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                            ServerManager.GetItem(blueSoulVnum).Name,
                                            soul[specialistInstance.Upgrade])), 10));
                                    break;
                                }

                                if (session.Character.Inventory.CountItem(redScrollVnum) == 0)
                                {
                                    session.SendPacket(session.Character.GenerateSay(
                                        Language.Instance.GetMessageFromKey(string.Format(
                                            Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                            ServerManager.GetItem(redScrollVnum).Name, 1)), 10));
                                    break;
                                }

                                itemToRemove = blueSoulVnum;
                                session.Character.Inventory.RemoveItemAmount(redScrollVnum);
                            }
                        }
                        else
                        {
                            if (specialistInstance.Upgrade < 5)
                            {
                                if (session.Character.Inventory.CountItem(dragonSkinVnum) < soul[specialistInstance.Upgrade])
                                {
                                    session.SendPacket(session.Character.GenerateSay(
                                        Language.Instance.GetMessageFromKey(string.Format(
                                            Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                            ServerManager.GetItem(dragonSkinVnum).Name,
                                            soul[specialistInstance.Upgrade])), 10));
                                    break;
                                }

                                if (session.Character.Inventory.CountItem(blueScrollVnum) == 0)
                                {
                                    session.SendPacket(session.Character.GenerateSay(
                                        Language.Instance.GetMessageFromKey(string.Format(
                                            Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                            ServerManager.GetItem(blueScrollVnum).Name, 1)), 10));
                                    break;
                                }

                                session.Character.Inventory.RemoveItemAmount(blueScrollVnum);
                                itemToRemove = dragonSkinVnum;
                            }
                            else if (specialistInstance.Upgrade < 10)
                            {
                                if (session.Character.Inventory.CountItem(dragonBloodVnum) < soul[specialistInstance.Upgrade])
                                {
                                    session.SendPacket(session.Character.GenerateSay(
                                        Language.Instance.GetMessageFromKey(string.Format(
                                            Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                            ServerManager.GetItem(dragonBloodVnum).Name,
                                            soul[specialistInstance.Upgrade])), 10));
                                    break;
                                }

                                if (session.Character.Inventory.CountItem(blueScrollVnum) == 0)
                                {
                                    session.SendPacket(session.Character.GenerateSay(
                                        Language.Instance.GetMessageFromKey(string.Format(
                                            Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                            ServerManager.GetItem(blueScrollVnum).Name, 1)), 10));
                                    break;
                                }

                                session.Character.Inventory.RemoveItemAmount(blueScrollVnum);
                                itemToRemove = dragonBloodVnum;
                            }
                            else
                            {
                                if (session.Character.Inventory.CountItem(dragonHeartVnum) < soul[specialistInstance.Upgrade])
                                {
                                    session.SendPacket(session.Character.GenerateSay(
                                        Language.Instance.GetMessageFromKey(string.Format(
                                            Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                            ServerManager.GetItem(dragonHeartVnum).Name,
                                            soul[specialistInstance.Upgrade])), 10));
                                    break;
                                }

                                if (session.Character.Inventory.CountItem(redScrollVnum) == 0)
                                {
                                    session.SendPacket(session.Character.GenerateSay(
                                        Language.Instance.GetMessageFromKey(string.Format(
                                            Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                            ServerManager.GetItem(redScrollVnum).Name, 1)), 10));
                                    break;
                                }

                                session.Character.Inventory.RemoveItemAmount(redScrollVnum);
                                itemToRemove = dragonHeartVnum;
                            }
                        }

                        session.Character.Gold -= goldprice[specialistInstance.Upgrade];
                        session.Character.Inventory.RemoveItemAmount(featherVnum, feather[specialistInstance.Upgrade]);
                        var randomNumber = ServerManager.RandomNumber();

                        if (randomNumber < upfail[specialistInstance.Upgrade])
                        {
                            session.Character.Inventory.RemoveItemAmount(itemToRemove, soul[specialistInstance.Upgrade]);
                            session.Character.Inventory.RemoveItemAmount(fullmoonVnum, fullmoon[specialistInstance.Upgrade]);
                            fallimento++;
                        }
                        else
                        {
                             session.Character.Inventory.RemoveItemAmount(itemToRemove, soul[specialistInstance.Upgrade]);
                             session.Character.Inventory.RemoveItemAmount(fullmoonVnum, fullmoon[specialistInstance.Upgrade]);
                             specialistInstance.Upgrade++;

                            if (specialistInstance.Upgrade > 9)
                            {
                                session.Character.Family?.InsertFamilyLog(FamilyLogType.ItemUpgraded, session.Character.Name, itemVNum: specialistInstance.ItemVNum, upgrade: specialistInstance.Upgrade);

                            }
                            session.SendPacket(specialistInstance.GenerateInventoryAdd());
                        }

                        session.SendPacket(session.Character.GenerateGold());
                        session.SendPacket(session.Character.GenerateEq());
                    }
                    session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3005), session.Character.MapX, session.Character.MapY);
                    session.SendPacket(session.Character.GenerateSay("-------------Upgrade Review-------------", 11));
                    session.SendPacket(session.Character.GenerateSay("Current SP Upgrade: " + specialistInstance.Upgrade, 11));
                    session.SendPacket(session.Character.GenerateSay("Times Failed: " + fallimento, 11));
                    session.SendPacket(session.Character.GenerateSay("---------------------------------------", 11));
                }
                    break;

                default:
                    switch (EffectValue)
                    {
                        // Angel Base Flag
                        case 965:

                        // Demon Base Flag
                        case 966:
                            if (ServerManager.Instance.ChannelId == 51 &&
                                session.CurrentMapInstance?.Map.MapId != 130 &&
                                session.CurrentMapInstance?.Map.MapId != 131 &&
                                EffectValue - 964 == (short) session.Character.Faction)
                            {
                                if (session.CurrentMapInstance?.MapInstanceType == MapInstanceType.Act4Berios || session.CurrentMapInstance?.MapInstanceType == MapInstanceType.Act4Calvina ||
                                    session.CurrentMapInstance?.MapInstanceType == MapInstanceType.Act4Morcos || session.CurrentMapInstance?.MapInstanceType == MapInstanceType.Act4Hatus ||
                                    session.CurrentMapInstance?.MapInstanceType == MapInstanceType.CaligorInstance)

                                {
                                    session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_PERMITTED"), 0));
                                    return;
                                }

                                session.CurrentMapInstance?.SummonMonster(new MonsterToSummon((short) EffectValue, new MapCell { X = session.Character.PositionX, Y = session.Character.PositionY },
                                null, false, isHostile: false, aliveTime: 1800));

                                session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                            }

                            break;

                        default:
                            switch (VNum)
                            {
                                case 5856: // Partner Slot Expansion
                                case 9113: // Partner Slot Expansion (Limited)
                                {
                                    if (Option == 0)
                                    {
                                        session.SendPacket(
                                            $"qna #u_i^1^{session.Character.CharacterId}^{(byte) inv.Type}^{inv.Slot}^2 {Language.Instance.GetMessageFromKey("ASK_PARTNER_MAX")}");
                                    }
                                    else if (session.Character.MaxPartnerCount < 12)
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.MaxPartnerCount++;
                                        session.SendPacket(session.Character.GenerateSay(
                                            Language.Instance.GetMessageFromKey("GET_PARTNER_PLACES"), 10));
                                        session.SendPacket(session.Character.GenerateScpStc());
                                    }
                                }
                                    break;

                                case 5931: // Partner Skill Ticket (Single)
                                case 9109: //Partner Skill Ticket (Limited)
                                    {
                                        if (session?.Character?.Mates == null)
                                        {
                                            return;
                                        }

                                        if (packetsplit.Length != 10 || !byte.TryParse(packetsplit[8], out byte petId) || !byte.TryParse(packetsplit[9], out byte castId))
                                        {
                                            return;
                                        }

                                        if (castId < 0 || castId > 2)
                                        {
                                            return;
                                        }

                                        Mate partner = session.Character.Mates.ToList().FirstOrDefault(s => s.IsTeamMember && s.MateType == MateType.Partner && s.PetId == petId);

                                        if (partner?.Sp == null || partner.IsUsingSp)
                                        {
                                            return;
                                        }

                                        PartnerSkill skill = partner.Sp.GetSkill(castId);

                                        if (skill?.Skill == null)
                                        {
                                            return;
                                        }

                                        if (skill.Level == (byte)PartnerSkillLevelType.S)
                                        {
                                            return;
                                        }

                                        if (partner.Sp.RemoveSkill(castId))
                                        {
                                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);

                                            partner.Sp.ReloadSkills();
                                            partner.Sp.FullXp();

                                            session.SendPacket(UserInterfaceHelper.GenerateModal(Language.Instance.GetMessageFromKey("PSP_SKILL_RESETTED"), 1));
                                        }

                                        session.SendPacket(partner.GenerateScPacket());
                                    }
                                    break;

                                case 5932: // Partner Skill Ticket (All)
                                case 9110: // Partner Skill Ticket (Limited)
                                    {
                                        if (packetsplit.Length != 10
                                            || session?.Character?.Mates == null)
                                        {
                                            return;
                                        }

                                        if (!byte.TryParse(packetsplit[8], out byte petId) || !byte.TryParse(packetsplit[9], out byte castId))
                                        {
                                            return;
                                        }

                                        if (castId < 0 || castId > 2)
                                        {
                                            return;
                                        }

                                        Mate partner = session.Character.Mates.ToList().FirstOrDefault(s => s.IsTeamMember && s.MateType == MateType.Partner && s.PetId == petId);

                                        if (partner?.Sp == null || partner.IsUsingSp)
                                        {
                                            return;
                                        }

                                        if (partner.Sp.GetSkillsCount() < 1)
                                        {
                                            return;
                                        }

                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);

                                        partner.Sp.ClearSkills();
                                        partner.Sp.FullXp();

                                        session.SendPacket(UserInterfaceHelper.GenerateModal(Language.Instance.GetMessageFromKey("PSP_ALL_SKILLS_RESETTED"), 1));

                                        session.SendPacket(partner.GenerateScPacket());
                                    }
                                    break;

                                #region Flower Quest
                                case 1087:
                                    if (ServerManager.Instance.ChannelId == 51)
                                    {
                                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_PERMITTED"), 0));
                                        return;
                                    }
                                    if (ServerManager.Instance.FlowerQuestId != null)
                                    {
                                        session.Character.AddQuest((long)ServerManager.Instance.FlowerQuestId);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;
                                #endregion

                                // Event Upgrade Scrolls
                                case 5107:
                                case 5207:
                                case 5519:
                                    if (EffectValue != 0)
                                    {
                                        if (session.Character.IsSitting)
                                        {
                                            session.Character.IsSitting = false;
                                            session.SendPacket(session.Character.GenerateRest());
                                        }

                                        session.SendPacket(UserInterfaceHelper.GenerateGuri(12, 1,
                                            session.Character.CharacterId, EffectValue));
                                    }
                                    break;

                                case 5511:
                                    {
                                        session.Character.GeneralLogs.Where(s => s.LogType == "InstanceEntry" && (short.Parse(s.LogData) == 16 || short.Parse(s.LogData) == 17) && s.Timestamp.Date == DateTime.Today).ToList().ForEach(s =>
                                        {
                                            s.LogType = "NulledInstanceEntry";
                                            DAOFactory.GeneralLogDAO.InsertOrUpdate(ref s);
                                        });
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        return;
                                    }

                                case 11122: //Autoloot item
                                    {
                                        if (!session.Character.StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.AutoLoot))
                                        {
                                            session.Character.StaticBonusList.Add(new StaticBonusDTO
                                            {
                                                CharacterId = session.Character.CharacterId,
                                                DateEnd = DateTime.Now.AddDays(30),
                                                StaticBonusType = StaticBonusType.AutoLoot
                                            });
                                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                            session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("EFFECT_ACTIVATED"), Name), 12));
                                        }
                                    }
                                    break;

                                case 5120: // click to get fixed amount of fxp, don't know how to make it live-update in (%) tab
                                    {
                                        if (session.Character.Family.FamilyLevel == 20)
                                        {
                                            session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_PERMITTED"), 0));
                                            return;
                                        }

                                        session.Character.GenerateFamilyXp(1000);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.Family.InsertFamilyLog(FamilyLogType.FamilyXP, session.Character.Name, experience: 1000, message: "using fxp boost");
                                    }
                                    break;

                                case 1254:
                                    {
                                        if (!session.Character.Buff.ContainsKey(146))
                                        {
                                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                            session.Character.AddStaticBuff(new StaticBuffDTO { CardId = 146 });
                                        }
                                        else
                                        {
                                            session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));

                                        }
                                    }
                                    break;

                                #region PvP Capsules
                                case 1455: // PvP Attack Capsule
                                    {
                                        if (ServerManager.Instance.ChannelId == 51 && !session.Character.Buff.ContainsKey(170))
                                        {
                                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                            session.Character.AddStaticBuff(new StaticBuffDTO { CardId = 170 });
                                        }
                                        else
                                        {
                                            session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_PERMITTED"), 0));
                                            return;
                                        }
                                       
                                    }
                                    break;

                                case 1456: // PvP Defence Capsule
                                    {
                                        if (ServerManager.Instance.ChannelId == 51 && !session.Character.Buff.ContainsKey(171))
                                        {

                                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                            session.Character.AddStaticBuff(new StaticBuffDTO { CardId = 171 });

                                        }
                                        else
                                        {
                                            session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_PERMITTED"), 0));
                                            return;
                                        }
                                    }
                                    break;

                                case 1457: // PvP Resistance Capsule
                                    {
                                        if (ServerManager.Instance.ChannelId == 51 && !session.Character.Buff.ContainsKey(172))
                                        {

                                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                            session.Character.AddStaticBuff(new StaticBuffDTO { CardId = 172 });

                                        }
                                        else
                                        {
                                            session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_PERMITTED"), 0));
                                            return;
                                        }
                                    }
                                    break;
                                #endregion

                                #region Costume sets
                                // Rottweiler costume box set (Perm)
                                case 1518:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(787, 1);
                                        session.Character.GiftAdd(839, 1);
                                    }
                                    break;

                                // Cat siamois costume box set (Perm)
                                case 1519:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(790, 1);
                                        session.Character.GiftAdd(842, 1);
                                    }
                                    break;

                                // Korat costume box set (Perm)
                                case 1526:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(821, 1);
                                        session.Character.GiftAdd(869, 1);
                                    }
                                    break;

                                // Black lion costume box set (Perm)
                                case 1527:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(833, 1);
                                        session.Character.GiftAdd(881, 1);
                                    }
                                    break;

                                // Gold lion costume box set (Perm)
                                case 1528:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(830, 1);
                                        session.Character.GiftAdd(878, 1);
                                    }
                                    break;

                                case 5051: // Aqua Bushtail Costume Set
                                    {
                                        session.Character.GiftAdd(4064, 1);
                                        session.Character.GiftAdd(4065, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        break;
                                    }

                                case 5080: // Christmas Bushtail Costume Set
                                    {
                                        session.Character.GiftAdd(4074, 1);
                                        session.Character.GiftAdd(4077, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        break;
                                    }

                                case 5183: // Black Bushtail Costume Set
                                    {
                                        session.Character.GiftAdd(4107, 1);
                                        session.Character.GiftAdd(4114, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        break;
                                    }

                                case 5184: // Blue Bushtail Costume Set
                                    {
                                        session.Character.GiftAdd(4108, 1);
                                        session.Character.GiftAdd(4115, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        break;
                                    }

                                case 5185: // Green Bushtail Costume Set
                                    {
                                        session.Character.GiftAdd(4109, 1);
                                        session.Character.GiftAdd(4116, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        break;
                                    }

                                case 5186: // Red Bushtail Costume Set
                                    {
                                        session.Character.GiftAdd(4110, 1);
                                        session.Character.GiftAdd(4117, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        break;
                                    }

                                case 5187: // Pink Bushtail Costume Set
                                    {
                                        session.Character.GiftAdd(4111, 1);
                                        session.Character.GiftAdd(4118, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        break;
                                    }

                                case 5188: // Light blue Bushtail Costume Set
                                    {
                                        session.Character.GiftAdd(4112, 1);
                                        session.Character.GiftAdd(4119, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        break;
                                    }

                                case 5189: // Yellow Bushtail Costume Set
                                    {
                                        session.Character.GiftAdd(4113, 1);
                                        session.Character.GiftAdd(4120, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        break;
                                    }

                                case 5190: // Classic Bushtail Costume Set
                                    {
                                        session.Character.GiftAdd(970, 1);
                                        session.Character.GiftAdd(972, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        break;
                                    }

                                case 5302: // Fox Oto Costume Set
                                    {
                                        session.Character.GiftAdd(4177, 1);
                                        session.Character.GiftAdd(4179, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        break;
                                    }

                                // Magic light costume set box
                                case 5358:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(4185, 1);
                                        session.Character.GiftAdd(4181, 1);
                                    }
                                    break;

                                //  Magic Dark costume set box
                                case 5359:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(4187, 1);
                                        session.Character.GiftAdd(4183, 1);
                                    }
                                    break;

                                // Desert costume set box
                                case 5638:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(4317, 1);
                                        session.Character.GiftAdd(4321, 1);
                                    }
                                    break;

                                // Dancing costume set box
                                case 5639:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(4319, 1);
                                        session.Character.GiftAdd(4323, 1);
                                    }
                                    break;

                                // Policeman costume set box
                                case 5599:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(4283, 1);
                                        session.Character.GiftAdd(4285, 1);
                                    }
                                    break;

                                // Nutcracker costume set box
                                case 5878:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(4827, 1);
                                        session.Character.GiftAdd(4829, 1);
                                    }
                                    break;

                                case 5733: // Easter Rabbit Costume Set
                                    {
                                        session.Character.GiftAdd(4429, 1);
                                        session.Character.GiftAdd(4433, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        break;
                                    }

                                case 5572: // Illusionist Costume Set
                                    {
                                        session.Character.GiftAdd(4258, 1);
                                        session.Character.GiftAdd(4260, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        break;
                                    }

                                // Football costume pack permanant
                                case 5441:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(4195, 1);
                                        session.Character.GiftAdd(4196, 1);
                                    }
                                    break;

                                case 5266: // Bunny (f)
                                    {
                                        session.Character.GiftAdd(4142, 1);
                                        session.Character.GiftAdd(4150, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;


                                case 5737: // FAIRY COSTUME SET
                                    {
                                        session.Character.GiftAdd(4439, 1);
                                        session.Character.GiftAdd(4441, 1);
                                        session.Character.GiftAdd(4443, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;

                                case 5716: // FIRE DEVIL SET
                                    {
                                        session.Character.GiftAdd(4409, 1);
                                        session.Character.GiftAdd(4411, 1);
                                        session.Character.GiftAdd(4435, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;

                                case 5816: // ICE WITCH
                                    {
                                        session.Character.GiftAdd(4534, 1);
                                        session.Character.GiftAdd(4536, 1);
                                        session.Character.GiftAdd(4538, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;

                                case 5487: // Whit Tiger set
                                    {
                                        session.Character.GiftAdd(4248, 1);
                                        session.Character.GiftAdd(4256, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;

                                case 5486: // Tiger peluche set
                                    {
                                        session.Character.GiftAdd(4252, 1);
                                        session.Character.GiftAdd(4244, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;

                                case 5610: // Viking
                                    {
                                        session.Character.GiftAdd(4301, 1);
                                        session.Character.GiftAdd(4303, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;

                                case 5412: // Party Set 1
                                    {
                                        session.Character.GiftAdd(4219, 1);
                                        session.Character.GiftAdd(4225, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;

                                case 5413: // Party Set 2
                                    {
                                        session.Character.GiftAdd(4220, 1);
                                        session.Character.GiftAdd(4226, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;

                                case 5414: // Party Set 3
                                    {
                                        session.Character.GiftAdd(4221, 1);
                                        session.Character.GiftAdd(4227, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;

                                case 5604: // Portiere
                                    {
                                        session.Character.GiftAdd(4289, 1);
                                        session.Character.GiftAdd(4287, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;

                                case 5789: // Tropical Set
                                    {
                                        session.Character.GiftAdd(4529, 1);
                                        session.Character.GiftAdd(4527, 1);
                                        session.Character.GiftAdd(4531, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;

                                case 1480: // haloween female
                                    {
                                        session.Character.GiftAdd(4388, 1);
                                        session.Character.GiftAdd(4386, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;

                                case 1481: // haloween male
                                    {
                                        session.Character.GiftAdd(4392, 1);
                                        session.Character.GiftAdd(4390, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;

                                case 5729: // BOX 3
                                    {
                                        session.Character.GiftAdd(4377, 1);
                                        session.Character.GiftAdd(4375, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;

                                case 5736: // BOX 2
                                    {
                                        session.Character.GiftAdd(4367, 1);
                                        session.Character.GiftAdd(4365, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;

                                case 5742: // BOX 5
                                    {
                                        session.Character.GiftAdd(4073, 1);
                                        session.Character.GiftAdd(4074, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;

                                case 5732:
                                    {
                                        session.Character.GiftAdd(4421, 1);
                                        session.Character.GiftAdd(4425, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;


                                case 5265: // Bunny (m)
                                    {
                                        session.Character.GiftAdd(4138, 1);
                                        session.Character.GiftAdd(4146, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;

                                case 5723: // Jaguar Set (Vehicle + Costume)
                                    {
                                        session.Character.GiftAdd(4382, 1);
                                        session.Character.GiftAdd(4384, 1);
                                        session.Character.GiftAdd(5834, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;
                                #endregion

                                // Martial Artist Starter Pack
                                case 5826:
                                {
                                        // Steel Fist
                                        session.Character.GiftAdd(4756, 1, 6, 7);

                                        // Token
                                        session.Character.GiftAdd(4758, 1, 6, 7);

                                        // Trainee Martial Artist's Uniform
                                        session.Character.GiftAdd(4757, 1, 6, 7);

                                    session.Character.GiftAdd(1286, 1);
                                    session.Character.GiftAdd(1249, 1);
                                    session.Character.GiftAdd(1296, 1);
                                    session.Character.GiftAdd(4503, 1);
                                    session.Character.GiftAdd(4504, 1);

                                   for (short itemVNum = 800; itemVNum <= 803; itemVNum++)
                                   {
                                     session.Character.GiftAdd(itemVNum, 1);
                                   }
                                   session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                }
                                break;

                                case 5018: // Weeding box
                                {
                                    session.Character.GiftAdd(4416, 1); // SP
                                    session.Character.GiftAdd(1984, 10);
                                    session.Character.GiftAdd(1985, 10);
                                    session.Character.GiftAdd(1986, 10);
                                    session.Character.GiftAdd(1981, 1); // Cupid's arrow
                                    //session.Character.GiftAdd(1982, 1); // DIVORCE LETTER
                                    session.Character.GiftAdd(982, 1);
                                    session.Character.GiftAdd(982, 1);
                                    session.Character.GiftAdd(986, 1);
                                    session.Character.GiftAdd(986, 1);
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                }
                                break;

                                //Sellaim, Woondine, Eperial, Turik random boxes
                                case 5004:
                                case 5005:
                                case 5006:
                                case 5007:
                                    int rnd100 = ServerManager.RandomNumber(0, 100);
                                    Item Item = inv.Item;
                                    short[] vnums100 = null;
                                    if (rnd100 < 15 && Item.VNum == 5004)
                                    {
                                        vnums100 = new short[] { 274, 1218 };
                                        byte[] counts = { 1, 1, };
                                        int item = ServerManager.RandomNumber(0, 2);
                                        session.Character.GiftAdd(vnums100[item], counts[item]);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    else if (rnd100 < 15 && Item.VNum == 5005)
                                    {
                                        vnums100 = new short[] { 275, 1218 };
                                        byte[] counts = { 1, 1, };
                                        int item = ServerManager.RandomNumber(0, 2);
                                        session.Character.GiftAdd(vnums100[item], counts[item]);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    else if (rnd100 < 15 && Item.VNum == 5006)
                                    {
                                        vnums100 = new short[] { 276, 1218 };
                                        byte[] counts = { 1, 1, };
                                        int item = ServerManager.RandomNumber(0, 2);
                                        session.Character.GiftAdd(vnums100[item], counts[item]);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    else if (rnd100 < 15 && Item.VNum == 5007)
                                    {
                                        vnums100 = new short[] { 277, 1218 };
                                        byte[] counts = { 1, 1, };
                                        int item = ServerManager.RandomNumber(0, 2);
                                        session.Character.GiftAdd(vnums100[item], counts[item]);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    else
                                    {
                                        short[] vnums2 = null;
                                        vnums2 = new short[] { 1904, 1296, 1296, 1122, 2282, 1219, 1119, 2158 };
                                        byte[] counts2 = { 1, 5, 3, 45, 40, 1, 2, 5 };
                                        int item2 = ServerManager.RandomNumber(0, 8);
                                        session.Character.GiftAdd(vnums2[item2], counts2[item2]);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;

                                //Great Sellaim, Great Woondine, Great Eperial, Great Turik random boxes
                                case 5014:
                                case 5015:
                                case 5016:
                                case 5017:
                                    int rnd101 = ServerManager.RandomNumber(0, 100);
                                    Item Item1 = inv.Item;
                                    short[] vnums101 = null;
                                    if (rnd101 < 10 && Item1.VNum == 5014)
                                    {
                                        vnums101 = new short[] { 278, 1218 };
                                        byte[] counts = { 1, 2, };
                                        int item = ServerManager.RandomNumber(0, 2);
                                        session.Character.GiftAdd(vnums101[item], counts[item]);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    else if (rnd101 < 10 && Item1.VNum == 5015)
                                    {
                                        vnums101 = new short[] { 279, 1218 };
                                        byte[] counts = { 1, 2, };
                                        int item = ServerManager.RandomNumber(0, 2);
                                        session.Character.GiftAdd(vnums101[item], counts[item]);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    else if (rnd101 < 10 && Item1.VNum == 5016)
                                    {
                                        vnums101 = new short[] { 280, 1218 };
                                        byte[] counts = { 1, 2, };
                                        int item = ServerManager.RandomNumber(0, 2);
                                        session.Character.GiftAdd(vnums101[item], counts[item]);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    else if (rnd101 < 10 && Item1.VNum == 5017)
                                    {
                                        vnums101 = new short[] { 281, 1218 };
                                        byte[] counts = { 1, 2, };
                                        int item = ServerManager.RandomNumber(0, 2);
                                        session.Character.GiftAdd(vnums101[item], counts[item]);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    else
                                    {
                                        short[] vnums2 = null;
                                        vnums2 = new short[] { 1904, 1296, 1286, 1122, 2282, 1219, 1119, 2158 };
                                        byte[] counts2 = { 2, 10, 6, 90, 80, 2, 2, 10 };
                                        int item2 = ServerManager.RandomNumber(0, 8);
                                        session.Character.GiftAdd(vnums2[item2], counts2[item2]);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;

                                case 5171: // Ancient's Wizard opo box
                                    {
                                        int rnd102 = ServerManager.RandomNumber(0, 100);
                                        Item Item2 = inv.Item;
                                        short[] vnums102 = null;
                                        if (rnd102 < 10 && Item2.VNum == 5171)
                                        {
                                            vnums102 = new short[] { 397, 4085, 4086, 4087, 4088, 4089, 4090, 4091 };
                                            byte[] counts = { 1, 1, 1, 1, 1, 1, 1, 1 };
                                            int item2 = ServerManager.RandomNumber(0, 8);
                                            session.Character.GiftAdd(vnums102[item2], counts[item2]);
                                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        }
                                    }
                                    break;


                                #region Mount boxes
                                // Scooter box
                                case 1926:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(1906, 1);
                                    }
                                    break;

                                // Tapis box
                                case 1927:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(1907, 1);
                                    }
                                    break;

                                // White tiger box
                                case 1966:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(1965, 1);
                                    }
                                    break;

                                // Balai box
                                case 5153:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(5152, 1);
                                    }
                                    break;

                                // Yakari box
                                case 5181:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(5173, 1);
                                    }
                                    break;

                                // Mac Umulonimbus box
                                case 5118:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(5117, 1);
                                    }
                                    break;

                                // Nossi box
                                case 5197:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(5196, 1);
                                    }
                                    break;

                                // Chameau box
                                case 5915:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(5914, 1);
                                    }
                                    break;

                                // Rollers box
                                case 5235:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(5234, 1);
                                    }
                                    break;

                                // VTT box
                                case 5233:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(5232, 1);
                                    }
                                    break;

                                // Skateboard box
                                case 5237:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(5236, 1);
                                    }
                                    break;

                                // Skateboard inivisble box
                                case 5229:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(5228, 1);
                                    }
                                    break;

                                // Snowboard box
                                case 5241:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(5240, 1);
                                    }
                                    break;

                                // Skis box
                                case 5239:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(5238, 1);
                                    }
                                    break;

                                // Ski invisible box
                                case 5227:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(5226, 1);
                                    }
                                    break;

                                // Magic Bone Drake
                                case 5998:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(5997, 1);
                                    }
                                    break;

                                // Aerosurfeur box
                                case 5361:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(5360, 1);
                                    }
                                    break;

                                // Jaguar box
                                case 5835:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(5834, 1);
                                    }
                                    break;

                                // Traineau box
                                case 5713:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(5712, 1);
                                    }
                                    break;
                                // Ski invisible box
                                case 5744:
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.GiftAdd(5743, 1);
                                    }
                                    break;
                                #endregion


                                #region Event Boxes

                                // YT 3TH
                                case 11144:
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    session.Character.GiftAdd(11115, 3);
                                    session.Character.GiftAdd(11116, 3);
                                    session.Character.GiftAdd(11117, 3);
                                    session.Character.GiftAdd(11138, 3);
                                    session.Character.GiftAdd(2282, 150);
                                    session.Character.GiftAdd(1030, 150);
                                    session.Character.GiftAdd(1363, 10);
                                    session.Character.GiftAdd(1364, 10);
                                    session.Character.GiftAdd(5369, 10);
                                    session.Character.GiftAdd(1244, 30);
                                    session.Character.GiftAdd(1904, 3);
                                    session.Character.GiftAdd(5433, 3);
                                    session.Character.GiftAdd(5061, 3);
                                    session.Character.GiftAdd(9397, 1);
                                    break;

                                // YT 2ND
                                case 11145:
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    session.Character.GiftAdd(11115, 5);
                                    session.Character.GiftAdd(11116, 5);
                                    session.Character.GiftAdd(11117, 5);
                                    session.Character.GiftAdd(11138, 5);
                                    session.Character.GiftAdd(2282, 200);
                                    session.Character.GiftAdd(1030, 200);
                                    session.Character.GiftAdd(1363, 20);
                                    session.Character.GiftAdd(1364, 20);
                                    session.Character.GiftAdd(5369, 10);
                                    session.Character.GiftAdd(1244, 50);
                                    session.Character.GiftAdd(1904, 5);
                                    session.Character.GiftAdd(5433, 5);
                                    session.Character.GiftAdd(5060, 1);
                                    session.Character.GiftAdd(9397, 1);
                                    break;

                                // YT 1ST

                                case 11146:
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    session.Character.GiftAdd(11115, 8);
                                    session.Character.GiftAdd(11116, 8);
                                    session.Character.GiftAdd(11117, 8);
                                    session.Character.GiftAdd(11138, 8);
                                    session.Character.GiftAdd(2282, 300);
                                    session.Character.GiftAdd(1030, 300);
                                    session.Character.GiftAdd(1363, 30);
                                    session.Character.GiftAdd(1364, 30);
                                    session.Character.GiftAdd(5369, 20);
                                    session.Character.GiftAdd(1244, 70);
                                    session.Character.GiftAdd(1904, 10);
                                    session.Character.GiftAdd(5433, 8);
                                    session.Character.GiftAdd(5060, 1);
                                    session.Character.GiftAdd(9397, 1);
                                    break;


                                case 5592: // BEACH Costume (EVENT)
                                    {
                                        session.Character.GiftAdd(8249, 1);
                                        session.Character.GiftAdd(8251, 1);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;

                                #endregion

                                #region SPs boxes
                                case 11147:
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    session.Character.GiftAdd(901, 1);
                                    session.Character.GiftAdd(902, 1);
                                    session.Character.GiftAdd(909, 1);
                                    session.Character.GiftAdd(910, 1);
                                    break;

                                case 11148:
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    session.Character.GiftAdd(903, 1);
                                    session.Character.GiftAdd(904, 1);
                                    session.Character.GiftAdd(911, 1);
                                    session.Character.GiftAdd(912, 1);
                                    break;

                                case 11149:
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    session.Character.GiftAdd(905, 1);
                                    session.Character.GiftAdd(906, 1);
                                    session.Character.GiftAdd(913, 1);
                                    session.Character.GiftAdd(914, 1);
                                    break;
                                #endregion

                                #region Special Boxes

                                case 11150: // Premium Starter Pack
                                {
                                    session.Character.GiftAdd(2283, 50);
                                    session.Character.GiftAdd(2284, 30);
                                    session.Character.GiftAdd(1286, 3);
                                    session.Character.GiftAdd(1246, 10);
                                    session.Character.GiftAdd(1247, 10);
                                    session.Character.GiftAdd(1248, 10);
                                    session.Character.GiftAdd(1249, 10);
                                    session.Character.GiftAdd(2037, 25);
                                    session.Character.GiftAdd(2041, 25);
                                    session.Character.GiftAdd(2045, 25);
                                    session.Character.GiftAdd(2049, 25);
                                    session.Character.GiftAdd(2002, 25);
                                    session.Character.GiftAdd(2282, 99);
                                    session.Character.GiftAdd(1030, 99);
                                    session.Character.GiftAdd(1363, 10);
                                    session.Character.GiftAdd(1364, 10);
                                    session.Character.GiftAdd(5014, 3);
                                    session.Character.GiftAdd(5015, 3);
                                    session.Character.GiftAdd(5016, 3);
                                    session.Character.GiftAdd(5017, 3);
                                    session.Character.GiftAdd(5878, 1); //nutcracker
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                }
                                   break;

                                case 11136: // Pefection stones pack
                                {
                                    session.Character.GiftAdd(2514, 50);
                                    session.Character.GiftAdd(2515, 50);
                                    session.Character.GiftAdd(2516, 50);
                                    session.Character.GiftAdd(2517, 50);
                                    session.Character.GiftAdd(2518, 50);
                                    session.Character.GiftAdd(2519, 50);
                                    session.Character.GiftAdd(2520, 50);
                                    session.Character.GiftAdd(2521, 50);
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                }
                                    break;
                            

                                case 5073: // PvP Shells rnd
                                {
                                    session.Character.GiftAdd(573, 1, 7, 0, 0, true, 4);
                                    session.Character.GiftAdd(583, 1, 7, 0, 0, true, 4);
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                }
                                    break;

                                case 1608: // rnd box r7 pvp weap 
                                {
                                    session.Character.GiftAdd(573, 1, 7, 0, 0, false, 7);
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                }
                                    break;

                                case 1606: // rnd box r7 pvp armor
                                {
                                    session.Character.GiftAdd(583, 1, 7, 0, 0, false, 7);
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                }
                                    break;

                                case 5075: // PvP Shells
                                {
                                    session.Character.GiftAdd(573, 1, 7, 0, 0, false, 7);
                                    session.Character.GiftAdd(583, 1, 7, 0, 0, false, 7);
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                }
                                    break;

                                case 5001: // Permanent runes
                                {
                                    session.Character.GiftAdd(4356, 1);
                                    session.Character.GiftAdd(4357, 1);
                                    session.Character.GiftAdd(4358, 1);
                                    session.Character.GiftAdd(4359, 1);
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                }
                                    break;

                                #endregion

                                case 9678: // Fill glacernon raid bar
                                    {
                                        if (ServerManager.Instance.ChannelId == 51 && ServerManager.Instance.Act4DemonStat.Mode == 0 && ServerManager.Instance.Act4AngelStat.Mode == 0)                         
                                        {
                                            switch (session.Character.Faction)
                                            {
                                                case FactionType.Angel:
                                                    {
                                                        ServerManager.Instance.Act4AngelStat.Percentage += 2000; //20%
                                                    }
                                                    break;

                                                case FactionType.Demon:
                                                    {
                                                        ServerManager.Instance.Act4DemonStat.Percentage += 2000;
                                                    }
                                                    break;

                                            }
                                        }
                                        else
                                        {
                                            session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_PERMITTED"), 0));
                                            return;
                                        }
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;


                               #region Tier Chests
                               case 1507: // Tier I chest
                               {
                                    session.Character.GiftAdd(2282, 20);
                                    session.Character.GiftAdd(2283, 20);
                                    session.Character.GiftAdd(1030, 20);
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                               }
                               break;

                               case 1508: // Tier II chest
                               {
                                    session.Character.GiftAdd(2282, 45);
                                    session.Character.GiftAdd(2283, 45);
                                    session.Character.GiftAdd(1030, 45);
                                    session.Character.GiftAdd(1363, 3);
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                               }
                               break;

                               case 1506: // Tier III chest
                               {
                                     session.Character.GiftAdd(2282, 70);
                                     session.Character.GiftAdd(2283, 70);
                                     session.Character.GiftAdd(2284, 70);
                                     session.Character.GiftAdd(2282, 70);
                                     session.Character.GiftAdd(1363, 5);
                                     session.Character.GiftAdd(1364, 5);
                                     session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                               }
                               break;

                               case 1583: // Tier IV chest
                               {
                                        session.Character.GiftAdd(2282, 90);
                                        session.Character.GiftAdd(1030, 90);
                                        session.Character.GiftAdd(2284, 90);
                                        session.Character.GiftAdd(2285, 45);
                                        session.Character.GiftAdd(1363, 5);
                                        session.Character.GiftAdd(1364, 5);
                                        session.Character.GiftAdd(5369, 5);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                               }
                               break;

                                case 11141: // Tier V chest
                                    {
                                        session.Character.GiftAdd(2282, 120);
                                        session.Character.GiftAdd(1030, 120);
                                        session.Character.GiftAdd(2285, 70);
                                        session.Character.GiftAdd(11138, 2);
                                        session.Character.GiftAdd(1364, 8);
                                        session.Character.GiftAdd(5369, 8);
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    }
                                    break;
                                #endregion

                                // Soulstone Blessing
                                case 1362:
                                case 5195:
                                case 5211:
                                case 9075:
                                    if (!session.Character.Buff.ContainsKey(146))
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                        session.Character.AddStaticBuff(new StaticBuffDTO {CardId = 146});
                                    }
                                    else
                                    {
                                        session.SendPacket(
                                            UserInterfaceHelper.GenerateMsg(
                                                Language.Instance.GetMessageFromKey("ITEM_IN_USE"), 0));
                                    }

                                    break;

                                case 1428:
                                    session.SendPacket("guri 18 1");
                                    break;

                                case 1429:
                                    session.SendPacket("guri 18 0");
                                    break;

                                case 1904:
                                    short[] items = {1894, 1895, 1896, 1897, 1898, 1899, 1900, 1901, 1902, 1903};
                                    for (var i = 0; i < 5; i++)
                                    {
                                        session.Character.GiftAdd(items[ServerManager.RandomNumber(0, items.Length)], 1);

                                    }
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    break;

                                case 5370:
                                    if (session.Character.Buff.Any(s => s.Card.CardId == 393) || session.Character.Buff.Any(s => s.Card.CardId == 4047))
                                    {
                                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("ALREADY_GOT_BUFF"), session.Character.Buff.FirstOrDefault(s => s.Card.CardId == 393)?.Card.Name), 10));
                                        return;
                                    }
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    session.Character.AddStaticBuff(new StaticBuffDTO {CardId = 393});
                                    break;

                                case 11117:
                                    if (session.Character.Buff.Any(s => s.Card.CardId == 4047) || session.Character.Buff.Any(s => s.Card.CardId == 393))
                                    {
                                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("ALREADY_GOT_BUFF"), session.Character.Buff.FirstOrDefault(s => s.Card.CardId == 4047)?.Card.Name), 10));
                                        return;
                                    }
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    session.Character.AddStaticBuff(new StaticBuffDTO { CardId = 4047 });
                                    break;

                                case 5841:
                                    var rnd = ServerManager.RandomNumber(0, 1000);
                                    short[] vnums = null;
                                    if (rnd < 900)
                                    {
                                        vnums = new short[] {4356, 4357, 4358, 4359};
                                    }
                                    else
                                    {
                                        vnums = new short[] {4360, 4361, 4362, 4363};
                                    }

                                    session.Character.GiftAdd(vnums[ServerManager.RandomNumber(0, 4)], 1);
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    break;

                                case 5916:
                                case 5927:
                                    session.Character.AddStaticBuff(new StaticBuffDTO
                                    {
                                        CardId = 340,
                                        CharacterId = session.Character.CharacterId,
                                        RemainingTime = 7200
                                    });
                                    session.Character.RemoveBuff(339);
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    break;

                                case 5929:
                                case 5930:
                                    session.Character.AddStaticBuff(new StaticBuffDTO
                                    {
                                        CardId = 340,
                                        CharacterId = session.Character.CharacterId,
                                        RemainingTime = 600
                                    });
                                    session.Character.RemoveBuff(339);
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    break;

                                // Mother Nature's Rune Pack (limited)
                                case 9117:
                                    rnd = ServerManager.RandomNumber(0, 1000);
                                    vnums = null;
                                    if (rnd < 900)
                                    {
                                        vnums = new short[] {8312, 8313, 8314, 8315};
                                    }
                                    else
                                    {
                                        vnums = new short[] {8316, 8317, 8318, 8319};
                                    }

                                    session.Character.GiftAdd(vnums[ServerManager.RandomNumber(0, 4)], 1);
                                    session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                                    break;

                                default:
                                    var roll = DAOFactory.RollGeneratedItemDAO.LoadByItemVNum(VNum);
                                    IEnumerable<RollGeneratedItemDTO> rollGeneratedItemDtos =
                                        roll as IList<RollGeneratedItemDTO> ?? roll.ToList();
                                    if (!rollGeneratedItemDtos.Any())
                                    {
                                        Logger.Warn(string.Format(
                                            Language.Instance.GetMessageFromKey("NO_HANDLER_ITEM"), GetType(), VNum,
                                            Effect, EffectValue));
                                        return;
                                    }

                                    var probabilities = rollGeneratedItemDtos.Where(s => s.Probability != 10000)
                                        .Sum(s => s.Probability);
                                    var rnd2 = ServerManager.RandomNumber(0, probabilities);
                                    var currentrnd = 0;
                                    foreach (var rollitem in rollGeneratedItemDtos.Where(s => s.Probability == 10000))
                                    {
                                        sbyte rare = 0;
                                        if (rollitem.IsRareRandom)
                                        {
                                            rnd = ServerManager.RandomNumber();

                                            for (var j = ItemHelper.RareRate.Length - 1; j >= 0; j--)
                                            {
                                                if (rnd < ItemHelper.RareRate[j])
                                                {
                                                    rare = (sbyte) j;
                                                    break;
                                                }
                                            }

                                            if (rare < 1)
                                            {
                                                rare = 1;
                                            }
                                        }

                                        session.Character.GiftAdd(rollitem.ItemGeneratedVNum,
                                            rollitem.ItemGeneratedAmount, (byte) rare,
                                            design: rollitem.ItemGeneratedDesign);
                                    }

                                    foreach (var rollitem in rollGeneratedItemDtos.Where(s => s.Probability != 10000)
                                        .OrderBy(s => ServerManager.RandomNumber()))
                                    {
                                        sbyte rare = 0;
                                        if (rollitem.IsRareRandom)
                                        {
                                            rnd = ServerManager.RandomNumber();

                                            for (var j = ItemHelper.RareRate.Length - 1; j >= 0; j--)
                                            {
                                                if (rnd < ItemHelper.RareRate[j])
                                                {
                                                    rare = (sbyte) j;
                                                    break;
                                                }
                                            }

                                            if (rare < 1)
                                            {
                                                rare = 1;
                                            }

                                            if (rollitem.ItemGeneratedVNum == 573 ||
                                                rollitem.ItemGeneratedVNum == 583 &&
                                                rollitem.MaximumOriginalItemRare == 7)
                                            {
                                                CommunicationServiceClient.Instance.SendMessageToCharacter(
                                                        new SCSCharacterMessage
                                                        {
                                                                DestinationCharacterId = null,
                                                                SourceCharacterId      = session.Character.CharacterId,
                                                                SourceWorldId          = ServerManager.Instance.WorldId,
                                                                Message                = Language.Instance.GetMessageFromKey("FOUND_R7_SHELL"),
                                                                Type                   = MessageType.Family
                                                        });
                                            }
                                        }

                                        currentrnd += rollitem.Probability;
                                        if (currentrnd < rnd2)
                                        {
                                            continue;
                                        }

                                        /*if (rollitem.IsSuperReward)
                                        {
                                            CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                                            {
                                                DestinationCharacterId = null,
                                                SourceCharacterId = session.Character.CharacterId,
                                                SourceWorldId = ServerManager.Instance.WorldId,
                                                Message = Language.Instance.GetMessageFromKey("SUPER_REWARD"),
                                                Type = MessageType.Shout
                                            });
                                        }*/
                                        session.Character.GiftAdd(rollitem.ItemGeneratedVNum,
                                            rollitem.ItemGeneratedAmount, (byte) rare,
                                            design: rollitem.ItemGeneratedDesign); //, rollitem.ItemGeneratedUpgrade);
                                        break;
                                    }

                                    session.Character.Inventory.RemoveItemAmount(VNum);
                                    break;
                            }

                            break;
                    }

                    break;
            }

            session.Character.IncrementQuests(QuestType.Use, inv.ItemVNum);
        }

        #endregion
    }
}