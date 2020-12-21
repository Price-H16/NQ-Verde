using System;
using System.Collections.Generic;
using System.Linq;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject.Helpers
{
    public class UserInterfaceHelper
    {
        #region Members

        private static UserInterfaceHelper _instance;

        #endregion

        #region Properties

        public static UserInterfaceHelper Instance => _instance ?? (_instance = new UserInterfaceHelper());

        #endregion

        #region Methods

        public static string GenerateBazarRecollect(long pricePerUnit, int soldAmount, int amount, long taxes, long totalPrice, long name) => $"rc_scalc 1 {pricePerUnit} {soldAmount} {amount} {taxes} {totalPrice} {name}";

        public static string GenerateBSInfo(byte mode, short title, short time, short text) => $"bsinfo {mode} {title} {time} {text}";

        public static string GenerateCHDM(int maxhp, int angeldmg, int demondmg, int time) => $"ch_dm {maxhp} {angeldmg} {demondmg} {time}";

        public static string GenerateDelay(int delay, int type, string argument) => $"delay {delay} {type} {argument}";

        public static string GenerateDialog(string dialog) => $"dlg {dialog}";

        public static string GenerateFrank(byte type, ClientSession session)
        {
            var packet = "frank_stc";
            var i = 0;
            List<Family> familyordered = null;

            switch (type)
            {
                case 0:
                case 2:
                case 3:
                    packet += " 0";
                    break;

                case 4:
                case 6:
                case 7:
                    packet += " 1";
                    break;

                case 8:
                case 9:
                    packet += " 2";
                    break;

                default:
                    return string.Empty;
            }

            switch (type)
            {
                case 0:
                    familyordered = ServerManager.Instance.FamilyList.GetAllItems().OrderByDescending(
                        s => s.FamilyExperience).ToList();
                    break;

                case 2:
                    familyordered = ServerManager.Instance.FamilyList.GetAllItems().Where(a => a.FamilyFaction == 1)
                        .OrderByDescending(
                            s => s.FamilyExperience).ToList();
                    break;

                case 3:
                    familyordered = ServerManager.Instance.FamilyList.GetAllItems().Where(a => a.FamilyFaction == 2)
                        .OrderByDescending(
                            s => s.FamilyExperience).ToList();
                    break;

                case 4:
                    familyordered = ServerManager.Instance.FamilyList.GetAllItems().OrderByDescending(
                        s => s.FamilyLogs.Where(l =>
                                l.FamilyLogType == FamilyLogType.FamilyXP && l.Timestamp.AddDays(30) < DateTime.Now)
                            .ToList().Sum(c => long.Parse(c.FamilyLogData.Split('|')[1]))).ToList();
                    break;

                case 6:
                    familyordered = ServerManager.Instance.FamilyList.GetAllItems().Where(a => a.FamilyFaction == 1)
                        .OrderByDescending(
                            s => s.FamilyLogs.Where(l =>
                                    l.FamilyLogType == FamilyLogType.FamilyXP && l.Timestamp.AddDays(30) < DateTime.Now)
                                .ToList().Sum(c => long.Parse(c.FamilyLogData.Split('|')[1]))).ToList();
                    break;

                case 7:
                    familyordered = ServerManager.Instance.FamilyList.GetAllItems().Where(a => a.FamilyFaction == 2)
                        .OrderByDescending(
                            s => s.FamilyLogs.Where(l =>
                                    l.FamilyLogType == FamilyLogType.FamilyXP && l.Timestamp.AddDays(30) < DateTime.Now)
                                .ToList().Sum(c => long.Parse(c.FamilyLogData.Split('|')[1]))).ToList();
                    break;

                case 8:
                    familyordered = ServerManager.Instance.FamilyList.GetAllItems().OrderByDescending(
                        s => s.FamilyExperience).ToList();
                    break;

                case 9:
                    familyordered = ServerManager.Instance.FamilyList.GetAllItems().OrderByDescending(
                        s => s.FamilyCharacters.Sum(c => c.Character.Reputation)).ToList();
                    break;
            }

            if (familyordered != null)
            {
                session.Character.GenerateFmRank(type);
                foreach (var fam in familyordered.Take(100))
                {
                    i++;
                    switch (type)
                    {
                        case 0:
                            packet += $" {i}|{fam.Name}|{fam.FamilyLevel}|{fam.FamilyExperience}";
                            break;

                        case 2:
                            packet +=
                                $" {i}|{fam.Name}|{fam.FamilyLevel}|{fam.FamilyCharacters.Sum(c => c.Character.Act4Points)}";
                            break;

                        case 3:
                            packet +=
                                $" {i}|{fam.Name}|{fam.FamilyLevel}|{fam.FamilyCharacters.Sum(c => c.Character.Act4Points)}";
                            break;

                        case 4:
                            packet += $" {i}|{fam.Name}|{fam.FamilyLevel}|{fam.FamilyExperience}";
                            break;

                        case 6:
                            packet +=
                                $" {i}|{fam.Name}|{fam.FamilyLevel}|{fam.FamilyCharacters.Sum(c => c.Character.Act4Points)}";
                            break;

                        case 7:
                            packet +=
                                $" {i}|{fam.Name}|{fam.FamilyLevel}|{fam.FamilyCharacters.Sum(c => c.Character.Act4Points)}";
                            break;

                        case 8:
                            packet += $" {i}|{fam.Name}|{fam.FamilyLevel}|{fam.FamilyExperience}";
                            break;

                        case 9:
                            packet +=
                                $" {i}|{fam.Name}|{fam.FamilyLevel}|{fam.FamilyCharacters.Sum(c => c.Character.Reputation)}";
                            break;
                    }
                }
            }

            return packet;
        }

        public static string GenerateGuri(byte type, byte argument, long callerId, int value = 0, int value2 = 0)
        {
            switch (type)
            {
                case 2:
                    return $"guri 2 {argument} {callerId}";

                case 6:
                    return $"guri 6 1 {callerId} 0 0";

                case 10:
                    return $"guri 10 {argument} {value} {callerId}";

                case 15:
                    return $"guri 15 {argument} 0 0";

                case 31:
                    return $"guri 31 {argument} {callerId} {value} {value2}";

                default:
                    return $"guri {type} {argument} {callerId} {value}";
            }
        }

        public static string GenerateInbox(string value) => $"inbox {value}";

        public static string GenerateInfo(string message) => $"info {message}";

        public static string GenerateMapOut() => "mapout";

        public static string GenerateModal(string message, int type) => $"modal {type} {message}";

        public static string GenerateMsg(string message, int type) => $"msg {type} {message}";

        public static string GeneratePClear() => "p_clear";

        public static string GenerateRCBList(CBListPacket packet)
        {
            if (packet == null || packet.ItemVNumFilter == null)
            {
                return "";
            }
            string itembazar = "";

            List<string> itemssearch = packet.ItemVNumFilter == "0" ? new List<string>() : packet.ItemVNumFilter.Split(' ').ToList();
            List<BazaarItemLink> bzlist = new List<BazaarItemLink>();
            BazaarItemLink[] billist = new BazaarItemLink[ServerManager.Instance.BazaarList.Count + 20];
            ServerManager.Instance.BazaarList.Where(s => s != null).CopyTo(billist);
            try
            {
                foreach (BazaarItemLink bz in billist)
                {
                    if (bz?.Item == null || ServerManager.Instance.BannedCharacters.Contains(bz.Item.CharacterId))
                    {
                        continue;
                    }

                    switch (packet.TypeFilter)
                    {
                        case BazaarListType.Weapon:
                            if (bz.Item.Item.Type == InventoryType.Equipment && bz.Item.Item.ItemType == ItemType.Weapon && (packet.SubTypeFilter == 0 || ((bz.Item.Item.Class + 1 >> packet.SubTypeFilter) & 1) == 1) && ((packet.LevelFilter == 0 || (packet.LevelFilter == 11 && bz.Item.Item.IsHeroic) || (bz.Item.Item.LevelMinimum < (packet.LevelFilter * 10) + 1 && bz.Item.Item.LevelMinimum >= (packet.LevelFilter * 10) - 9)) && ((packet.RareFilter == 0 || packet.RareFilter == bz.Item.Rare + 1) && (packet.UpgradeFilter == 0 || packet.UpgradeFilter == bz.Item.Upgrade + 1))))
                            {
                                bzlist.Add(bz);
                            }
                            break;

                        case BazaarListType.Armor:
                            if (bz.Item.Item.Type == InventoryType.Equipment && bz.Item.Item.ItemType == ItemType.Armor && (packet.SubTypeFilter == 0 || ((bz.Item.Item.Class + 1 >> packet.SubTypeFilter) & 1) == 1) && ((packet.LevelFilter == 0 || (packet.LevelFilter == 11 && bz.Item.Item.IsHeroic) || (bz.Item.Item.LevelMinimum < (packet.LevelFilter * 10) + 1 && bz.Item.Item.LevelMinimum >= (packet.LevelFilter * 10) - 9)) && ((packet.RareFilter == 0 || packet.RareFilter == bz.Item.Rare + 1) && (packet.UpgradeFilter == 0 || packet.UpgradeFilter == bz.Item.Upgrade + 1))))
                            {
                                bzlist.Add(bz);
                            }
                            break;

                        case BazaarListType.Equipment:
                            if (bz.Item.Item.Type == InventoryType.Equipment && bz.Item.Item.ItemType == ItemType.Fashion && ((packet.SubTypeFilter == 0 || (packet.SubTypeFilter == 2 && bz.Item.Item.EquipmentSlot == EquipmentType.Mask) || ((packet.SubTypeFilter == 1 && bz.Item.Item.EquipmentSlot == EquipmentType.Hat) || (packet.SubTypeFilter == 6 && bz.Item.Item.EquipmentSlot == EquipmentType.CostumeHat) || (packet.SubTypeFilter == 5 && bz.Item.Item.EquipmentSlot == EquipmentType.CostumeSuit) || (packet.SubTypeFilter == 3 && bz.Item.Item.EquipmentSlot == EquipmentType.Gloves) || (packet.SubTypeFilter == 4 && bz.Item.Item.EquipmentSlot == EquipmentType.Boots))) && (packet.LevelFilter == 0 || (packet.LevelFilter == 11 && bz.Item.Item.IsHeroic) || (bz.Item.Item.LevelMinimum < (packet.LevelFilter * 10) + 1 && bz.Item.Item.LevelMinimum >= (packet.LevelFilter * 10) - 9))))
                            {
                                bzlist.Add(bz);
                            }
                            break;

                        case BazaarListType.Jewelery:
                            if (bz.Item.Item.Type == InventoryType.Equipment && bz.Item.Item.ItemType == ItemType.Jewelery && ((packet.SubTypeFilter == 0 || (packet.SubTypeFilter == 2 && bz.Item.Item.EquipmentSlot == EquipmentType.Ring) || (packet.SubTypeFilter == 1 && bz.Item.Item.EquipmentSlot == EquipmentType.Necklace) || (packet.SubTypeFilter == 5 && bz.Item.Item.EquipmentSlot == EquipmentType.Amulet) || (packet.SubTypeFilter == 3 && bz.Item.Item.EquipmentSlot == EquipmentType.Bracelet) || (packet.SubTypeFilter == 4 && (bz.Item.Item.EquipmentSlot == EquipmentType.Fairy || (bz.Item.Item.ItemType == ItemType.Box && bz.Item.Item.ItemSubType == 5)))) && (packet.LevelFilter == 0 || (packet.LevelFilter == 11 && bz.Item.Item.IsHeroic) || (bz.Item.Item.LevelMinimum < (packet.LevelFilter * 10) + 1 && bz.Item.Item.LevelMinimum >= (packet.LevelFilter * 10) - 9))))
                            {
                                bzlist.Add(bz);
                            }
                            break;

                        case BazaarListType.Specialist:
                            if (bz.Item.Item.Type == InventoryType.Equipment && bz.Item.Item.ItemType == ItemType.Box && bz.Item.Item.ItemSubType == 2)
                            {
                                if (packet.SubTypeFilter == 0 && ((packet.LevelFilter == 0 || (bz.Item.SpLevel < (packet.LevelFilter * 10) + 1 && bz.Item.SpLevel >= (packet.LevelFilter * 10) - 9)) && ((packet.UpgradeFilter == 0 || packet.UpgradeFilter == bz.Item.Upgrade + 1) && (packet.SubTypeFilter == 0 || (packet.SubTypeFilter == 1 && bz.Item.HoldingVNum == 0) || (packet.SubTypeFilter == 2 && bz.Item.HoldingVNum != 0)))))
                                {
                                    bzlist.Add(bz);
                                }
                                else if (bz.Item.HoldingVNum == 0 && (packet.SubTypeFilter == 1 && ((packet.LevelFilter == 0 || (bz.Item.SpLevel < (packet.LevelFilter * 10) + 1 && bz.Item.SpLevel >= (packet.LevelFilter * 10) - 9)) && ((packet.UpgradeFilter == 0 || packet.UpgradeFilter == bz.Item.Upgrade + 1) && (packet.SubTypeFilter == 0 || (packet.SubTypeFilter == 1 && bz.Item.HoldingVNum == 0) || (packet.SubTypeFilter == 2 && bz.Item.HoldingVNum != 0))))))
                                {
                                    bzlist.Add(bz);
                                }
                                else if ((packet.SubTypeFilter == 2 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 10) || (packet.SubTypeFilter == 3 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 11) || (packet.SubTypeFilter == 4 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 2) || (packet.SubTypeFilter == 5 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 3) || (packet.SubTypeFilter == 6 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 13) || (packet.SubTypeFilter == 7 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 5) || (packet.SubTypeFilter == 8 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 12) || (packet.SubTypeFilter == 9 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 4) || (packet.SubTypeFilter == 10 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 7) || (packet.SubTypeFilter == 11 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 15) || (packet.SubTypeFilter == 12 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 6) || (packet.SubTypeFilter == 13 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 14) || (packet.SubTypeFilter == 14 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 9) || (packet.SubTypeFilter == 15 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 8) || (packet.SubTypeFilter == 16 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 1) || (packet.SubTypeFilter == 17 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 16) || (packet.SubTypeFilter == 18 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 17) || ((packet.SubTypeFilter == 19 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 18) || (packet.SubTypeFilter == 20 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 19) || (packet.SubTypeFilter == 21 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 20) || (packet.SubTypeFilter == 22 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 21) || (packet.SubTypeFilter == 23 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 22) || (packet.SubTypeFilter == 24 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 23) || (packet.SubTypeFilter == 25 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 24) || (packet.SubTypeFilter == 26 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 25) || (packet.SubTypeFilter == 27 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 26) || (packet.SubTypeFilter == 28 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 27) || (packet.SubTypeFilter == 29 && ServerManager.GetItem(bz.Item.HoldingVNum).Morph == 28)))
                                {
                                    if ((packet.LevelFilter == 0 || (bz.Item.SpLevel < (packet.LevelFilter * 10) + 1 && bz.Item.SpLevel >= (packet.LevelFilter * 10) - 9)) && ((packet.UpgradeFilter == 0 || packet.UpgradeFilter == bz.Item.Upgrade + 1) && (packet.SubTypeFilter == 0 || (packet.SubTypeFilter == 1 && bz.Item.HoldingVNum == 0) || (packet.SubTypeFilter >= 2 && bz.Item.HoldingVNum != 0))))
                                    {
                                        bzlist.Add(bz);
                                    }
                                }
                            }
                            break;

                        case BazaarListType.Pet:
                            if (bz.Item.Item.Type == InventoryType.Equipment && bz.Item.Item.ItemType == ItemType.Box && bz.Item.Item.ItemSubType == 0 && (packet.LevelFilter == 0 || (bz.Item.SpLevel < (packet.LevelFilter * 10) + 1 && bz.Item.SpLevel >= (packet.LevelFilter * 10) - 9)) && (packet.SubTypeFilter == 0 || (packet.SubTypeFilter == 1 && bz.Item.HoldingVNum == 0) || (packet.SubTypeFilter == 2 && bz.Item.HoldingVNum != 0)))
                            {
                                bzlist.Add(bz);
                            }
                            break;

                        case BazaarListType.Npc:
                            if (bz.Item.Item.Type == InventoryType.Equipment && bz.Item.Item.ItemType == ItemType.Box && (bz.Item.Item.ItemSubType == 1 || bz.Item.Item.ItemSubType == 6) && (packet.LevelFilter == 0 || (bz.Item.SpLevel < (packet.LevelFilter * 10) + 1 && bz.Item.SpLevel >= (packet.LevelFilter * 10) - 9)) && (packet.SubTypeFilter == 0 || (packet.SubTypeFilter == 1 && bz.Item.HoldingVNum == 0) || (packet.SubTypeFilter == 2 && bz.Item.HoldingVNum != 0)))
                            {
                                bzlist.Add(bz);
                            }
                            break;

                        case BazaarListType.Shell:
                            if (bz.Item.Item.Type == InventoryType.Equipment && bz.Item.Item.ItemType == ItemType.Shell && (packet.SubTypeFilter == 0 || bz.Item.Item.ItemSubType == bz.Item.Item.ItemSubType + 1) && ((packet.RareFilter == 0 || packet.RareFilter == bz.Item.Rare + 1) && (packet.LevelFilter == 0 || (bz.Item.SpLevel < (packet.LevelFilter * 10) + 1 && bz.Item.SpLevel >= (packet.LevelFilter * 10) - 9))))
                            {
                                bzlist.Add(bz);
                            }
                            break;

                        case BazaarListType.Main:
                            if (bz.Item.Item.Type == InventoryType.Main && (packet.SubTypeFilter == 0 || (packet.SubTypeFilter == 1 && bz.Item.Item.ItemType == ItemType.Main) || (packet.SubTypeFilter == 2 && bz.Item.Item.ItemType == ItemType.Upgrade) || (packet.SubTypeFilter == 3 && bz.Item.Item.ItemType == ItemType.Production) || (packet.SubTypeFilter == 4 && bz.Item.Item.ItemType == ItemType.Special) || (packet.SubTypeFilter == 5 && bz.Item.Item.ItemType == ItemType.Potion) || (packet.SubTypeFilter == 6 && bz.Item.Item.ItemType == ItemType.Event)))
                            {
                                bzlist.Add(bz);
                            }
                            break;

                        case BazaarListType.Usable:
                            if (bz.Item.Item.Type == InventoryType.Etc && (packet.SubTypeFilter == 0 || (packet.SubTypeFilter == 1 && bz.Item.Item.ItemType == ItemType.Food) || ((packet.SubTypeFilter == 2 && bz.Item.Item.ItemType == ItemType.Snack) || (packet.SubTypeFilter == 3 && bz.Item.Item.ItemType == ItemType.Magical) || (packet.SubTypeFilter == 4 && bz.Item.Item.ItemType == ItemType.Part) || (packet.SubTypeFilter == 5 && bz.Item.Item.ItemType == ItemType.Teacher) || (packet.SubTypeFilter == 6 && bz.Item.Item.ItemType == ItemType.Sell))))
                            {
                                bzlist.Add(bz);
                            }
                            break;

                        case BazaarListType.Other:
                            if (bz.Item.Item.Type == InventoryType.Equipment && bz.Item.Item.ItemType == ItemType.Box && !bz.Item.Item.IsHolder)
                            {
                                bzlist.Add(bz);
                            }
                            break;

                        case BazaarListType.Vehicle:
                            if (bz.Item.Item.ItemType == ItemType.Box && bz.Item.Item.ItemSubType == 4 && (packet.SubTypeFilter == 0 || (packet.SubTypeFilter == 1 && bz.Item.HoldingVNum == 0) || (packet.SubTypeFilter == 2 && bz.Item.HoldingVNum != 0)))
                            {
                                bzlist.Add(bz);
                            }
                            break;

                        default:
                            bzlist.Add(bz);
                            break;
                    }
                }
                List<BazaarItemLink> bzlistsearched = bzlist.Where(s => itemssearch.Contains(s.Item.ItemVNum.ToString())).ToList();

                //price up price down quantity up quantity down
                List<BazaarItemLink> definitivelist = itemssearch.Count > 0 ? bzlistsearched : bzlist;
                switch (packet.OrderFilter)
                {
                    case 0:
                        definitivelist = definitivelist.OrderBy(s => s.Item.Item.Name).ThenBy(s => s.BazaarItem.Price).ToList();
                        break;

                    case 1:
                        definitivelist = definitivelist.OrderBy(s => s.Item.Item.Name).ThenByDescending(s => s.BazaarItem.Price).ToList();
                        break;

                    case 2:
                        definitivelist = definitivelist.OrderBy(s => s.Item.Item.Name).ThenBy(s => s.BazaarItem.Amount).ToList();
                        break;

                    case 3:
                        definitivelist = definitivelist.OrderBy(s => s.Item.Item.Name).ThenByDescending(s => s.BazaarItem.Amount).ToList();
                        break;

                    default:
                        definitivelist = definitivelist.OrderBy(s => s.Item.Item.Name).ToList();
                        break;
                }
                foreach (BazaarItemLink bzlink in definitivelist.Where(s => (s.BazaarItem.DateStart.AddHours(s.BazaarItem.Duration) - DateTime.Now).TotalMinutes > 0 && s.Item.Amount > 0).Skip(packet.Index * 50).Take(50))
                {
                    long time = (long)(bzlink.BazaarItem.DateStart.AddHours(bzlink.BazaarItem.Duration) - DateTime.Now).TotalMinutes;
                    string info = "";
                    if (bzlink.Item.Item.Type == InventoryType.Equipment)
                    {
                        //if (bzlink.Item is ItemInstance wear) / Dupe Shells
                        //{
                        //    wear.ShellEffects.Clear();
                        //    wear.ShellEffects.AddRange(DAOFactory.ShellEffectDAO.LoadByEquipmentSerialId(wear.EquipmentSerialId));
                        //}
                        info = (bzlink.Item.Item.EquipmentSlot != EquipmentType.Sp ?
                            bzlink.Item?.GenerateEInfo() : bzlink.Item.Item.SpType == 0 && bzlink.Item.Item.ItemSubType == 4 ?
                            bzlink.Item?.GeneratePslInfo() : bzlink.Item?.GenerateSlInfo()).Replace(' ', '^').Replace("slinfo^", "").Replace("e_info^", "");
                    }
                    itembazar += $"{bzlink.BazaarItem.BazaarItemId}|{bzlink.BazaarItem.SellerId}|{bzlink.Owner}|{bzlink.Item.Item.VNum}|{bzlink.Item.Amount}|{(bzlink.BazaarItem.IsPackage ? 1 : 0)}|{bzlink.BazaarItem.Price}|{time}|2|0|{bzlink.Item.Rare}|{bzlink.Item.Upgrade}|{bzlink.Item.RuneAmount}|0|{info} ";
                }

                return $"rc_blist {packet.Index} {itembazar} ";
            }
            catch (Exception ex)
            {
                Core.Logger.Error(ex);
                return "";
            }
        }

        public static string GenerateRemovePacket(short slot) => $"{slot}.-1.0.0.0";

        public static string GenerateRl(byte type)
        {
            var str = $"rl {type}";

            ServerManager.Instance.GroupList.ForEach(s =>
            {
                if (s.SessionCount > 0)
                {
                    var leader = s.Sessions.ElementAt(0);
                    str +=
                        $" {s.Raid.Id}.{s.Raid?.LevelMinimum}.{s.Raid?.LevelMaximum}.{leader.Character.Name}.{leader.Character.Level}.{(leader.Character.UseSp ? leader.Character.Morph : -1)}.{(byte)leader.Character.Class}.{(byte)leader.Character.Gender}.{s.SessionCount}.{leader.Character.HeroLevel}";
                }
            });

            return str;
        }

        public static string GenerateRp(int mapid, int x, int y, string param) => $"rp {mapid} {x} {y} {param}";

        public static string GenerateSay(string message, int type, long callerId = 0) => $"say 1 {callerId} {type} {message}";

        public static string GenerateShopMemo(int type, string message) => $"s_memo {type} {message}";

        public static string GenerateTeamArenaClose() => "ta_close";

        public static string GenerateTeamArenaMenu(byte mode, byte zenasScore, byte ereniaScore, int time,
                                                   byte arenaType) => $"ta_m {mode} {zenasScore} {ereniaScore} {time} {arenaType}";

        public static IEnumerable<string> GenerateVb()
        {
            return new[] { "vb 340 0 0", "vb 339 0 0", "vb 472 0 0", "vb 471 0 0" };
        }

        public string GenerateFStashRemove(short slot) => $"f_stash {GenerateRemovePacket(slot)}";

        public string GenerateInventoryRemove(InventoryType Type, short Slot) => $"ivn {(byte)Type} {GenerateRemovePacket(Slot)}";

        public string GeneratePStashRemove(short slot) => $"pstash {GenerateRemovePacket(slot)}";

        public string GenerateStashRemove(short slot) => $"stash {GenerateRemovePacket(slot)}";

        public string GenerateTaSt(TalentArenaOptionType watch) => $"ta_st {(byte)watch}";


        #endregion
    }
}