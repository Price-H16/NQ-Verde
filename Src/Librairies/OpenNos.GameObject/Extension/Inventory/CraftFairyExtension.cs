using System.Linq;
using NosTale.Configuration;
using NosTale.Configuration.Utilities;
using OpenNos.Core;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject.Extension.Inventory
{
    public static class CraftFairyExtension
    {
        #region Methods

        public static void CraftFairy(this ItemInstance e, ClientSession s, byte value)
        {
            var get = DependencyContainer.Instance.GetInstance<JsonItemConfiguration>().Fairy;

            var items = get.Item[value];

            if (!get.FairyVnum.Any(f => f == e.ItemVNum))
                // Bad Fairy
                return;

            foreach (var item in items)
                if (s.Character.Inventory.CountItem(item.Id) < item.Quantity)
                {
                    // Not Enough Item
                    s.SendShopEnd();
                    return;
                }

            if (s.Character.Gold < get.GoldPrice[value])
            {
                s.SendShopEnd();
                return;
            }

            var rnd = ServerManager.RandomNumber();
            var saySucess = false;
            if (rnd < get.PercentSucess[value])
            {
                var newItem = e;
                newItem.ItemVNum = (short) (get.SuccesVnumFairy[value] + (newItem.Item.Element - 1));
                s.Character.Inventory.RemoveItemFromInventory(e.Id);
                s.Character.GiftAdd((short) (get.SuccesVnumFairy[value] + newItem.Item.Element - 1), 1);
                saySucess = true;
                s.SendPacket($"pdti 11 {newItem.ItemVNum} 1 29 0 0");
                s.SendPacket(UserInterfaceHelper.GenerateGuri(19, 1, s.Character.CharacterId, 1324));
                s.SendPacket(UserInterfaceHelper.GenerateMsg(
                    string.Format(Language.Instance.GetMessageFromKey("CRAFTED_OBJECT"), newItem.Item.Name, 1), 0));
            }

            s.SendPacket(s.Character.GenerateSay(saySucess ? "Succes" : "Failed", 10));
            foreach (var item in items) s.Character.Inventory.RemoveItemAmount(item.Id, item.Quantity);
            s.GoldLess(get.GoldPrice[value]);
            s.SendShopEnd();
        }

        #endregion
    }
}