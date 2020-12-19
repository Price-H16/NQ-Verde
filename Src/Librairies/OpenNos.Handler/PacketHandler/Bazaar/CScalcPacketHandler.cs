﻿using System;
using System.Reactive.Linq;
using System.Threading;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Bazaar
{
    public class CScalcPacketHandler : IPacketHandler
    {
        #region Instantiation

        public CScalcPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void GetBazaar(CScalcPacket cScalcPacket)
        {
            if (!Session.Character.CanUseNosBazaar())
            {
                Session.SendPacket(
                    UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("INFO_BAZAAR")));
                return;
            }

            SpinWait.SpinUntil(() => !ServerManager.Instance.InBazaarRefreshMode);

            var bazaarItemDTO = DAOFactory.BazaarItemDAO.LoadById(cScalcPacket.BazaarId);

            if (bazaarItemDTO != null)
            {
                var itemInstanceDTO = DAOFactory.ItemInstanceDAO.LoadById(bazaarItemDTO.ItemInstanceId);
                
                if (itemInstanceDTO == null)
                {
                    return;
                }

                var itemInstance = new ItemInstance(itemInstanceDTO);

                if (itemInstance == null)
                {
                    return;
                }

                if (bazaarItemDTO.SellerId != Session.Character.CharacterId)
                {
                    return;
                }

                if ((bazaarItemDTO.DateStart.AddHours(bazaarItemDTO.Duration)
                                  .AddDays(bazaarItemDTO.MedalUsed ? 30 : 7) - DateTime.Now).TotalMinutes <= 0)
                {
                    return;
                }

                var soldAmount = bazaarItemDTO.Amount - itemInstance.Amount;
                var taxes = bazaarItemDTO.MedalUsed ? 0 : (long) (bazaarItemDTO.Price * 0.10 * soldAmount);
                var price = bazaarItemDTO.Price * soldAmount - taxes;

                var name = itemInstance.Item?.Name ?? "None";
                
                if (bazaarItemDTO.DateStart.AddMinutes(2) >= DateTime.Now)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateInfo("You have to wait at least 2 minutes after publishing the item to take it out from NosBazaar"));
                    return;
                }

                if (itemInstance.Amount == 0 || Session.Character.Inventory.CanAddItem(itemInstance.ItemVNum))
                {

                    if (itemInstance.Amount != bazaarItemDTO.Amount - cScalcPacket.Amount)
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("EXPLOIT_LOGGED"), 0));
                        return;
                    }
                    if (Session.Character.Gold + price <= ServerManager.Instance.Configuration.MaxGold)
                    {
                        Session.Character.Gold += price;
                        Session.SendPacket(Session.Character.GenerateGold());
                        Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("REMOVE_FROM_BAZAAR"), price), 10));

                        // Edit this soo we dont generate new guid every single time we take something out.
                        if (itemInstance.Amount != 0)
                        {
                            ItemInstance newItemInstance = itemInstance.DeepCopy();
                            newItemInstance.Id = Guid.NewGuid();
                            newItemInstance.Type = newItemInstance.Item.Type;
                            Session.Character.Inventory.AddToInventory(newItemInstance);
                        }

                        Session.SendPacket(UserInterfaceHelper.GenerateBazarRecollect(bazaarItemDTO.Price, soldAmount,
                            bazaarItemDTO.Amount, taxes, price, name));

                        Logger.LogUserEvent("BAZAAR_REMOVE", Session.GenerateIdentity(),
                            $"BazaarId: {cScalcPacket.BazaarId}, IId: {itemInstance.Id} VNum: {itemInstance.ItemVNum} Amount: {bazaarItemDTO.Amount} RemainingAmount: {itemInstance.Amount} Price: {bazaarItemDTO.Price}");

                        if (DAOFactory.BazaarItemDAO.LoadById(bazaarItemDTO.BazaarItemId) != null)
                        {
                            DAOFactory.BazaarItemDAO.Delete(bazaarItemDTO.BazaarItemId);
                        }

                        DAOFactory.ItemInstanceDAO.Delete(itemInstance.Id);

                        Session.Character.Inventory.RemoveItemFromInventory(itemInstance.Id, itemInstance.Amount);

                        ServerManager.Instance.BazaarRefresh(bazaarItemDTO.BazaarItemId);

                        Observable.Timer(TimeSpan.FromMilliseconds(1000)).Subscribe(o =>
                            new CSListPacketHandler(Session).RefreshPersonalBazarList(new CSListPacket()));
                    }
                    else
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("MAX_GOLD"), 0));
                        Session.SendPacket($"rc_scalc 0 -1 -1 -1 -1 -1 -1");
                    }
                }
                else
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE")));
                    Session.SendPacket($"rc_scalc 0 -1 -1 -1 -1 -1 -1");
                }
            }
            else
            {
                Session.SendPacket($"rc_scalc 0 -1 -1 -1 -1 -1 -1");
            }
        }

        #endregion
    }
}