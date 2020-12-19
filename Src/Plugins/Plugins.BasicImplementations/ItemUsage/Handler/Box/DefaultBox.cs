﻿using System;
using System.Linq;
using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._ItemUsage;
using OpenNos.GameObject._ItemUsage.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.GameObject.Extension;


namespace Plugins.BasicImplementations.ItemUsage.Handler.Box
{
   public class DefaultBox : IUseItemRequestHandlerAsync
    {
        public ItemPluginType Type => ItemPluginType.Box;
        public long EffectId => default;

        public async Task HandleAsync(ClientSession session, InventoryUseItemEvent e)
        {
 
            if (session.Character.IsVehicled && e.Item.Item.Effect != 888)
            {
                session.SendPacket(
                    session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_DO_VEHICLED"), 10));
                return;
            }

            if (e.Item.ItemVNum == 333 || e.Item.ItemVNum == 334) // Sealed Jajamaru Specialist Card & Sealed Princess Sakura Bead
            {
                return;
            }

            switch (e.Item.Item.Effect)
            {
                case 0:
                    switch (e.Item.Item.VNum)
                    {
                        case 4801:
                            var box = session.Character.Inventory.LoadBySlotAndType(e.Item.Slot, InventoryType.Equipment);
                            if (box != null)
                            {
                                if (box.HoldingVNum == 0)
                                {
                                    session.SendPacket($"wopen 44 {e.Item.Slot} 1");
                                }
                                else
                                {
                                    var newInv = session.Character.Inventory.AddNewToInventory(box.HoldingVNum);
                                    if (newInv.Count > 0)
                                    {
                                        newInv[0].EquipmentSerialId = box.EquipmentSerialId;
                                        var itemInstance = newInv[0];
                                        var specialist =
                                            session.Character.Inventory.LoadBySlotAndType(itemInstance.Slot,
                                                itemInstance.Type);
                                        var Slot = e.Item.Slot;
                                        if (Slot != -1)
                                        {
                                            if (specialist != null)
                                            {
                                                session.SendPacket(session.Character.GenerateSay(
                                                    $"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {specialist.Item.Name}",
                                                    12));
                                                newInv.ForEach(s =>
                                                    session.SendPacket(specialist.GenerateInventoryAdd()));
                                            }

                                            session.Character.Inventory.RemoveItemFromInventory(box.Id);
                                        }
                                    }
                                    else
                                    {
                                        session.SendPacket(
                                            UserInterfaceHelper.GenerateMsg(
                                                Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"), 0));
                                    }
                                }
                            }

                            return;
                    }

                    if (e.Option == 0)
                    {
                        if (e.PacketSplit?.Length == 9)
                        {
                            var box = session.Character.Inventory.LoadBySlotAndType(e.Item.Slot, InventoryType.Equipment);
                            if (box != null)
                            {
                                if (box.Item.ItemSubType == 3)
                                {
                                    session.SendPacket(
                                            $"qna #guri^300^8023^{e.Item.Slot} {Language.Instance.GetMessageFromKey("ASK_OPEN_BOX")}");
                                }
                                else if (box.HoldingVNum == 0)
                                {
                                    session.SendPacket(
                                            $"qna #guri^300^8023^{e.Item.Slot}^{e.PacketSplit[3]} {Language.Instance.GetMessageFromKey("ASK_STORE_PET")}");
                                }
                                else
                                {
                                    session.SendPacket(
                                            $"qna #guri^300^8023^{e.Item.Slot} {Language.Instance.GetMessageFromKey("ASK_RELEASE_PET")}");
                                }
                            }
                        }
                    }
                    else
                    {
                        //u_i 2 2000000 0 21 0 0
                        var box = session.Character.Inventory.LoadBySlotAndType(e.Item.Slot, InventoryType.Equipment);
                        if (box != null)
                        {
                            if (box.Item.ItemSubType == 3)
                            {
                                var roll = box.Item.RollGeneratedItems.Where(s => s.MinimumOriginalItemRare <= box.Rare
                                                                                  && s.MaximumOriginalItemRare >=
                                                                                  box.Rare
                                                                                  && s.OriginalItemDesign == box.Design)
                                    .ToList();
                                var probabilities = roll.Sum(s => s.Probability);
                                var rnd = ServerManager.RandomNumber(0, probabilities);
                                var currentrnd = 0;
                                foreach (var rollitem in roll.OrderBy(s => ServerManager.RandomNumber()))
                                {
                                    currentrnd += rollitem.Probability;
                                    if (currentrnd >= rnd)
                                    {
                                        var i = ServerManager.GetItem(rollitem.ItemGeneratedVNum);
                                        sbyte rare = 0;
                                        byte upgrade = 0;
                                        if (i.ItemType == ItemType.Armor || i.ItemType == ItemType.Weapon ||
                                            i.ItemType == ItemType.Shell || i.ItemType == ItemType.Box)
                                        {
                                            rare = box.Rare;
                                        }

                                        if (i.ItemType == ItemType.Shell)
                                        {
                                            if (rare < 1)
                                            {
                                                rare = 1;
                                            }
                                            else if (rare > 7)
                                            {
                                                rare = 7;
                                            }

                                            upgrade = (byte) ServerManager.RandomNumber(50, 81);
                                        }

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
                                            rollitem.ItemGeneratedAmount, (byte) rare, upgrade,
                                            rollitem.ItemGeneratedDesign);
                                        session.SendPacket(
                                            $"rdi {rollitem.ItemGeneratedVNum} {rollitem.ItemGeneratedAmount}");
                                        session.Character.Inventory.RemoveItemFromInventory(box.Id);
                                        return;

                                        //newInv = session.Character.Inventory.AddNewToInventory(rollitem.ItemGeneratedVNum, amount: rollitem.ItemGeneratedAmount, Design: design, Rare: rare);
                                        //if (newInv.Count > 0)
                                        //{
                                        //    short Slot = inv.Slot;
                                        //    if (Slot != -1)
                                        //    {
                                        //        session.SendPacket(session.Character.GenerateSay(
                                        //            $"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {newInv.FirstOrDefault(s => s != null)?.Item?.Name} x {rollitem.ItemGeneratedAmount}",
                                        //            12));
                                        //        newInv.Where(s => s != null).ToList()
                                        //            .ForEach(s => session.SendPacket(s.GenerateInventoryAdd()));
                                        //        session.Character.Inventory
                                        //            .RemoveItemAmountFromInventory(box.Id);
                                        //    }
                                        //}
                                    }
                                }
                            }
                            else if (box.HoldingVNum == 0)
                            {
                                if (e.PacketSplit.Length == 1 && int.TryParse(e.PacketSplit[0], out var PetId) &&
                                    session.Character.Mates.Find(s => s.MateTransportId == PetId) is Mate mate)
                                {
                                    if (e.Item.Item.ItemSubType == 0 && mate.MateType != MateType.Pet ||
                                        e.Item.Item.ItemSubType == 1 && mate.MateType != MateType.Partner)
                                    {
                                        return;
                                    }

                                    if (mate.MateType == MateType.Partner && mate.GetInventory().Count > 0)
                                    {
                                        session.SendPacket(
                                            UserInterfaceHelper.GenerateMsg(
                                                Language.Instance.GetMessageFromKey("EQ_NOT_EMPTY"), 0));
                                        return;
                                    }

                                    box.HoldingVNum = mate.NpcMonsterVNum;
                                    box.SpLevel = mate.Level;
                                    box.SpDamage = mate.Attack;
                                    box.SpDefence = mate.Defence;
                                    session.Character.Mates.Remove(mate);
                                    if (mate.MateType == MateType.Partner)
                                    {
                                        byte i = 0;
                                        session.Character.Mates.Where(s => s.MateType == MateType.Partner).ToList()
                                            .ForEach(s =>
                                            {
                                                s.GetInventory().ForEach(item => item.Type = (InventoryType) (13 + i));
                                                s.PetId = i;
                                                i++;
                                            });
                                    }

                                    session.SendPacket(
                                        UserInterfaceHelper.GenerateInfo(
                                            Language.Instance.GetMessageFromKey("PET_STORED")));
                                    session.SendPacket(UserInterfaceHelper.GeneratePClear());
                                    session.SendPackets(session.Character.GenerateScP());
                                    session.SendPackets(session.Character.GenerateScN());
                                    session.CurrentMapInstance?.Broadcast(mate.GenerateOut());
                                }
                            }
                            else
                            {
                                var heldMonster = ServerManager.GetNpcMonster(box.HoldingVNum);
                                if (heldMonster != null)
                                {
                                    var mate = new Mate(session.Character, heldMonster, box.SpLevel,
                                        e.Item.Item.ItemSubType == 0 ? MateType.Pet : MateType.Partner)
                                    {
                                        Attack = box.SpDamage,
                                        Defence = box.SpDefence
                                    };
                                    if (session.Character.AddPet(mate))
                                    {
                                        session.Character.Inventory.RemoveItemFromInventory(e.Item.Id);
                                        session.SendPacket(
                                            UserInterfaceHelper.GenerateInfo(
                                                Language.Instance.GetMessageFromKey("PET_LEAVE_BEAD")));
                                    }
                                }
                            }
                        }
                    }

                    break;

                case 1:
                    if (e.Option == 0)
                    {
                        session.SendPacket(
                            $"qna #guri^300^8023^{e.Item.Slot} {Language.Instance.GetMessageFromKey("ASK_RELEASE_PET")}");
                    }
                    else
                    {
                        var heldMonster = ServerManager.GetNpcMonster((short) e.Item.Item.EffectValue);
                        if (session.CurrentMapInstance == session.Character.Miniland && heldMonster != null)
                        {
                            var mate = new Mate(session.Character, heldMonster, e.Item.Item.LevelMinimum,
                                e.Item.Item.ItemSubType == 1 ? MateType.Partner : MateType.Pet);
                            if (session.Character.AddPet(mate))
                            {
                                session.Character.Inventory.RemoveItemFromInventory(e.Item.Id);
                                session.SendPacket(
                                    UserInterfaceHelper.GenerateInfo(
                                        Language.Instance.GetMessageFromKey("PET_LEAVE_BEAD")));
                            }
                        }
                        else
                        {
                            session.SendPacket(
                                session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_IN_MINILAND"),
                                    12));
                        }
                    }

                    break;

                case 69:
                    if (e.Item.Item.EffectValue == 1 || e.Item.Item.EffectValue == 2)
                    {
                        var box = session.Character.Inventory.LoadBySlotAndType(e.Item.Slot, InventoryType.Equipment);
                        if (box != null)
                        {
                            if (box.HoldingVNum == 0)
                            {
                                session.SendPacket($"wopen 44 {e.Item.Slot}");
                            }
                            else
                            {
                                var newInv = session.Character.Inventory.AddNewToInventory(box.HoldingVNum);
                                if (newInv.Count > 0)
                                {
                                    var itemInstance = newInv[0];
                                    var specialist =
                                        session.Character.Inventory.LoadBySlotAndType(itemInstance.Slot,
                                            itemInstance.Type);
                                    if (specialist != null)
                                    {
                                        specialist.SlDamage = box.SlDamage;
                                        specialist.SlDefence = box.SlDefence;
                                        specialist.SlElement = box.SlElement;
                                        specialist.SlHP = box.SlHP;
                                        specialist.SpDamage = box.SpDamage;
                                        specialist.SpDark = box.SpDark;
                                        specialist.SpDefence = box.SpDefence;
                                        specialist.SpElement = box.SpElement;
                                        specialist.SpFire = box.SpFire;
                                        specialist.SpHP = box.SpHP;
                                        specialist.SpLevel = box.SpLevel;
                                        specialist.SpLight = box.SpLight;
                                        specialist.SpStoneUpgrade = box.SpStoneUpgrade;
                                        specialist.SpWater = box.SpWater;
                                        specialist.Upgrade = box.Upgrade;
                                        specialist.EquipmentSerialId = box.EquipmentSerialId;
                                        specialist.XP = box.XP;
                                    }

                                    var Slot = e.Item.Slot;
                                    if (Slot != -1)
                                    {
                                        if (specialist != null)
                                        {
                                            session.SendPacket(session.Character.GenerateSay(
                                                $"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {specialist.Item.Name} + {specialist.Upgrade}",
                                                12));
                                            newInv.ForEach(s => session.SendPacket(specialist.GenerateInventoryAdd()));
                                        }

                                        session.Character.Inventory.RemoveItemFromInventory(box.Id);
                                    }
                                }
                                else
                                {
                                    session.SendPacket(
                                        UserInterfaceHelper.GenerateMsg(
                                            Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"), 0));
                                }
                            }
                        }
                    }

                    if (e.Item.Item.EffectValue == 3)
                    {
                        var box = session.Character.Inventory.LoadBySlotAndType(e.Item.Slot, InventoryType.Equipment);
                        if (box != null)
                        {
                            if (box.HoldingVNum == 0)
                            {
                                session.SendPacket($"guri 26 0 {e.Item.Slot}");
                            }
                            else
                            {
                                var newInv = session.Character.Inventory.AddNewToInventory(box.HoldingVNum);
                                if (newInv.Count > 0)
                                {
                                    var itemInstance = newInv[0];
                                    var fairy = session.Character.Inventory.LoadBySlotAndType(itemInstance.Slot,
                                        itemInstance.Type);
                                    if (fairy != null)
                                    {
                                        fairy.ElementRate = box.ElementRate;
                                    }

                                    var Slot = e.Item.Slot;
                                    if (Slot != -1)
                                    {
                                        if (fairy != null)
                                        {
                                            session.SendPacket(session.Character.GenerateSay(
                                                $"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {fairy.Item.Name} ({fairy.ElementRate}%)",
                                                12));
                                            newInv.ForEach(s => session.SendPacket(fairy.GenerateInventoryAdd()));
                                        }

                                        session.Character.Inventory.RemoveItemFromInventory(box.Id);
                                    }
                                }
                                else
                                {
                                    session.SendPacket(
                                        UserInterfaceHelper.GenerateMsg(
                                            Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"), 0));
                                }
                            }
                        }
                    }

                    if (e.Item.Item.EffectValue == 4)
                    {
                        var box = session.Character.Inventory.LoadBySlotAndType(e.Item.Slot, InventoryType.Equipment);
                        if (box != null)
                        {
                            if (box.HoldingVNum == 0)
                            {
                                session.SendPacket($"guri 24 0 {e.Item.Slot}");
                            }
                            else
                            {
                                var newInv = session.Character.Inventory.AddNewToInventory(box.HoldingVNum);
                                if (newInv.Count > 0)
                                {
                                    var Slot = e.Item.Slot;
                                    if (Slot != -1)
                                    {
                                        session.SendPacket(session.Character.GenerateSay(
                                            $"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {newInv[0].Item.Name} x 1)",
                                            12));
                                        newInv.ForEach(s => session.SendPacket(s.GenerateInventoryAdd()));
                                        session.Character.Inventory.RemoveItemFromInventory(box.Id);
                                    }
                                }
                                else
                                {
                                    session.SendPacket(
                                        UserInterfaceHelper.GenerateMsg(
                                            Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"), 0));
                                }
                            }
                        }
                    }

                    break;

                case 888:
                    if (session.Character.IsVehicled)
                    {
                        if (!session.Character.Buff.Any(s => s.Card.CardId == 336))
                        {
                            if (e.Item.ItemDeleteTime == null)
                            {
                                e.Item.ItemDeleteTime = DateTime.Now.AddHours(e.Item.Item.LevelMinimum);
                            }
                            session.Character.VehicleItem.BCards.ForEach(s =>
                                    s.ApplyBCards(session.Character.BattleEntity, session.Character.BattleEntity));
                            session.CurrentMapInstance.Broadcast($"eff 1 {session.Character.CharacterId} 885");
                        }
                    }

                    break;

                default:
                    Logger.Warn(string.Format(Language.Instance.GetMessageFromKey("NO_HANDLER_ITEM"), GetType(), e.Item.Item.VNum,
                        e.Item.Item.Effect, e.Item.Item.EffectValue));
                    break;
            }
        }
    }
}