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

            if (Session.Character.InExchangeOrTrade)
            {
                return;
            }

            switch (gboxPacket.Type)
            {
                case BankActionType.Deposit:
                    if (gboxPacket.Option == 0)
                    {
                        Session.SendPacket($"qna #gbox^1^{gboxPacket.Amount}^1 Want to deposit {gboxPacket.Amount}000 gold?");
                        return;
                    }

                    if (gboxPacket.Option == 1)
                    {
                        if (gboxPacket.Amount <= 0)
                        {
                            //Packet hacking duplication
                            return;
                        }

                        Session.SendPacket(Session.Character.GenerateSmemo(Language.Instance.GetMessageFromKey("BANK_DEPOSIT"), (byte)SmemoType.Information));
                        if (Session.Account.BankMoney + gboxPacket.Amount * 1000 > ServerManager.Instance.MaxBankGold)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("MAX_GOLD_BANK_REACHED")));
                            Session.SendPacket(Session.Character.GenerateSmemo(Language.Instance.GetMessageFromKey("MAX_GOLD_BANK_REACHED"), (byte)SmemoType.Error));
                            return;
                        }

                        if (Session.Character.Gold < gboxPacket.Amount * 1000)
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("NOT_ENOUGH_GOLD")));
                            Session.SendPacket(Session.Character.GenerateSmemo(Language.Instance.GetMessageFromKey("NOT_ENOUGH_GOLD"), (byte)SmemoType.Error));
                            return;
                        }
                        Session.Character.GoldBank += gboxPacket.Amount * 1000;
                        Session.Character.Gold -= gboxPacket.Amount * 1000;
                        Session.SendPacket(Session.Character.GenerateGold());
                        Session.SendPacket(Session.Character.GenerateGB((byte)GoldBankPacketType.Deposit));
                        Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("BANK_BALANCE"), Session.Account.BankMoney, Session.Character.Gold), 1));
                        Session.SendPacket(Session.Character.GenerateSmemo(string.Format(Language.Instance.GetMessageFromKey("BANK_BALANCE"), Session.Account.BankMoney, Session.Character.Gold),
                            (byte)SmemoType.Balance));
                    }

                    break;

                case BankActionType.Withdraw:
                    if (gboxPacket.Option == 0)
                    {
                        Session.SendPacket($"qna #gbox^2^{gboxPacket.Amount}^1 Would you like to withdraw {gboxPacket.Amount}000 gold? (Fee: 0 gold)");
                        return;
                    }

                    if (gboxPacket.Option == 1)
                    {
                        if (gboxPacket.Amount <= 0)
                        {
                            //Packet hacking duplication
                            return;
                        }

                        Session.SendPacket(Session.Character.GenerateSmemo(Language.Instance.GetMessageFromKey("WITHDRAW_BANK"), (byte)SmemoType.Information));

                        //TODO: Too much gold

                        //TODO: Not enough Funds

                        Session.Character.GoldBank -= gboxPacket.Amount * 1000;
                        Session.Character.Gold += gboxPacket.Amount * 1000;
                        Session.SendPacket(Session.Character.GenerateGold());
                        Session.SendPacket(Session.Character.GenerateGB((byte)GoldBankPacketType.Withdraw));
                        Session.SendPacket(Session.Character.GenerateSmemo(Language.Instance.GetMessageFromKey("BANK_BALANCE"), (byte)SmemoType.Balance));
                    }

                    break;
            }

        }

        #endregion
    }
}