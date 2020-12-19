using System;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Bazaar
{
    public class CModPacketHandler : IPacketHandler
    {
        #region Instantiation

        public CModPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void ModPriceBazaar(CModPacket cModPacket)
        {

            if (Session.Character.LastBazaarModeration.AddMinutes(5) > DateTime.Now)
            {
                return;
            }

            if (Session.Character.IsMuted())
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg("You are sanctioned you cannot do this", 0));
                return;
            }

            if (!Session.Character.VerifiedLock)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CHARACTER_LOCKED_USE_UNLOCK"), 0));
                return;
            }

            if (!Session.Character.CanUseNosBazaar())
            {
                Session.SendPacket(
                    UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("INFO_BAZAAR")));
                return;
            }

            var bz = DAOFactory.BazaarItemDAO.LoadById(cModPacket.BazaarId);
            if (bz != null)
            {
                if (bz.SellerId != Session.Character.CharacterId)
                {
                    return;
                }

                var itemInstance = new ItemInstance(DAOFactory.ItemInstanceDAO.LoadById(bz.ItemInstanceId));

                if (itemInstance == null || bz.Amount != itemInstance.Amount)
                {
                    return;
                }

                if ((bz.DateStart.AddHours(bz.Duration).AddDays(bz.MedalUsed ? 30 : 7) - DateTime.Now).TotalMinutes <= 0)
                {
                    return;
                }

                if (cModPacket.Price <= 0)
                {
                    return;
                }

                var medal = Session.Character.StaticBonusList.Find(s =>
                    s.StaticBonusType == StaticBonusType.BazaarMedalGold
                    || s.StaticBonusType == StaticBonusType.BazaarMedalSilver);
                if (cModPacket.Price >= (medal == null ? 1000000 : ServerManager.Instance.Configuration.MaxGold))
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("PRICE_EXCEEDED"), 0));
                    return;
                }

                bz.Price = cModPacket.Price;

                DAOFactory.BazaarItemDAO.InsertOrUpdate(ref bz);
                ServerManager.Instance.BazaarRefresh(bz.BazaarItemId);

                Session.SendPacket(Session.Character.GenerateSay(
                    string.Format(Language.Instance.GetMessageFromKey("OBJECT_MOD_IN_BAZAAR"), bz.Price),
                    10));
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                    string.Format(Language.Instance.GetMessageFromKey("OBJECT_MOD_IN_BAZAAR"), bz.Price),
                    0));

                Logger.LogUserEvent("BAZAAR_MOD", Session.GenerateIdentity(),
                    $"BazaarId: {bz.BazaarItemId}, IIId: {bz.ItemInstanceId} VNum: {itemInstance.ItemVNum} Amount: {bz.Amount} Price: {bz.Price} Time: {bz.Duration}");
                Logger.LogUserEvent("BAZAAR_BUY_PACKET", Session.GenerateIdentity(), $"Packet string: {cModPacket.OriginalContent.ToString()}");

                new CSListPacketHandler(Session).RefreshPersonalBazarList(new CSListPacket());

                Session.Character.LastBazaarModeration = DateTime.Now;
            }
        }

        #endregion
    }
}