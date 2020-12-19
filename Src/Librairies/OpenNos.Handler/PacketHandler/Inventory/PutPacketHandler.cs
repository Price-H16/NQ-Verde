using NosTale.Configuration;
using NosTale.Configuration.Utilities;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Inventory
{
    public class PutPacketHandler : IPacketHandler
    {
        #region Instantiation

        public PutPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void PutItem(PutPacket putPacket)
        {

            if (putPacket == null || Session.Character.HasShopOpened) return;

            if (putPacket.InventoryType != InventoryType.Equipment
                && putPacket.InventoryType != InventoryType.Main
                && putPacket.InventoryType != InventoryType.Etc)
            {
                return;
            }

            lock (Session.Character.Inventory)
            {
                var invitem =
                    Session.Character.Inventory.LoadBySlotAndType(putPacket.Slot, putPacket.InventoryType);
                if (invitem?.Item.IsDroppable == true && invitem.Item.IsTradable
                                                      && !Session.Character.InExchangeOrTrade &&
                                                      putPacket.InventoryType != InventoryType.Bazaar)
                {
                    if (putPacket.Amount > 0 && putPacket.Amount <=
                        DependencyContainer.Instance.GetInstance<JsonItemConfiguration>().Inventory.MaxItemPerSlot)
                    {
                        if (Session.Character.MapInstance.DroppedList.Count < 200 && Session.HasCurrentMapInstance)
                        {
                            var droppedItem = Session.CurrentMapInstance.PutItem(putPacket.InventoryType,
                                putPacket.Slot, putPacket.Amount, ref invitem, Session);
                            if (droppedItem == null)
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("ITEM_NOT_DROPPABLE_HERE"), 0));
                                return;
                            }

                            Session.SendPacket(invitem.GenerateInventoryAdd());

                            if (invitem.Amount == 0) Session.Character.DeleteItem(invitem.Type, invitem.Slot);

                            Logger.LogUserEvent("CHARACTER_ITEM_DROP", Session.GenerateIdentity(),
                                $"[PutItem]IIId: {invitem.Id} ItemVNum: {droppedItem.ItemVNum} Amount: {droppedItem.Amount} MapId: {Session.CurrentMapInstance.Map.MapId} MapX: {droppedItem.PositionX} MapY: {droppedItem.PositionY}");
                            Session.CurrentMapInstance?.Broadcast(
                                $"drop {droppedItem.ItemVNum} {droppedItem.TransportId} {droppedItem.PositionX} {droppedItem.PositionY} {droppedItem.Amount} 0 -1");
                        }
                        else
                        {
                            Session.SendPacket(
                                UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("DROP_MAP_FULL"),
                                    0));
                        }
                    }
                    else
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("BAD_DROP_AMOUNT"), 0));
                    }
                }
                else
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_NOT_DROPPABLE"), 0));
                }
            }
        }

        #endregion
    }
}