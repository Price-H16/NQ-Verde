using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.GameObject.Helpers;

namespace OpenNos.GameObject.Extension.Inventory
{
    public static class ItemDownGradeRarityExtension
    {
        #region Methods

        public static void DownGradeRarity(this ItemInstance wearableInstance, ClientSession Session)
        {
            if (wearableInstance == null)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_A_GOOD_STUFF"), 10));
                return;
            }

            if (!wearableInstance.Item.IsHeroic)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_CHAMPION"), 10));
                return;
            }

            if (wearableInstance.Rare != 8)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ITEM_NOT_MAX_RARE"), 10));
                return;
            }

            var goldless = 200000;

            if (Session.Character.Gold < goldless)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_HAVE_GOLD"), 10));
                return;
            }

            Session.GoldLess(goldless);
            wearableInstance.Rare = 0;
            wearableInstance.SetRarityPoint();
            wearableInstance.ShellEffects.Clear();
            DAOFactory.ShellEffectDAO.DeleteByEquipmentSerialId(wearableInstance.EquipmentSerialId);
            Session.SendPacket(wearableInstance.GenerateInventoryAdd());
            Session.SendPacket(Session.Character.GenerateEq());
            Session.SendPacket(Session.Character.GenerateEquipment());
            Session.SendPacket($"pdti 14 {wearableInstance.ItemVNum} 2 {wearableInstance.Slot} {wearableInstance.Upgrade} {wearableInstance.Rare}");

        }

        #endregion
    }
}