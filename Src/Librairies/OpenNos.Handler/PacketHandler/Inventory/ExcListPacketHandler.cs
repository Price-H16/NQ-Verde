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

        public void ExchangeList(ExcListPacket packet)
        {

            Logger.LogUserEvent("EXC_LIST", Session.GenerateIdentity(),
                $"Packet string: {packet}");

            if (Session.Character.ExchangeInfo == null) return;

            if (Session.Character.ExchangeInfo.Gold != 0) return;

            if (Session.Character.ExchangeInfo.BankGold != 0) return;

            var targetSession =
                ServerManager.Instance.GetSessionByCharacterId(Session.Character.ExchangeInfo.TargetCharacterId);
            if (Session.Character.HasShopOpened || targetSession?.Character.HasShopOpened == true)
            {
                Session.CloseExchange(targetSession);
                return;
            }

            var packetsplit = packet.PacketData.Split(' ');
            if (packetsplit.Length < 2)
            {
                Session.SendPacket("exc_close 0");
                Session.CurrentMapInstance?.Broadcast(Session, "exc_close 0", ReceiverType.OnlySomeone,
                    "", Session.Character.ExchangeInfo.TargetCharacterId);

                if (targetSession != null) targetSession.Character.ExchangeInfo = null;
                Session.Character.ExchangeInfo = null;
                return;
            }

            if (!long.TryParse(packetsplit[0], out var gold)) return;

            if (!long.TryParse(packetsplit[1], out var bankGold)) return;

            var type = new byte[10];
            short[] slot = new short[10], qty = new short[10];
            var packetList = "";

            if (gold < 0 || gold > Session.Character.Gold ||
                bankGold < 0 || bankGold > Session.Account.BankMoney / 1000 ||
                Session.Character.ExchangeInfo == null || Session.Character.ExchangeInfo.ExchangeList.Any())
                return;

            for (int j = 4, i = 0; j <= packetsplit.Length && i < 10; j += 3, i++)
            {
                byte.TryParse(packetsplit[j - 2], out type[i]);
                short.TryParse(packetsplit[j - 1], out slot[i]);
                short.TryParse(packetsplit[j], out qty[i]);
                if ((InventoryType)type[i] == InventoryType.Bazaar)
                {
                    Session.CloseExchange(targetSession);
                    return;
                }

                var item = Session.Character.Inventory.LoadBySlotAndType(slot[i], (InventoryType)type[i]);
                if (item == null) return;

                if (qty[i] <= 0 || item.Amount < qty[i]) return;

                var it = item.DeepCopy();
                if (it.Item.IsTradable && !it.IsBound)
                {
                    it.Amount = qty[i];
                    Session.Character.ExchangeInfo.ExchangeList.Add(it);
                    if (type[i] != 0)
                        packetList += $"{i}.{type[i]}.{it.ItemVNum}.{qty[i]} ";
                    else
                        packetList += $"{i}.{type[i]}.{it.ItemVNum}.{it.Rare}.{it.Upgrade} ";
                }
                else if (it.IsBound)
                {
                    Session.SendPacket("exc_close 0");
                    Session.CurrentMapInstance?.Broadcast(Session, "exc_close 0", ReceiverType.OnlySomeone,
                        "", Session.Character.ExchangeInfo.TargetCharacterId);

                    if (targetSession != null) targetSession.Character.ExchangeInfo = null;
                    Session.Character.ExchangeInfo = null;
                    return;
                }
            }

            Session.Character.ExchangeInfo.Gold = gold;
            Session.Character.ExchangeInfo.BankGold = bankGold * 1000;
            Session.CurrentMapInstance?.Broadcast(Session,
                $"exc_list 1 {Session.Character.CharacterId} {gold} {bankGold} {packetList}", ReceiverType.OnlySomeone,
                "", Session.Character.ExchangeInfo.TargetCharacterId);
            Session.Character.ExchangeInfo.Validated = true;
        }

        #endregion
    }
}