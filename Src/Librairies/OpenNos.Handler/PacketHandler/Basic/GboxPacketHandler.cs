using NosTale.Packets.Packets.ServerPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Basic
{
    public class GboxPacketHandler : IPacketHandler
    {
        #region Instantiation

        public GboxPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void BankAction(GboxPacket gboxPacket)
        {
            if (Session.Character.InExchangeOrTrade) return;

            switch (gboxPacket.Type)
            {
                case BankActionType.Deposit:
                    if (gboxPacket.Option == 0)
                    {
                        Session.SendPacket(
                            $"qna #gbox^1^{gboxPacket.Amount}^1 Want to deposit {gboxPacket.Amount}000 gold?");
                        return;
                    }

                    if (gboxPacket.Option == 1)
                    {
                        if (gboxPacket.Amount <= 0)
                            //Packet hacking duplication
                            return;

                        Session.SendPacket(UserInterfaceHelper.GenerateShopMemo((byte)SmemoType.Information,
                            Language.Instance.GetMessageFromKey("BANK_DEPOSIT")));
                        if (Session.Account.BankMoney + gboxPacket.Amount * 1000 >
                            ServerManager.Instance.Configuration.MaxGoldBank)
                        {
                            Session.SendPacket(
                                UserInterfaceHelper.GenerateInfo(
                                    Language.Instance.GetMessageFromKey("MAX_GOLD_BANK_REACHED")));
                            Session.SendPacket(UserInterfaceHelper.GenerateShopMemo((byte)SmemoType.Error,
                                Language.Instance.GetMessageFromKey("MAX_GOLD_BANK_REACHED")));
                            return;
                        }

                        if (Session.Character.Gold < gboxPacket.Amount * 1000)
                        {
                            Session.SendPacket(
                                UserInterfaceHelper.GenerateInfo(
                                    Language.Instance.GetMessageFromKey("NOT_ENOUGH_GOLD")));
                            Session.SendPacket(UserInterfaceHelper.GenerateShopMemo((byte)SmemoType.Error,
                                Language.Instance.GetMessageFromKey("NOT_ENOUGH_GOLD")));
                            return;
                        }

                        Session.Account.BankMoney += gboxPacket.Amount * 1000;
                        Session.Character.Gold -= gboxPacket.Amount * 1000;
                        Session.SendPacket(Session.Character.GenerateGold());
                        Session.SendPacket(Session.Character.GenerateGb((byte)GoldBankPacketType.Deposit));
                        Session.SendPacket(Session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("BANK_BALANCE"), Session.Account.BankMoney,
                                Session.Character.Gold), 1));
                        Session.SendPacket(UserInterfaceHelper.GenerateShopMemo((byte)SmemoType.Balance,
                            string.Format(Language.Instance.GetMessageFromKey("BANK_BALANCE"), Session.Account.BankMoney,
                                Session.Character.Gold)));
                    }

                    break;

                case BankActionType.Withdraw:
                    if (gboxPacket.Option == 0)
                    {
                        Session.SendPacket(
                            $"qna #gbox^2^{gboxPacket.Amount}^1 Would you like to withdraw {gboxPacket.Amount}000 gold? (Fee: 0 gold)");
                        return;
                    }

                    if (gboxPacket.Option == 1)
                    {
                        if (gboxPacket.Amount <= 0)
                            //Packet hacking duplication
                            return;

                        Session.SendPacket(UserInterfaceHelper.GenerateShopMemo((byte)SmemoType.Information,
                            Language.Instance.GetMessageFromKey("WITHDRAW_BANK")));
                        if (Session.Character.Gold + gboxPacket.Amount * 1000 >
                            ServerManager.Instance.Configuration.MaxGoldBank)
                        {
                            Session.SendPacket(
                                UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("TOO_MUCH_GOLD")));
                            Session.SendPacket(UserInterfaceHelper.GenerateShopMemo((byte)SmemoType.Error,
                                Language.Instance.GetMessageFromKey("TOO_MUCH_GOLD")));
                            return;
                        }

                        if (Session.Account.BankMoney < gboxPacket.Amount * 1000)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateInfo("NOT_ENOUGH_FUNDS"));
                            Session.SendPacket(UserInterfaceHelper.GenerateShopMemo((byte)SmemoType.Error,
                                Language.Instance.GetMessageFromKey("NOT_ENOUGH_FUNDS")));
                            return;
                        }

                        Session.Account.BankMoney -= gboxPacket.Amount * 1000;
                        Session.Character.Gold += gboxPacket.Amount * 1000;
                        Session.SendPacket(Session.Character.GenerateGold());
                        Session.SendPacket(Session.Character.GenerateGb((byte)GoldBankPacketType.Withdraw));
                        Session.SendPacket(UserInterfaceHelper.GenerateShopMemo((byte)SmemoType.Balance,
                            Language.Instance.GetMessageFromKey("BANK_BALANCE")));
                    }

                    break;
            }
        }

        #endregion
    }
}