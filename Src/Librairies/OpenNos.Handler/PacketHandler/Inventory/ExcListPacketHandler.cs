using System.Linq;
using NosTale.Extension.Extension.Packet;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Core.Handling;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Inventory
{
    public class ExcListPacketHandler : IPacketHandler
    {
        #region Instantiation

        public ExcListPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        [Packet("exc_list")]
        public void ExchangeList(string packet)
        {
            string[] packetsplit = packet.Split(' ');

            if (!long.TryParse(packetsplit[2], out long gold))
            {
                return;
            }

            if (!long.TryParse(packetsplit[3], out long GoldBank))
            {
                return;
            }

            if (gold < 0 || gold > Session.Character.Gold || GoldBank < 0 || GoldBank > Session.Character.GoldBank || Session.Character.ExchangeInfo == null || Session.Character.ExchangeInfo.ExchangeList.Any())
            {
                return;
            }

            if (!Session.Character.VerifiedLock)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CHARACTER_LOCKED_USE_UNLOCK"), 0));
                return;
            }

            Logger.LogUserEvent("EXC_LIST", Session.GenerateIdentity(),
                $"Packet string: {packet.ToString()}");

            if (Session.Character.ExchangeInfo == null)
            {
                return;
            }
            ClientSession targetSession =
                ServerManager.Instance.GetSessionByCharacterId(Session.Character.ExchangeInfo.TargetCharacterId);
            if (Session.Character.HasShopOpened || targetSession?.Character.HasShopOpened == true)
            {
                CloseExchange(Session, targetSession);
                return;
            }

            if (packetsplit.Length < 4)
            {
                Session.SendPacket("exc_close 0");
                Session.CurrentMapInstance?.Broadcast(Session, "exc_close 0", ReceiverType.OnlySomeone,
                    "", Session.Character.ExchangeInfo.TargetCharacterId);

                if (targetSession != null)
                {
                    targetSession.Character.ExchangeInfo = null;
                }
                Session.Character.ExchangeInfo = null;
                return;
            }

            byte[] type = new byte[10];
            short[] slot = new short[10], qty = new short[10];
            string packetList = "";

            if (gold < 0 || gold > Session.Character.Gold || Session.Character.ExchangeInfo == null
                || Session.Character.ExchangeInfo.ExchangeList.Count > 0)
            {
                return;
            }

            if (GoldBank < 0 || GoldBank > Session.Character.GoldBank || Session.Character.ExchangeInfo == null
                || Session.Character.ExchangeInfo.ExchangeList.Count > 0)
            {
                return;
            }

            for (int j = 7, i = 0; j <= packetsplit.Length && i < 10; j += 3, i++)
            {
                byte.TryParse(packetsplit[j - 3], out type[i]);
                short.TryParse(packetsplit[j - 2], out slot[i]);
                short.TryParse(packetsplit[j - 1], out qty[i]);
                if ((InventoryType)type[i] == InventoryType.Bazaar)
                {
                    CloseExchange(Session, targetSession);
                    return;
                }

                ItemInstance item = Session.Character.Inventory.LoadBySlotAndType(slot[i], (InventoryType)type[i]);
                if (item == null)
                {
                    return;
                }

                if (qty[i] <= 0 || item.Amount < qty[i])
                {
                    return;
                }

                ItemInstance it = item.DeepCopy();
                if (it.Item.IsTradable && !it.IsBound)
                {
                    it.Amount = qty[i];
                    Session.Character.ExchangeInfo.ExchangeList.Add(it);
                    if (type[i] != 0)
                    {
                        packetList += $"{i}.{type[i]}.{it.ItemVNum}.{qty[i]} ";
                    }
                    else
                    {
                        packetList += $"{i}.{type[i]}.{it.ItemVNum}.{it.Rare}.{it.Upgrade} ";
                    }
                }
                else if (it.IsBound)
                {
                    Session.SendPacket("exc_close 0");
                    Session.CurrentMapInstance?.Broadcast(Session, "exc_close 0", ReceiverType.OnlySomeone,
                        "", Session.Character.ExchangeInfo.TargetCharacterId);

                    if (targetSession != null)
                    {
                        targetSession.Character.ExchangeInfo = null;
                    }
                    Session.Character.ExchangeInfo = null;
                    return;
                }
            }

            Session.Character.ExchangeInfo.Gold = gold;
            Session.Character.ExchangeInfo.BankGold = GoldBank;
            Session.CurrentMapInstance?.Broadcast(Session, $"exc_list 1 {Session.Character.CharacterId} {gold} {GoldBank} {packetList}", ReceiverType.OnlySomeone, string.Empty, Session.Character.ExchangeInfo.TargetCharacterId);
            Session.Character.ExchangeInfo.Validated = true;
        }

        #endregion
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
    }
}