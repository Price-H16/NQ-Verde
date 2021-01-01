using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System;
using System.Linq;

namespace NosTale.Extension.Extension.Packet
{
    public static class InvExt
    {
        #region Methods

        public static void ChangeSp(this ClientSession Session)
        {
            var sp = Session.Character.Inventory.LoadBySlotAndType((byte)EquipmentType.Sp, InventoryType.Wear);
            var fairy = Session.Character.Inventory.LoadBySlotAndType((byte)EquipmentType.Fairy, InventoryType.Wear);

            if (sp != null)
            {
                if (Session.Character.GetReputationIco() < sp.Item.ReputationMinimum)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("LOW_REP"), 0));
                    return;
                }

                if (fairy != null && sp.Item.Element != 0 && fairy.Item.Element != sp.Item.Element && fairy.Item.Element != sp.Item.SecondaryElement)

                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("BAD_FAIRY"), 0));
                    return;
                }

                if (new[] { 4494, 4495, 4496 }.Contains(sp.ItemVNum))
                {
                    if (Session.Character.Timespace == null)
                    {
                        return;
                    }

                    if (ServerManager.Instance.TimeSpaces.Any(s => s.SpNeeded?[(byte)Session.Character.Class] == sp.ItemVNum))
                    {
                        if (Session.Character.Timespace.SpNeeded?[(byte)Session.Character.Class] != sp.ItemVNum)
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                Session.Character.DisableBuffs(BuffType.All);
                Session.Character.ChargeValue = 0;
                Session.Character.EquipmentBCards.AddRange(sp.Item.BCards);
                Session.Character.LastTransform = DateTime.Now;
                Session.Character.UseSp = true;
                Session.Character.Morph = sp.Item.Morph;
                Session.Character.MorphUpgrade = sp.Upgrade;
                Session.Character.MorphUpgrade2 = sp.Design;
                Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateCMode());
                Session.SendPacket(Session.Character.GenerateLev());
                Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 196), Session.Character.PositionX, Session.Character.PositionY);
                Session.CurrentMapInstance?.Broadcast(UserInterfaceHelper.GenerateGuri(6, 1, Session.Character.CharacterId), Session.Character.PositionX, Session.Character.PositionY);
                Session.SendPacket(Session.Character.GenerateSpPoint());
                Session.Character.LoadSpeed();
                Session.SendPacket(Session.Character.GenerateCond());
                Session.SendPacket(Session.Character.GenerateStat());
                Session.SendPackets(Session.Character.GenerateStatChar());
                Session.Character.SkillsSp = new ThreadSafeSortedList<int, CharacterSkill>();
                foreach (var skill in ServerManager.GetAllSkill())
                    if (skill.UpgradeType == sp.Item.Morph && skill.SkillType == (byte)SkillType.CharacterSKill && sp.SpLevel >= skill.LevelMinimum)
                        Session.Character.SkillsSp[skill.SkillVNum] = new CharacterSkill
                        {
                            SkillVNum = skill.SkillVNum,
                            CharacterId = Session.Character.CharacterId
                        };
                Session.SendPacket(Session.Character.GenerateSki());
                Session.SendPackets(Session.Character.GenerateQuicklist());
                CharacterHelper.AddSpecialistWingsBuff(Session);
            }
        }

        public static void CloseExchange(this ClientSession session, ClientSession targetSession)
        {
            if (targetSession?.Character.ExchangeInfo != null)
            {
                targetSession.SendPacket("exc_close 0");
                targetSession.Character.ExchangeInfo = null;
                targetSession.Character.TradeRequests.Clear();
            }

            if (session?.Character.ExchangeInfo != null)
            {
                session.SendPacket("exc_close 0");
                session.Character.ExchangeInfo = null;
                session.Character.TradeRequests.Clear();
            }
        }

        public static void Exchange(this ClientSession sourceSession, ClientSession targetSession)
        {
            if (sourceSession?.Character.ExchangeInfo == null) return;

            var data = "";

            // remove all items from source session
            foreach (var item in sourceSession.Character.ExchangeInfo.ExchangeList)
            {
                var invtemp = sourceSession.Character.Inventory.GetItemInstanceById(item.Id);
                if (invtemp?.Amount >= item.Amount)
                    sourceSession.Character.Inventory.RemoveItemFromInventory(invtemp.Id, item.Amount);
                else
                    return;
            }

            // add all items to target session
            foreach (var item in sourceSession.Character.ExchangeInfo.ExchangeList)
            {
                var item2 = item.DeepCopy();
                item2.Id = Guid.NewGuid();
                data += $"[OldIIId: {item.Id} NewIIId: {item2.Id} ItemVNum: {item.ItemVNum} Amount: {item.Amount} Rare: {item.Rare} Upgrade: {item.Upgrade}]";
                var inv = targetSession.Character.Inventory.AddToInventory(item2);
                if (inv.Count == 0)
                {
                    // do what?
                }
            }

            data += $"[Gold: {sourceSession.Character.ExchangeInfo.Gold}]";
            data += $"[BankGold: {sourceSession.Character.ExchangeInfo.BankGold}]";

            // handle gold
            sourceSession.Character.Gold -= sourceSession.Character.ExchangeInfo.Gold;
            sourceSession.Account.BankMoney -= sourceSession.Character.ExchangeInfo.BankGold;
            sourceSession.SendPacket(sourceSession.Character.GenerateGold());
            targetSession.Character.Gold += sourceSession.Character.ExchangeInfo.Gold;
            targetSession.Account.BankMoney += sourceSession.Character.ExchangeInfo.BankGold;
            targetSession.SendPacket(targetSession.Character.GenerateGold());

            // all items and gold from sourceSession have been transferred, clean exchange info

            Logger.LogUserEvent("TRADE_COMPLETE", sourceSession.GenerateIdentity(), $"[{targetSession.GenerateIdentity()}]Data: {data}");

            sourceSession.Character.ExchangeInfo = null;
        }

        #endregion
    }
}