using System;
using System.Linq;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.ScriptedInstance
{
    public class RSelPacketHandler : IPacketHandler
    {
        #region Instantiation

        public RSelPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void GetGift(RSelPacket rSelPacket)
        {
            if (Session.Character.Timespace?.FirstMap?.MapInstanceType == MapInstanceType.TimeSpaceInstance)
            {
                ServerManager.GetBaseMapInstanceIdByMapId(Session.Character.MapId);
                if (Session.Character.Timespace?.FirstMap.InstanceBag.EndState == 5 ||
                    Session.Character.Timespace?.FirstMap.InstanceBag.EndState == 6)
                    if (!Session.Character.TimespaceRewardGotten)
                    {
                        Session.Character.TimespaceRewardGotten = true;
                        Session.Character.GetReputation(Session.Character.Timespace.Reputation);

                        Session.Character.Gold =
                            Session.Character.Gold + Session.Character.Timespace.Gold
                            > ServerManager.Instance.Configuration.MaxGold
                                ? ServerManager.Instance.Configuration.MaxGold
                                : Session.Character.Gold + Session.Character.Timespace.Gold;
                        Session.SendPacket(Session.Character.GenerateGold());

                        if (Session.Character.Timespace.Gold > 0)
                        {
                            Session.SendPacket(Session.Character.GenerateSay(
                                string.Format(Language.Instance.GetMessageFromKey("GOLD_TS_END"),
                                    Session.Character.Timespace.Gold), 10));
                        }

                        if (Session.Character.Timespace.Reputation > 0)
                        {
                            Session.SendPacket(Session.Character.GenerateSay(
                                string.Format(Language.Instance.GetMessageFromKey("REP_TS_END"),
                                    Session.Character.Timespace.Reputation), 10));
                        }

                        if (Session.Character.Timespace.FamExp > 0)
                        {
                            Session.SendPacket(Session.Character.GenerateSay(
                                string.Format(Language.Instance.GetMessageFromKey("FXP_TS_END"),
                                    Session.Character.Timespace.FamExp), 10));
                        }

                        var rand = new Random().Next(Session.Character.Timespace.DrawItems.Count);
                        var repay = "repay ";
                        if (Session.Character.Timespace.DrawItems.Count > 0)
                            Session.Character.GiftAdd(Session.Character.Timespace.DrawItems[rand].VNum,
                                Session.Character.Timespace.DrawItems[rand].Amount,
                                design: Session.Character.Timespace.DrawItems[rand].Design,
                                forceRandom: Session.Character.Timespace.DrawItems[rand].IsRandomRare);

                        for (var i = 0; i < 3; i++)
                        {
                            var gift = Session.Character.Timespace.GiftItems.ElementAtOrDefault(i);
                            repay += gift == null ? "-1.0.0 " : $"{gift.VNum}.0.{gift.Amount} ";
                            if (gift != null)
                                Session.Character.GiftAdd(gift.VNum, gift.Amount, design: gift.Design,
                                    forceRandom: gift.IsRandomRare);
                        }

                        // TODO: Add HasAlreadyDone
                        for (var i = 0; i < 2; i++)
                        {
                            var gift = Session.Character.Timespace.SpecialItems.ElementAtOrDefault(i);
                            repay += gift == null ? "-1.0.0 " : $"{gift.VNum}.0.{gift.Amount} ";
                            if (gift != null)
                                Session.Character.GiftAdd(gift.VNum, gift.Amount, design: gift.Design,
                                    forceRandom: gift.IsRandomRare);
                        }

                        // Add Partner 
                        var partnerVnum = (short) Session.Character.Timespace.PartnerVnumRewards;
                        if (partnerVnum != 0)
                        {
                            var mate = Session.Character.Mates.Find(s =>
                                s.NpcMonsterVNum == partnerVnum && s.MateType == MateType.Partner);

                            if (mate == null)
                            {
                                var mateNpc = ServerManager.GetNpcMonster(partnerVnum);
                                var lvl = Session.Character.Level -= 0;
                                var newMate = new GameObject.Mate(Session.Character, mateNpc, lvl, MateType.Partner);
                                Session.Character.AddPet(newMate);
                            }
                        }

                        if (Session.Character.Timespace.DrawItems.Count > 0)
                            repay +=
                                $"{Session.Character.Timespace.DrawItems[rand].VNum}.0.{Session.Character.Timespace.DrawItems[rand].Amount}";
                        else
                            repay +=
                                "-1.0.0";
                        Session.SendPacket(repay);
                        Session.Character.Timespace.FirstMap.InstanceBag.EndState = 6;


                        Session.Character.Timespace._mapInstanceDictionary.ToList().SelectMany(s => s.Value.Sessions)
                            .Where(s => s.Character?.Timespace != null).ToList().ForEach(s =>
                            {
                                s.Character.GeneralLogs.Add(new GeneralLogDTO
                                {
                                    AccountId = s.Account.AccountId,
                                    CharacterId = s.Character.CharacterId,
                                    IpAddress = s.IpAddress,
                                    LogData = s.Character.Timespace.Id.ToString(),
                                    LogType = "InstanceEntry",
                                    Timestamp = DateTime.Now
                                });
                            });
                    }
            }
        }

        #endregion
    }
}