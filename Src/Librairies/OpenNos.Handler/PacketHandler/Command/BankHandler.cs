using NosTale.Extension.GameExtension.Character;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class BankHandler : IPacketHandler
    {
        #region Instantiation

        public BankHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void BankManagement(BankPacket bankPacket)
        {
            if (Session.Character.IsMuted())
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg("You're mute, you can't do this", 0));
                return;
            }

            if (!Session.Character.VerifiedLock)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg("You cant do this because your account is blocked. Use $Unlock", 0));
                return;
            }


            if (bankPacket != null)
            {
                switch (bankPacket.Mode?.ToLower())
                {
                    case "balance":
                        {
                            Logger.LogEvent("BANK", $"[{Session.GenerateIdentity()}][Balance]Balance: {Session.Character.GoldBank}");

                            Session.SendPacket(Session.Character.GenerateSay($"Current Balance: {Session.Character.GoldBank} Gold.", 10));
                            return;
                        }
                    case "deposit":
                        {
                            if (bankPacket.Param1 != null && (long.TryParse(bankPacket.Param1, out long amount) || string.Equals(bankPacket.Param1, "all", StringComparison.OrdinalIgnoreCase)))
                            {
                                if (string.Equals(bankPacket.Param1, "all", StringComparison.OrdinalIgnoreCase) && Session.Character.Gold > 0)
                                {
                                    Logger.LogEvent("BANK", $"[{Session.GenerateIdentity()}][Deposit]Amount: {Session.Character.Gold} OldBank: {Session.Character.GoldBank} NewBank: {Session.Character.GoldBank + Session.Character.Gold}");

                                    Session.SendPacket(Session.Character.GenerateSay($"Deposited ALL({Session.Character.Gold}) Gold.",10));
                                    Session.Character.GoldBank += Session.Character.Gold;
                                    Session.Character.Gold = 0;
                                    Session.SendPacket(Session.Character.GenerateGold());
                                    Session.SendPacket(Session.Character.GenerateSay($"New Balance: {Session.Character.GoldBank} Gold.",10));
                                }
                                else if (amount <= Session.Character.Gold && Session.Character.Gold > 0)
                                {
                                    if (amount < 1)
                                    {
                                        Logger.LogEvent("BANK",
                                            $"[{Session.GenerateIdentity()}][Illegal]Mode: {bankPacket.Mode} Param1: {bankPacket.Param1} Param2: {bankPacket.Param2}");

                                        Session.SendPacket(Session.Character.GenerateSay(
                                            "I'm afraid I can't let you do that. This incident has been logged.", 10));
                                    }
                                    else
                                    {
                                        Logger.LogEvent("BANK",
                                            $"[{Session.GenerateIdentity()}][Deposit]Amount: {amount} OldBank: {Session.Character.GoldBank} NewBank: {Session.Character.GoldBank + amount}");

                                        Session.SendPacket(Session.Character.GenerateSay($"Deposited {amount} Gold.", 10));
                                        Session.Character.GoldBank += amount;
                                        Session.Character.Gold -= amount;
                                        Session.SendPacket(Session.Character.GenerateGold());
                                        Session.SendPacket(
                                            Session.Character.GenerateSay(
                                                $"New Balance: {Session.Character.GoldBank} Gold.", 10));
                                    }
                                }
                            }

                            return;
                        }
                    case "withdraw":
                        {
                            if (bankPacket.Param1 != null && long.TryParse(bankPacket.Param1, out long amount) && amount <= Session.Character.GoldBank && Session.Character.GoldBank > 0 && (Session.Character.Gold + amount) <= ServerManager.Instance.Configuration.MaxGold)
                            {
                                if (amount < 1)
                                {
                                    Logger.LogEvent("BANK", $"[{Session.GenerateIdentity()}][Illegal]Mode: {bankPacket.Mode} Param1: {bankPacket.Param1} Param2: {bankPacket.Param2}");

                                    Session.SendPacket(Session.Character.GenerateSay("I'm afraid I can't let you do that. This incident has been logged.", 10));
                                }
                                else
                                {
                                    Logger.LogEvent("BANK", $"[{Session.GenerateIdentity()}][Withdraw]Amount: {amount} OldBank: {Session.Character.GoldBank} NewBank: {Session.Character.GoldBank - amount}");

                                    Session.SendPacket(Session.Character.GenerateSay($"Withdrawn {amount} Gold.", 10));
                                    Session.Character.GoldBank -= amount;
                                    Session.Character.Gold += amount;
                                    Session.SendPacket(Session.Character.GenerateGold());
                                    Session.SendPacket(Session.Character.GenerateSay($"New Balance: {Session.Character.GoldBank} Gold.", 10));
                                }
                            }

                            return;
                        }
                    case "send":
                        {
                            if (bankPacket.Param1 != null)
                            {
                                long amount = bankPacket.Param2;
                                ClientSession receiver = ServerManager.Instance.GetSessionByCharacterName(bankPacket.Param1);
                                if (amount <= Session.Character.GoldBank && Session.Character.GoldBank > 0 && receiver != null)
                                {
                                    if (amount < 1)
                                    {
                                        Logger.LogEvent("BANK", $"[{Session.GenerateIdentity()}][Illegal]Mode: {bankPacket.Mode} Param1: {bankPacket.Param1} Param2: {bankPacket.Param2}");

                                        Session.SendPacket(Session.Character.GenerateSay( "I'm afraid I can't let you do that. This incident has been logged.", 10));
                                    }
                                    else
                                    {
                                        Logger.LogEvent("BANK", $"[{Session.GenerateIdentity()}][Send]Amount: {amount} OldBankSender: {Session.Character.GoldBank} NewBankSender: {Session.Character.GoldBank - amount} OldBankReceiver: {receiver.Character.GoldBank} NewBankReceiver: {receiver.Character.GoldBank + amount}");

                                        Session.SendPacket(Session.Character.GenerateSay($"Sent {amount} Gold to {receiver.Character.Name}", 10));
                                        receiver.SendPacket(Session.Character.GenerateSay($"Received {amount} Gold from {Session.Character.Name}", 10));
                                        Session.Character.GoldBank -= amount;
                                        receiver.Character.GoldBank += amount;
                                        Session.SendPacket(Session.Character.GenerateSay($"New Balance: {Session.Character.GoldBank} Gold.", 10));
                                        receiver.SendPacket(Session.Character.GenerateSay($"New Balance: {receiver.Character.GoldBank} Gold.", 10));
                                    }
                                }
                            }

                            return;
                        }
                    default:
                        {
                            Session.SendPacket(Session.Character.GenerateSay(BankPacket.ReturnHelp(), 10));
                            return;
                        }
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(BankPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}