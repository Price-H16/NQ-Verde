using System;
using System.Linq;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Core.Extensions;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Npc
{
    public class MShopPacketHandler : IPacketHandler
    {
        #region Instantiation

        public MShopPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void CreateShop(string packet)
        {
            if (!Session.Character.VerifiedLock)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CHARACTER_LOCKED_USE_UNLOCK"), 0));
                return;
            }

            string[] packetsplit = packet.Split(' ');
            InventoryType[] type = new InventoryType[20];
            long[] gold = new long[20];
            short[] slot = new short[20];
            short[] qty = new short[20];
            string shopname = "";
            if (packetsplit.Length > 2)
            {
                if (!short.TryParse(packetsplit[2], out short typePacket))
                {
                    return;
                }

                if ((Session.Character.HasShopOpened && typePacket != 1) || !Session.HasCurrentMapInstance || Session.Character.IsExchanging || Session.Character.ExchangeInfo != null)
                {
                    return;
                }

                if (Session.CurrentMapInstance.Portals.Any(por => Session.Character.PositionX < por.SourceX + 6 && Session.Character.PositionX > por.SourceX - 6 && Session.Character.PositionY < por.SourceY + 6 && Session.Character.PositionY > por.SourceY - 6))
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SHOP_NEAR_PORTAL"), 0));
                    return;
                }

                if (Session.Character.Group != null && Session.Character.Group?.GroupType != GroupType.Group)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SHOP_NOT_ALLOWED_IN_RAID"),0));
                    return;
                }

                if (!Session.CurrentMapInstance.ShopAllowed)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SHOP_NOT_ALLOWED"), 0));
                    return;
                }

                switch (typePacket)
                {
                    case 2:
                        Session.SendPacket("ishop");
                        break;

                    case 0:
                        if (Session.CurrentMapInstance.UserShops.Any(s => s.Value.OwnerId == Session.Character.CharacterId))
                        {
                            return;
                        }

                        MapShop myShop = new MapShop();

                        if (packetsplit.Length > 82)
                        {
                            short shopSlot = 0;

                            for (short j = 3, i = 0; j < 82; j += 4, i++)
                            {
                                Enum.TryParse(packetsplit[j], out type[i]);
                                short.TryParse(packetsplit[j + 1], out slot[i]);
                                short.TryParse(packetsplit[j + 2], out qty[i]);

                                long.TryParse(packetsplit[j + 3], out gold[i]);
                                if (gold[i] < 0)
                                {
                                    return;
                                }

                                if (qty[i] > 0)
                                {
                                    ItemInstance inv = Session.Character.Inventory.LoadBySlotAndType(slot[i], type[i]);
                                    if (inv != null)
                                    {
                                        if (inv.Amount < qty[i])
                                        {
                                            return;
                                        }

                                        if (myShop.Items.Where(s => s.ItemInstance.ItemVNum == inv.ItemVNum).Sum(s => s.SellAmount) + qty[i] > Session.Character.Inventory.CountItem(inv.ItemVNum))
                                        {
                                            return;
                                        }

                                        if (!inv.Item.IsTradable || inv.IsBound)
                                        {
                                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SHOP_ONLY_TRADABLE_ITEMS"), 0));
                                            Session.SendPacket("shop_end 0");
                                            return;
                                        }

                                        PersonalShopItem personalshopitem = new PersonalShopItem
                                        {
                                            ShopSlot = shopSlot,
                                            Price = gold[i],
                                            ItemInstance = inv,
                                            SellAmount = qty[i]
                                        };
                                        myShop.Items.Add(personalshopitem);
                                        shopSlot++;
                                    }
                                }
                            }
                        }

                        if (myShop.Items.Count != 0)
                        {
                            if (!myShop.Items.Any(s => !s.ItemInstance.Item.IsSoldable || s.ItemInstance.IsBound))
                            {
                                for (int i = 83; i < packetsplit.Length; i++)
                                {
                                    shopname += $"{packetsplit[i]} ";
                                }

                                // trim shopname
                                shopname = shopname.TrimEnd(' ');

                                // create default shopname if it's empty
                                if (string.IsNullOrWhiteSpace(shopname) || string.IsNullOrEmpty(shopname))
                                {
                                    shopname = Language.Instance.GetMessageFromKey("SHOP_PRIVATE_SHOP");
                                }

                                // truncate the string to a max-length of 20
                                shopname = shopname.Truncate(20);
                                myShop.OwnerId = Session.Character.CharacterId;
                                myShop.Name = shopname;
                                Session.CurrentMapInstance.UserShops.Add(Session.CurrentMapInstance.LastUserShopId++, myShop);

                                Session.Character.HasShopOpened = true;

                                Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GeneratePlayerFlag(Session.CurrentMapInstance.LastUserShopId), ReceiverType.AllExceptMe);
                                Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateShop(shopname));
                                Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("SHOP_OPEN")));

                                Session.Character.IsSitting = true;
                                Session.Character.IsShopping = true;

                                Session.Character.LoadSpeed();
                                Session.SendPacket(Session.Character.GenerateCond());
                                Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateRest());
                            }
                            else
                            {
                                Session.SendPacket("shop_end 0");
                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("ITEM_NOT_SOLDABLE"), 10));
                            }
                        }
                        else
                        {
                            Session.SendPacket("shop_end 0");
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("SHOP_EMPTY"), 10));
                        }

                        break;
                    case 1:
                        Session.Character.CloseShop();
                        break;
                }
            }
        }

        #endregion
    }
}