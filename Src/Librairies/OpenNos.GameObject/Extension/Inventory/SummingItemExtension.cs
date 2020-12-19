using NosTale.Configuration;
using NosTale.Configuration.Utilities;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject.Extension.Inventory
{
    public static class SummingItemExtension
    {
        #region Methods

        public static void SummingItem(this ItemInstance e, ClientSession session, ItemInstance itemToSum)
        {
            var conf = DependencyContainer.Instance.GetInstance<JsonItemConfiguration>().Summing;

            if (!session.HasCurrentMapInstance) return;

            if (e.Upgrade > conf.MaxSum) return;

            if (e.Upgrade + itemToSum.Upgrade >= conf.MaxSum || (itemToSum.Item.EquipmentSlot != EquipmentType.Gloves ||
                                                                 e.Item.EquipmentSlot != EquipmentType.Gloves) &&
                (e.Item.EquipmentSlot != EquipmentType.Boots || itemToSum.Item.EquipmentSlot != EquipmentType.Boots))
                return;

            if (session.Character.Gold < conf.GoldPrice[e.Upgrade + itemToSum.Upgrade]) return;

            if (session.Character.Inventory.CountItem(conf.SandVnum) <
                conf.SandAmount[e.Upgrade + itemToSum.Upgrade]) return;

            session.Character.Inventory.RemoveItemAmount(conf.SandVnum,
                (byte) conf.SandAmount[e.Upgrade + itemToSum.Upgrade]);
            session.GoldLess(conf.GoldPrice[e.Upgrade + itemToSum.Upgrade]);

            var rnd = ServerManager.RandomNumber();
            if (rnd < conf.UpSucess[e.Upgrade + itemToSum.Upgrade])
            {
                e.Upgrade += (byte) (itemToSum.Upgrade + 1);
                e.DarkResistance += (short) (itemToSum.DarkResistance + itemToSum.Item.DarkResistance);
                e.LightResistance += (short) (itemToSum.LightResistance + itemToSum.Item.LightResistance);
                e.WaterResistance += (short) (itemToSum.WaterResistance + itemToSum.Item.WaterResistance);
                e.FireResistance += (short) (itemToSum.FireResistance + itemToSum.Item.FireResistance);
                session.Character.DeleteItemByItemInstanceId(itemToSum.Id);
                session.SendPacket($"pdti 10 {e.ItemVNum} 1 27 {e.Upgrade} 0");
                session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SUM_SUCCESS"),
                    0));
                session.SendPacket(
                    session.Character.GenerateSay(Language.Instance.GetMessageFromKey("SUM_SUCCESS"), 12));
                session.SendPacket(UserInterfaceHelper.GenerateGuri(19, 1, session.Character.CharacterId, 1324));
                session.SendPacket(e.GenerateInventoryAdd());
            }
            else
            {
                session.SendPacket(
                    UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SUM_FAILED"), 0));
                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("SUM_FAILED"),
                    11));
                session.SendPacket(UserInterfaceHelper.GenerateGuri(19, 1, session.Character.CharacterId, 1332));
                session.Character.DeleteItemByItemInstanceId(itemToSum.Id);
                session.Character.DeleteItemByItemInstanceId(e.Id);
            }

            session.CurrentMapInstance?.Broadcast(UserInterfaceHelper.GenerateGuri(6, 1, session.Character.CharacterId),
                session.Character.MapX, session.Character.MapY);
            session.SendPacket("shop_end 1");
        }

        #endregion
    }
}