using System;
using System.Threading;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Bazaar
{
    public class CRegPacketHandler : IPacketHandler
    {
        #region Instantiation

        public CRegPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void SellBazaar(CRegPacket cRegPacket)
        {
            if (cRegPacket.Inventory != 0 && cRegPacket.Inventory != 1 && cRegPacket.Inventory != 2 && cRegPacket.Inventory != 4) // Dupe
            {
                return;
            }
            if (Session.Character == null)
            {
                return;
            }

            if (Session.Character.LastBazaarInsert.AddSeconds(5) > DateTime.Now)
            {
                return;
            }

            if (ServerManager.Instance.InShutdown)
            {
                return;
            }

            if (Session.Character == null || Session.Character.InExchangeOrTrade || Session.Character.HasShopOpened || Session.Character.MapInstance?.Map.MapId == 20001)
            {
                return;
            }

            if (Session.Character.IsMuted())
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg("Tu es sanctonné tu ne peux pas faire ça", 0));
                return;
            }
            
            if (!Session.Character.VerifiedLock)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg("You cant do this because your account is blocked. Use $Unlock", 0));
                return;
            }

            if (!Session.Character.CanUseNosBazaar())
            {
                Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("INFO_BAZAAR")));
                return;
            }

            if ((InventoryType)cRegPacket.Inventory == InventoryType.Bazaar)
            {
                //Dupe.
                return;
            }
            if (cRegPacket.Type == 9)
            {
                return;
            }

            if (cRegPacket.Inventory < 0 || cRegPacket.Inventory > 4 || cRegPacket.Inventory == 3 || cRegPacket.Taxes < 1 || cRegPacket.Taxes > 2000000000 || cRegPacket.Price < 1 || cRegPacket.Price > 2000000000 || cRegPacket.Durability > 4 || cRegPacket.Durability < 1)
            {
                Logger.Info($"{Session.Character.Name} tried to dupe via bazar");
                ServerManager.Instance.Kick(Session.Character.Name);
                Logger.LogUserEvent("BAZAAR_CHEAT_TRY", Session.GenerateIdentity(), $"Packet string: {cRegPacket.OriginalContent.ToString()}");
                return;
            }

            if (cRegPacket.Price * cRegPacket.Amount < 501)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("MINIMUM_PRICE_IS_501")));
                return;
            }


            SpinWait.SpinUntil(() => !ServerManager.Instance.InBazaarRefreshMode);
            var medal = Session.Character.StaticBonusList.Find(s =>
                s.StaticBonusType == StaticBonusType.BazaarMedalGold
                || s.StaticBonusType == StaticBonusType.BazaarMedalSilver);

            var price = cRegPacket.Price * cRegPacket.Amount;
            var taxmax = price > 100000 ? price / 200 : 500;
            var taxmin = price >= 4000
                ? 60 + (price - 4000) / 2000 * 30 > 10000 ? 10000 : 60 + (price - 4000) / 2000 * 30
                : 50;
            var tax = medal == null ? taxmax : taxmin;
            var maxGold = ServerManager.Instance.Configuration.MaxGold;
            if (Session.Character.Gold < tax || cRegPacket.Amount <= 0
                                             || Session.Character.ExchangeInfo?.ExchangeList.Count > 0 ||
                                             Session.Character.IsShopping)
                return;

            var it = Session.Character.Inventory.LoadBySlotAndType(cRegPacket.Slot,
                cRegPacket.Inventory == 4 ? 0 : (InventoryType)cRegPacket.Inventory);

            // This is done on purpose, because when an item is fresh created and has shell (hero eq), its shell effects are being deleted cause they're not being saved in DB before adding items to NB.
            // TODO: Find a better way to do it, ending the issue from the root.
            Session.Character.PerformItemSave(it);//pogg

            if (it == null || !it.Item.IsSoldable || !it.Item.IsTradable ||
                it.IsBound && it.ItemDeleteTime != null) return;

            if (Session.Character.Inventory.CountItemInAnInventory(InventoryType.Bazaar)
                >= 10 * (medal == null ? 2 : 10))
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("LIMIT_EXCEEDED"), 0));
                return;
            }

            if (it.Amount < 1)
            {
                return;
            }

            if (cRegPacket.Price >= (medal == null ? 1000000 : maxGold))
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("PRICE_EXCEEDED"), 0));
                return;
            }

            if (cRegPacket.Price <= 0) return;

            var bazaar = Session.Character.Inventory.AddIntoBazaarInventory(
                cRegPacket.Inventory == 4 ? 0 : (InventoryType)cRegPacket.Inventory, cRegPacket.Slot,
                cRegPacket.Amount);
            if (bazaar == null) return;

            short duration;
            switch (cRegPacket.Durability)
            {
                case 1:
                    duration = 24;
                    break;

                case 2:
                    duration = 168;
                    break;

                case 3:
                    duration = 360;
                    break;

                case 4:
                    duration = 720;
                    break;

                default:
                    return;
            }

            DAOFactory.ItemInstanceDAO.InsertOrUpdate(bazaar);

            var bazaarItem = new BazaarItemDTO
            {
                Amount = bazaar.Amount,
                DateStart = DateTime.Now,
                Duration = duration,
                IsPackage = cRegPacket.IsPackage != 0,
                MedalUsed = medal != null,
                Price = cRegPacket.Price,
                SellerId = Session.Character.CharacterId,
                ItemInstanceId = bazaar.Id
            };

            DAOFactory.BazaarItemDAO.InsertOrUpdate(ref bazaarItem);
            ServerManager.Instance.BazaarRefresh(bazaarItem.BazaarItemId);

            Session.Character.Gold -= tax;
            Session.SendPacket(Session.Character.GenerateGold());

            //Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("OBJECT_IN_BAZAAR"),
            //    10));
            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("OBJECT_IN_BAZAAR"),
                0));

            Session.Character.LastBazaarInsert = DateTime.Now;

            Logger.LogUserEvent("BAZAAR_INSERT", Session.GenerateIdentity(),
                $"BazaarId: {bazaarItem.BazaarItemId}, IIId: {bazaarItem.ItemInstanceId} VNum: {bazaar.ItemVNum} Amount: {cRegPacket.Amount} Price: {cRegPacket.Price} Time: {duration}");

            Session.SendPacket("rc_reg 1");
        }

        #endregion
    }
}