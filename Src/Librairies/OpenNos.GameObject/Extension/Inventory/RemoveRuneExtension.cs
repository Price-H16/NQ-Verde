using NosTale.Configuration;
using NosTale.Configuration.Utilities;
using OpenNos.DAL;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;

namespace OpenNos.GameObject.Extension.Inventory
{
    public static class RemoveRuneExtension
    {
        #region Methods

        public static void RemoveRune(this ItemInstance e, ClientSession s)
        {
            if (e.Item.EquipmentSlot != EquipmentType.MainWeapon && e.Item.EquipmentSlot != EquipmentType.SecondaryWeapon)
            {
                // Not Main weapon
                s.SendShopEnd();
                return;
            }

            var get = DependencyContainer.Instance.GetInstance<JsonItemConfiguration>().RRemove;

            if (s.Character.Inventory.CountItem(get.ItemVnum) < 1)
            {
                // Not Enough Item
                s.SendShopEnd();
                return;
            }

            e.RuneEffects.Clear();
            e.RuneAmount = 0;
            e.IsBreaked = false;
            DAOFactory.RuneEffectDAO.DeleteByEquipmentSerialId(e.EquipmentSerialId);
            var msg = $"The {e.Item.Name} runes have been removed";
            s.SendPacket(UserInterfaceHelper.GenerateMsg(msg, 0));
            s.SendPacket(UserInterfaceHelper.GenerateSay(msg, 11));
            s.Character.Inventory.RemoveItemAmount(get.ItemVnum);
            s.SendPacket(e.GenerateInventoryAdd());
            s.SendPacket(s.Character.GenerateEq());
            s.SendPacket(s.Character.GenerateEquipment());
            s.SendShopEnd();
        }

        #endregion
    }
}