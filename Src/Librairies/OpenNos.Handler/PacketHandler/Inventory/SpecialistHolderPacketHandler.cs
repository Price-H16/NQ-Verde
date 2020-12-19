using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Inventory
{
    public class SpecialistHolderPacketHandler : IPacketHandler
    {
        #region Instantiation

        public SpecialistHolderPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void SpecialistHolder(SpecialistHolderPacket specialistHolderPacket)
        {
            if (specialistHolderPacket == null) return;

            var specialist =
                Session.Character.Inventory.LoadBySlotAndType(specialistHolderPacket.Slot, InventoryType.Equipment);

            var holder =
                Session.Character.Inventory.LoadBySlotAndType(specialistHolderPacket.HolderSlot,
                    InventoryType.Equipment);

            if (specialist?.Item == null || holder?.Item == null) return;

            if (!holder.Item.IsHolder) return;

            if (holder.HoldingVNum > 0) return;

            switch (specialistHolderPacket.HolderType)
            {
                // SP Holder
                case 0:
                    TryToBoxSP(holder, specialist, Session);
                    return;
                // PSP Holder
                case 1:
                    TryToBoxPSP(holder, specialist, Session);
                    return;
            }
        }

        private static void TryToBoxSP(ItemInstance holder, ItemInstance e, ClientSession s)
        {
            if (holder.Item.ItemType != ItemType.Box && holder.Item.ItemSubType != 2) return;

            if (e.Item.ItemType != ItemType.Specialist || !e.Item.IsSoldable || e.Item.Class == 0) return;

            if (e.ItemVNum >= 4494 && e.ItemVNum <= 4496)
            {
                s.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("CANT_HOLD_SP")));
                s.SendPacket("shop_end 2");
                return;
            }

            s.Character.Inventory.RemoveItemFromInventory(e.Id);

            holder.HoldingVNum = e.ItemVNum;
            holder.SlDamage = e.SlDamage;
            holder.SlDefence = e.SlDefence;
            holder.SlElement = e.SlElement;
            holder.SlHP = e.SlHP;
            holder.SpDamage = e.SpDamage;
            holder.SpDark = e.SpDark;
            holder.SpDefence = e.SpDefence;
            holder.SpElement = e.SpElement;
            holder.SpFire = e.SpFire;
            holder.SpHP = e.SpHP;
            holder.SpLevel = e.SpLevel;
            holder.SpLight = e.SpLight;
            holder.SpStoneUpgrade = e.SpStoneUpgrade;
            holder.SpWater = e.SpWater;
            holder.Upgrade = e.Upgrade;
            holder.XP = e.XP;
            holder.EquipmentSerialId = e.EquipmentSerialId;

            s.SendPacket("shop_end 2");
        }

        private static void TryToBoxPSP(ItemInstance holder, ItemInstance e, ClientSession s)
        {
            if (holder.Item.ItemType != ItemType.Box && holder.Item.ItemSubType != 6) return;

            if (e.Item.ItemType != ItemType.Specialist || !e.Item.IsSoldable) return;

            s.Character.Inventory.RemoveItemFromInventory(e.Id);

            holder.HoldingVNum = e.ItemVNum;
            holder.XP = e.XP;
            holder.EquipmentSerialId = e.EquipmentSerialId;

            s.SendPacket("shop_end 2");
        }

        #endregion
    }
}