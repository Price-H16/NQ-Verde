using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;

namespace OpenNos.Handler.PacketHandler.Bazaar
{
    public class CBuyPacketHandler : IPacketHandler
    {
        #region Instantiation

        public CBuyPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void BuyBazaar(CBuyPacket cBuyPacket) //Multiple dupes
        {
            if (Session == null || Session.Character == null)
            {
                return;
            }

            if (Session.Character == null || Session.Character.InExchangeOrTrade)
            {
                return;
            }
            if (ServerManager.Instance.InShutdown)
            {
                return;
            }
            Session.Character.BazarRequests++;
            if (!Session.Character.VerifiedLock)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg("You cant do this because your account is blocked. Use $Unlock", 0));
                return;
            }
            if (Session.Character.InExchangeOrTrade)
            {
                return;
            }

            if (Session.Character.InExchangeOrTrade || Session.Character.HasShopOpened)
            {
                return;
            }

            if (Session.Character.IsMuted())
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg("Tu es sanctonné tu ne peux pas faire ça", 0));
                return;
            }
            if (Session.Character.BazarRequests > 20)
            {
                PenaltyLogDTO log = new PenaltyLogDTO
                {
                    AccountId = Session.Account.AccountId,
                    Reason = "Auto ban c_buy PL",
                    Penalty = PenaltyType.Banned,
                    DateStart = DateTime.Now,
                    DateEnd = DateTime.Now.AddYears(2),
                };
                Character.InsertOrUpdatePenalty(log);
                Session?.Disconnect();
                return;
            }

            Observable.Timer(TimeSpan.FromSeconds(10)).Subscribe(x => 
            {
                if (Session?.Character?.BazarRequests > 0)
                    Session.Character.BazarRequests = 0;
            });
            if (cBuyPacket.Price * cBuyPacket.Amount < 501)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("MINIMUM_BUY_PRICE_IS_501")));
                return;
            }

            SpinWait.SpinUntil(() => !ServerManager.Instance.InBazaarRefreshMode);
            BazaarItemDTO bz = DAOFactory.BazaarItemDAO.LoadById(cBuyPacket.BazaarId);
            if (bz != null && cBuyPacket.Amount > 0)
            {
                long price = cBuyPacket.Amount * bz.Price;

                if (Session.Character.Gold >= price)
                {
                    BazaarItemLink bzcree = new BazaarItemLink { BazaarItem = bz };
                    if (DAOFactory.CharacterDAO.LoadById(bz.SellerId) != null)
                    {
                        bzcree.Owner = DAOFactory.CharacterDAO.LoadById(bz.SellerId)?.Name;
                        bzcree.Item = new ItemInstance(DAOFactory.ItemInstanceDAO.LoadById(bz.ItemInstanceId));
                    }
                    else
                    {
                        return;
                    }

                    if (cBuyPacket.Price * cBuyPacket.Amount < 501)
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("MINIMUM_BUY_PRICE_IS_501")));
                        return;
                    }


                    if (Session.Character.LastBazaarInsert.AddSeconds(5) > DateTime.Now)
                    {
                        return;
                    }



                    if (cBuyPacket.Amount <= bzcree.Item.Amount)
                    {
                        if (!Session.Character.Inventory.CanAddItem(bzcree.Item.ItemVNum))
                        {
                            Session.SendPacket(
                                UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"),
                                    0));
                            return;
                        }

                        if (bzcree.Item != null)
                        {
                            if (bz.IsPackage && cBuyPacket.Amount != bz.Amount)
                            {
                                return;
                            }

                            ItemInstanceDTO bzitemdto =
                                DAOFactory.ItemInstanceDAO.LoadById(bzcree.BazaarItem.ItemInstanceId);
                            if (bzitemdto.Amount < cBuyPacket.Amount)
                            {
                                return;
                            }

                            // Edit this soo we dont generate new guid every single time we take something out.
                            ItemInstance newBz = bzcree.Item.DeepCopy();
                            newBz.Id = Guid.NewGuid();
                            newBz.Amount = cBuyPacket.Amount;
                            newBz.Type = newBz.Item.Type;
                            List<ItemInstance> newInv = Session.Character.Inventory.AddToInventory(newBz);

                            if (newInv.Count > 0)
                            {
                                bzitemdto.Amount -= cBuyPacket.Amount;
                                Session.Character.Gold -= price;
                                Session.SendPacket(Session.Character.GenerateGold());
                                DAOFactory.ItemInstanceDAO.InsertOrUpdate(bzitemdto);
                                ServerManager.Instance.BazaarRefresh(bzcree.BazaarItem.BazaarItemId);
                                Session.SendPacket(
                                    $"rc_buy 1 {bzcree.Item.Item.VNum} {bzcree.Owner} {cBuyPacket.Amount} {cBuyPacket.Price} 0 0 0");

                                Session.SendPacket(Session.Character.GenerateSay(
                                    $"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {bzcree.Item.Item.Name} x {cBuyPacket.Amount}",
                                    10));

                                CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                                {
                                    DestinationCharacterId = bz.SellerId,
                                    SourceWorldId = ServerManager.Instance.WorldId,
                                    Message = StaticPacketHelper.Say(1, bz.SellerId, 12, string.Format(Language.Instance.GetMessageFromKey("BAZAAR_ITEM_SOLD"), cBuyPacket.Amount, bzcree.Item.Item.Name)),
                                    Type = MessageType.Other
                                });

                                Logger.LogUserEvent("BAZAAR_BUY", Session.GenerateIdentity(),
                                    $"BazaarId: {cBuyPacket.BazaarId} VNum: {cBuyPacket.VNum} Amount: {cBuyPacket.Amount} Price: {cBuyPacket.Price}");
                                Logger.LogUserEvent("BAZAAR_BUY_PACKET", Session.GenerateIdentity(), $"Packet string: {cBuyPacket.OriginalContent.ToString()}");
                            }
                        }
                    }
                    else
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateModal(Language.Instance.GetMessageFromKey("STATE_CHANGED"), 1));
                    }
                }
                else
                {
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateModal(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 1));
                }
            }
            else
            {
                Session.SendPacket(
                    UserInterfaceHelper.GenerateModal(Language.Instance.GetMessageFromKey("STATE_CHANGED"), 1));
            }
        }

        #endregion
    }
}