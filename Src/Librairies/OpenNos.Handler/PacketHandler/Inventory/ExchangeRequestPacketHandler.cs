using System;
using System.Collections.Generic;
using System.Linq;
using NosTale.Extension.Extension.Packet;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Inventory
{
    public class ExchangeRequestPacketHandler : IPacketHandler
    {
        #region Instantiation

        public ExchangeRequestPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void ExchangeRequest(ExchangeRequestPacket exchangeRequestPacket)
        {
            if (!Session.Character.VerifiedLock)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CHARACTER_LOCKED_USE_UNLOCK"), 0));
                return;
            }

            if (exchangeRequestPacket != null)
            {
                ClientSession sess = ServerManager.Instance.GetSessionByCharacterId(exchangeRequestPacket.CharacterId);

                if (sess != null && Session.Character.MapInstanceId != sess.Character.MapInstanceId)
                {
                    sess.Character.ExchangeInfo = null;
                    Session.Character.ExchangeInfo = null;
                }
                else
                {
                    switch (exchangeRequestPacket.RequestType)
                    {
                        case RequestExchangeType.Requested:
                            if (!Session.HasCurrentMapInstance)
                            {
                                return;
                            }

                            ClientSession targetSession =
                                Session.CurrentMapInstance.GetSessionByCharacterId(exchangeRequestPacket.CharacterId);
                            if (targetSession?.Account == null)
                            {
                                return;
                            }

                            if (!targetSession.Character.VerifiedLock)
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CHARACTER_LOCKED_USE_UNLOCK"), 0));
                                return;
                            }

                            if (targetSession.CurrentMapInstance?.MapInstanceType == MapInstanceType.TalentArenaMapInstance)
                            {
                                return;
                            }

                            if (targetSession.Character.Group != null
                                && targetSession.Character.Group?.GroupType != GroupType.Group)
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("EXCHANGE_NOT_ALLOWED_IN_RAID"), 0));
                                return;
                            }

                            if (Session.Character.Group != null
                                && Session.Character.Group?.GroupType != GroupType.Group)
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("EXCHANGE_NOT_ALLOWED_WITH_RAID_MEMBER"), 0));
                                return;
                            }

                            if (Session.Character.IsBlockedByCharacter(exchangeRequestPacket.CharacterId))
                            {
                                Session.SendPacket(
                                    UserInterfaceHelper.GenerateInfo(
                                        Language.Instance.GetMessageFromKey("BLACKLIST_BLOCKED")));
                                return;
                            }

                            if (Session.Character.Speed == 0 || targetSession.Character.Speed == 0)
                            {
                                Session.Character.ExchangeBlocked = true;
                            }

                            if (targetSession.Character.LastSkillUse.AddSeconds(20) > DateTime.Now
                                || targetSession.Character.LastDefence.AddSeconds(20) > DateTime.Now)
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateInfo(
                                    string.Format(Language.Instance.GetMessageFromKey("PLAYER_IN_BATTLE"),
                                        targetSession.Character.Name)));
                                return;
                            }

                            if (Session.Character.LastSkillUse.AddSeconds(20) > DateTime.Now
                                || Session.Character.LastDefence.AddSeconds(20) > DateTime.Now)
                            {
                                Session.SendPacket(
                                    UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("IN_BATTLE")));
                                return;
                            }

                            if (Session.Character.HasShopOpened || targetSession.Character.HasShopOpened)
                            {
                                Session.SendPacket(
                                    UserInterfaceHelper.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("HAS_SHOP_OPENED"), 10));
                                return;
                            }

                            if (targetSession.Character.ExchangeBlocked)
                            {
                                Session.SendPacket(
                                    Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("TRADE_BLOCKED"),
                                        11));
                            }
                            else
                            {
                                if (Session.Character.InExchangeOrTrade || targetSession.Character.InExchangeOrTrade)
                                {
                                    Session.SendPacket(
                                        UserInterfaceHelper.GenerateModal(
                                            Language.Instance.GetMessageFromKey("ALREADY_EXCHANGE"), 0));
                                }
                                else
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateModal(
                                        string.Format(Language.Instance.GetMessageFromKey("YOU_ASK_FOR_EXCHANGE"),
                                            targetSession.Character.Name), 0));

                                    Logger.LogUserEvent("TRADE_REQUEST", Session.GenerateIdentity(),
                                        $"[ExchangeRequest][{targetSession.GenerateIdentity()}]");

                                    Session.Character.TradeRequests.Add(targetSession.Character.CharacterId);
                                    targetSession.SendPacket(UserInterfaceHelper.GenerateDialog(
                                        $"#req_exc^2^{Session.Character.CharacterId} #req_exc^5^{Session.Character.CharacterId} {string.Format(Language.Instance.GetMessageFromKey("INCOMING_EXCHANGE"), Session.Character.Name)}"));
                                }
                            }

                            break;

                        case RequestExchangeType.Confirmed: // click Trade button in exchange window
                            if (Session.HasCurrentMapInstance && Session.HasSelectedCharacter
                                                              && Session.Character.ExchangeInfo != null
                                                              && Session.Character.ExchangeInfo.TargetCharacterId
                                                              != Session.Character.CharacterId)
                            {
                                if (!Session.HasCurrentMapInstance)
                                {
                                    return;
                                }

                                targetSession =
                                    Session.CurrentMapInstance.GetSessionByCharacterId(Session.Character.ExchangeInfo
                                        .TargetCharacterId);

                                if (targetSession == null)
                                {
                                    return;
                                }

                                if (Session.Character.Group != null
                                    && Session.Character.Group?.GroupType != GroupType.Group)
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("EXCHANGE_NOT_ALLOWED_IN_RAID"), 0));
                                    return;
                                }

                                if (targetSession.Character.Group != null
                                    && targetSession.Character.Group?.GroupType != GroupType.Group)
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("EXCHANGE_NOT_ALLOWED_WITH_RAID_MEMBER"),
                                        0));
                                    return;
                                }

                                if (Session.IsDisposing || targetSession.IsDisposing || targetSession != null && targetSession.Character.HasShopOpened)
                                {
                                    CloseExchange(Session, targetSession);
                                    return;
                                }

                                lock (targetSession.Character.Inventory)
                                {
                                    lock (Session.Character.Inventory)
                                    {
                                        ExchangeInfo targetExchange = targetSession.Character.ExchangeInfo;
                                        GameObject.Inventory inventory = targetSession.Character.Inventory;

                                        long gold = targetSession.Character.Gold;
                                        var backpack = targetSession.Character.HaveBackpack() ? 1 : 0;
                                        var newbackpack = targetSession.Character.HaveExtension() ? 1 : 0;
                                        long goldBank = targetSession.Character.GoldBank;
                                        long maxGold = ServerManager.Instance.Configuration.MaxGold;
                                        var maxBankGold = ServerManager.Instance.MaxBankGold;

                                        if (targetExchange == null || Session.Character.ExchangeInfo == null)
                                        {
                                            return;
                                            Logger.LogUserEvent("TRADE_ACCEPT", Session.GenerateIdentity(),
                                                $"[ExchangeAccept][{targetSession.GenerateIdentity()}]");
                                        }

                                        if (Session.Character.ExchangeInfo.Validated && targetExchange.Validated)
                                        {
                                            Session.Character.ExchangeInfo.Confirmed = true;
                                            if (targetExchange != null && targetExchange.Confirmed && Session.Character.ExchangeInfo.Confirmed)
                                            {
                                                Session.SendPacket("exc_close 1");
                                                targetSession.SendPacket("exc_close 1");

                                                var @continue = true;
                                                var goldmax = false;
                                                if (!Session.Character.Inventory.EnoughPlaceV2(targetExchange.ExchangeList, ((Session.Character.HaveBackpack() ? 1 : 0) * 12) + ((Session.Character.HaveExtension() ? 1 : 0) * 60)))
                                                {
                                                    @continue = false;
                                                }

                                                if (!inventory.EnoughPlaceV2(Session.Character.ExchangeInfo.ExchangeList, backpack + newbackpack))
                                                {
                                                    @continue = false;
                                                }

                                                if (Session.Character.ExchangeInfo.Gold + gold > maxGold)
                                                {
                                                    goldmax = true;
                                                }
                                                if (Session.Character.ExchangeInfo.BankGold + goldBank > maxBankGold)
                                                    goldmax = true;
                                                if (Session.Character.ExchangeInfo.BankGold > Session.Character.GoldBank)
                                                {
                                                    return;
                                                }

                                                if (Session.Character.ExchangeInfo.Gold > Session.Character.Gold)
                                                {
                                                    return;
                                                }

                                                if (targetExchange.BankGold + Session.Character.ExchangeInfo.BankGold > maxBankGold)
                                                    goldmax = true;
                                                if (targetExchange.Gold + Session.Character.Gold > maxGold)
                                                {
                                                    goldmax = true;
                                                }

                                                if (!@continue || goldmax)
                                                {
                                                    var message = !@continue ? UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"), 0)
                                                        : UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("MAX_GOLD"), 0);
                                                    Session.SendPacket(message);
                                                    targetSession.SendPacket(message);
                                                    CloseExchange(Session, targetSession);
                                                }
                                                else
                                                {
                                                    if (Session.Character.ExchangeInfo.ExchangeList.Any(ei => !(ei.Item.IsTradable || ei.IsBound)))
                                                    {
                                                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_NOT_TRADABLE"), 0));
                                                        CloseExchange(Session, targetSession);
                                                    }
                                                    else // all items can be traded
                                                    {
                                                        Session.Character.IsExchanging = targetSession.Character.IsExchanging = true;

                                                        // exchange all items from target to source
                                                        Exchange(targetSession, Session);

                                                        // exchange all items from source to target
                                                        Exchange(Session, targetSession);

                                                        Session.Character.IsExchanging = targetSession.Character.IsExchanging = false;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Session.SendPacket(UserInterfaceHelper.GenerateInfo(string.Format(Language.Instance.GetMessageFromKey("IN_WAITING_FOR"), targetSession.Character.Name)));
                                            }
                                        }
                                        {
                                            try
                                            {
                                                if (Session.Character == null || Session.Character.ExchangeInfo == null)
                                                {
                                                    return;
                                                }
                                                Session.Character.ExchangeInfo.Confirmed = true;
                                                if (targetExchange.Confirmed
                                                    && Session.Character.ExchangeInfo.Confirmed)
                                                {
                                                    Session.SendPacket("exc_close 1");
                                                    targetSession.SendPacket("exc_close 1");

                                                    bool continues = true;
                                                    bool goldmax = false;
                                                    if (!Session.Character.Inventory.EnoughPlace(targetExchange
                                                        .ExchangeList))
                                                    {
                                                        continues = false;
                                                    }

                                                    continues &=
                                                        inventory.EnoughPlace(Session.Character.ExchangeInfo
                                                            .ExchangeList);
                                                    goldmax |= Session.Character.ExchangeInfo.Gold + gold > maxGold;
                                                    goldmax |= Session.Character.ExchangeInfo.BankGold + goldBank > maxGold;
                                                    if (Session.Character.ExchangeInfo.Gold > Session.Character.Gold
                                                        || Session.Character.ExchangeInfo.BankGold > Session.Character.GoldBank)
                                                    {
                                                        return;
                                                    }

                                                    goldmax |= targetExchange.Gold + Session.Character.Gold > maxGold;
                                                    goldmax |= targetExchange.BankGold + Session.Character.GoldBank > maxGold;
                                                    if (!continues || goldmax)
                                                    {
                                                        string message = !continues
                                                            ? UserInterfaceHelper.GenerateMsg(
                                                                Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"),
                                                                0)
                                                            : UserInterfaceHelper.GenerateMsg(
                                                                Language.Instance.GetMessageFromKey("MAX_GOLD"), 0);
                                                        Session.SendPacket(message);
                                                        targetSession.SendPacket(message);
                                                        CloseExchange(Session, targetSession);
                                                    }
                                                    else if (Session.Character.Gold < Session.Character.ExchangeInfo.Gold || targetSession.Character.Gold < targetExchange.Gold
                                                        || Session.Character.GoldBank < Session.Character.ExchangeInfo.BankGold || targetSession.Character.GoldBank < targetExchange.BankGold)
                                                    {
                                                        string message = UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ERROR_ON_EXANGE"), 0);
                                                        Session.SendPacket(message);
                                                        targetSession.SendPacket(message);
                                                        CloseExchange(Session, targetSession);
                                                    }
                                                    else
                                                    {
                                                        if (Session.Character.ExchangeInfo.ExchangeList.Any(ei =>
                                                            !(ei.Item.IsTradable || ei.IsBound)))
                                                        {
                                                            Session.SendPacket(
                                                                UserInterfaceHelper.GenerateMsg(
                                                                    Language.Instance.GetMessageFromKey(
                                                                        "ITEM_NOT_TRADABLE"), 0));
                                                            CloseExchange(Session, targetSession);
                                                        }
                                                        if (targetSession.Character.ExchangeInfo.ExchangeList.Any(ei =>
                                                            !(ei.Item.IsTradable || ei.IsBound)))
                                                        {
                                                            targetSession.SendPacket(
                                                                UserInterfaceHelper.GenerateMsg(
                                                                    Language.Instance.GetMessageFromKey(
                                                                        "ITEM_NOT_TRADABLE"), 0));
                                                            CloseExchange(targetSession, Session);
                                                        }
                                                        else // all items can be traded
                                                        {
                                                            Session.Character.IsExchanging =
                                                                targetSession.Character.IsExchanging = true;

                                                            // exchange all items from target to source
                                                            Exchange(targetSession, Session);

                                                            // exchange all items from source to target
                                                            Exchange(Session, targetSession);

                                                            Session.Character.IsExchanging =
                                                                targetSession.Character.IsExchanging = false;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    Session.SendPacket(UserInterfaceHelper.GenerateInfo(
                                                        string.Format(
                                                            Language.Instance.GetMessageFromKey("IN_WAITING_FOR"),
                                                            targetSession.Character.Name)));
                                                }
                                            }
                                            catch (NullReferenceException nre)
                                            {
                                                Logger.Error(nre);
                                            }
                                        }
                                    }
                                }
                            }

                            break;

                        case RequestExchangeType.Cancelled: // cancel trade thru exchange window
                            if (Session.HasCurrentMapInstance && Session.Character.ExchangeInfo != null)
                            {
                                targetSession =
                                    Session.CurrentMapInstance.GetSessionByCharacterId(Session.Character.ExchangeInfo
                                        .TargetCharacterId);
                                CloseExchange(Session, targetSession);
                            }

                            break;

                        case RequestExchangeType.List:
                            if (sess != null && (!Session.Character.InExchangeOrTrade || !sess.Character.InExchangeOrTrade))
                            {
                                ClientSession otherSession =
                                    ServerManager.Instance.GetSessionByCharacterId(exchangeRequestPacket.CharacterId);
                                if (exchangeRequestPacket.CharacterId == Session.Character.CharacterId
                                    || Session.Character.Speed == 0 || otherSession == null
                                    || otherSession.Character.TradeRequests.All(s => s != Session.Character.CharacterId))
                                {
                                    return;
                                }

                                if (Session.Character.Group != null
                                    && Session.Character.Group?.GroupType != GroupType.Group)
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("EXCHANGE_NOT_ALLOWED_IN_RAID"), 0));
                                    return;
                                }

                                if (otherSession.Character.Group != null
                                    && otherSession.Character.Group?.GroupType != GroupType.Group)
                                {
                                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("EXCHANGE_NOT_ALLOWED_WITH_RAID_MEMBER"),
                                        0));
                                    return;
                                }

                                Session.SendPacket($"exc_list 1 {exchangeRequestPacket.CharacterId} -1");
                                Session.SendPacket($"gbex {Session.Character.GoldBank / 1000} {Session.Character.Gold} 0 0");
                                Session.Character.ExchangeInfo = new ExchangeInfo
                                {
                                    TargetCharacterId = exchangeRequestPacket.CharacterId,
                                    Confirmed = false
                                };
                                sess.Character.ExchangeInfo = new ExchangeInfo
                                {
                                    TargetCharacterId = Session.Character.CharacterId,
                                    Confirmed = false
                                };
                                Session.CurrentMapInstance?.Broadcast(Session,
                                    $"exc_list 1 {Session.Character.CharacterId} -1", ReceiverType.OnlySomeone,
                                    "", exchangeRequestPacket.CharacterId);
                                ClientSession test = ServerManager.Instance.GetSessionByCharacterId(exchangeRequestPacket.CharacterId);
                                test.SendPacket($"gbex {test.Character.GoldBank / 1000} {test.Character.Gold} 0 0");
                            }
                            else
                            {
                                Session.CurrentMapInstance?.Broadcast(Session,
                                    UserInterfaceHelper.GenerateModal(
                                        Language.Instance.GetMessageFromKey("ALREADY_EXCHANGE"), 0),
                                    ReceiverType.OnlySomeone, "", exchangeRequestPacket.CharacterId);
                            }

                            break;

                        case RequestExchangeType.Declined:
                            if (sess != null)
                            {
                                sess.Character.ExchangeInfo = null;
                            }
                            Session.Character.ExchangeInfo = null;
                            Session.SendPacket(
                                Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("YOU_REFUSED"), 10));
                            if (sess != null)
                            {
                                sess.SendPacket(
                                    Session.Character.GenerateSay(
                                        string.Format(Language.Instance.GetMessageFromKey("EXCHANGE_REFUSED"),
                                            Session.Character.Name), 10));

                            }

                            break;

                        default:
                            Logger.Warn(
                                $"Exchange-Request-Type not implemented. RequestType: {exchangeRequestPacket.RequestType})");
                            break;
                    }
                }
            }
        }
        private static void CloseExchange(ClientSession session, ClientSession targetSession)
        {
            if (targetSession?.Character.ExchangeInfo != null)
            {
                targetSession.SendPacket("exc_close 0");
                targetSession.Character.ExchangeInfo = null;
            }

            if (session?.Character.ExchangeInfo != null)
            {
                session.SendPacket("exc_close 0");
                session.Character.ExchangeInfo = null;
            }
        }

        private static void Exchange(ClientSession sourceSession, ClientSession targetSession)
        {
            if (sourceSession?.Character.ExchangeInfo == null)
            {
                return;
            }

            string data = "";

            // remove all items from source session
            foreach (ItemInstance item in sourceSession.Character.ExchangeInfo.ExchangeList)
            {
                ItemInstance invtemp = sourceSession.Character.Inventory.GetItemInstanceById(item.Id);
                if (invtemp?.Amount >= item.Amount)
                {
                    sourceSession.Character.Inventory.RemoveItemFromInventory(invtemp.Id, item.Amount);
                }
                else
                {
                    return;
                }
            }

            // add all items to target session
            foreach (ItemInstance item in sourceSession.Character.ExchangeInfo.ExchangeList)
            {
                ItemInstance item2 = item.DeepCopy();
                item2.Id = Guid.NewGuid();
                data += $"[OldIIId: {item.Id} NewIIId: {item2.Id} ItemVNum: {item.ItemVNum} Amount: {item.Amount} Rare: {item.Rare} Upgrade: {item.Upgrade}]";
                List<ItemInstance> inv = targetSession.Character.Inventory.AddToInventory(item2);
                if (inv.Count == 0)
                {
                    // do what?
                }
            }

            data += $"[Gold: {sourceSession.Character.ExchangeInfo.Gold}]";
            data += $"[BankGold: {sourceSession.Character.ExchangeInfo.BankGold}]";

            // handle gold
            sourceSession.Character.Gold -= sourceSession.Character.ExchangeInfo.Gold;
            sourceSession.Character.GoldBank -= sourceSession.Character.ExchangeInfo.BankGold;
            sourceSession.SendPacket(sourceSession.Character.GenerateGold());
            sourceSession.Character.GoldBank -= (sourceSession.Character.ExchangeInfo.BankGold * 1000);
            targetSession.Character.Gold += sourceSession.Character.ExchangeInfo.Gold;
            targetSession.Character.GoldBank += sourceSession.Character.ExchangeInfo.BankGold;
            targetSession.SendPacket(targetSession.Character.GenerateGold());
            targetSession.Character.GoldBank += (sourceSession.Character.ExchangeInfo.BankGold * 1000);


            // all items and gold from sourceSession have been transferred, clean exchange info

            Logger.LogUserEvent("TRADE_COMPLETE", sourceSession.GenerateIdentity(),
                $"[{targetSession.GenerateIdentity()}]Data: {data}");

            sourceSession.Character.ExchangeInfo = null;
        }

        #endregion
    }
}