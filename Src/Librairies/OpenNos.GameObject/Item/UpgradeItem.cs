using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;

namespace OpenNos.GameObject
{
    public class UpgradeItem : Item
    {
        #region Instantiation

        public UpgradeItem(ItemDTO item) : base(item)
        {
        }

        #endregion

        #region Methods

        public override void Use(ClientSession session, ref ItemInstance inv, byte Option = 0,
            string[] packetsplit = null)
        {
            if (session.Character.IsVehicled)
            {
                session.SendPacket(
                    session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_DO_VEHICLED"), 10));
                return;
            }

            if (session.CurrentMapInstance.MapInstanceType == MapInstanceType.RainbowBattleInstance)
            {
                return;
            }

            if (session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance) return;

            if (Effect == 0)
            {
                if (EffectValue != 0)
                {
                    if (session.Character.IsSitting)
                    {
                        session.Character.IsSitting = false;
                        session.SendPacket(session.Character.GenerateRest());
                    }

                    session.SendPacket(UserInterfaceHelper.GenerateGuri(12, 1, session.Character.CharacterId,
                        EffectValue));
                }
                else if (EffectValue == 0)
                {
                    if (packetsplit?.Length > 9 && byte.TryParse(packetsplit[8], out var TypeEquip) &&
                        short.TryParse(packetsplit[9], out var SlotEquip))
                    {
                        if (session.Character.IsSitting)
                        {
                            session.Character.IsSitting = false;
                            session.SendPacket(session.Character.GenerateRest());
                        }

                        if (Option != 0)
                        {
                            var isUsed = false;
                            switch (inv.ItemVNum)
                            {
                                case 1219:
                                case 9130:
                                    var equip = session.Character.Inventory.LoadBySlotAndType(SlotEquip,
                                        (InventoryType) TypeEquip);
                                    if (equip?.IsFixed == true)
                                    {
                                        equip.IsFixed = false;
                                        session.SendPacket(StaticPacketHelper.GenerateEff(UserType.Player,
                                            session.Character.CharacterId, 3003));
                                        session.SendPacket(UserInterfaceHelper.GenerateGuri(17, 1,
                                            session.Character.CharacterId, SlotEquip));
                                        session.SendPacket(
                                            session.Character.GenerateSay(
                                                Language.Instance.GetMessageFromKey("ITEM_UNFIXED"), 12));
                                        isUsed = true;
                                    }

                                    break;

                                case 1365:
                                case 9039:
                                    var specialist =
                                        session.Character.Inventory.LoadBySlotAndType(SlotEquip,
                                            (InventoryType) TypeEquip);
                                    if (specialist?.Rare == -2)
                                    {
                                        specialist.Rare = 0;
                                        session.SendPacket(
                                            UserInterfaceHelper.GenerateMsg(
                                                Language.Instance.GetMessageFromKey("SP_RESURRECTED"), 0));
                                        session.SendPacket(UserInterfaceHelper.GenerateGuri(13, 1,
                                            session.Character.CharacterId, 1));
                                        session.Character.SpPoint = 10000;
                                        if (session.Character.SpPoint > 10000) session.Character.SpPoint = 10000;
                                        session.SendPacket(session.Character.GenerateSpPoint());
                                        session.SendPacket(specialist.GenerateInventoryAdd());
                                        isUsed = true;
                                    }

                                    break;

                                case 5811:
                                    var weapon =
                                        session.Character.Inventory.LoadBySlotAndType(SlotEquip,
                                            (InventoryType)TypeEquip);
                                    if(weapon != null && weapon.IsBreaked)
                                    {
                                        weapon.IsBreaked = false;
                                        session.SendPacket(
                                            UserInterfaceHelper.GenerateMsg($"The {weapon.Item.Name}'s runes have been fixed.", 0));
                                        session.SendPacket(weapon.GenerateInventoryAdd());
                                        session.SendPacket(session.Character.GenerateEq());
                                        session.SendPacket(session.Character.GenerateEquipment());
                                    }
                                    break;
                            }

                            if (!isUsed)
                                session.SendPacket(
                                    session.Character.GenerateSay(
                                        Language.Instance.GetMessageFromKey("ITEM_IS_NOT_FIXED"), 11));
                            else
                                session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        }
                        else
                        {
                            session.SendPacket(
                                $"qna #u_i^1^{session.Character.CharacterId}^{(byte) inv.Type}^{inv.Slot}^0^1^{TypeEquip}^{SlotEquip} {Language.Instance.GetMessageFromKey("QNA_ITEM")}");
                        }
                    }
                    else
                    {
                        switch (inv.ItemVNum)
                        {
                            case 5813: // Rune Premium
                                session.SendPacket(
                                    UserInterfaceHelper.GenerateGuri(12, 1, session.Character.CharacterId, 89));
                                break;

                            case 5823: // Rune basic
                                session.SendPacket(
                                    UserInterfaceHelper.GenerateGuri(12, 1, session.Character.CharacterId, 93));
                                break;

                            case 5815: // Tattoo
                                session.SendPacket(
                                    UserInterfaceHelper.GenerateGuri(12, 1, session.Character.CharacterId, 90));
                                break;

                            case 5874:
                                session.SendPacket(
                                    UserInterfaceHelper.GenerateGuri(12, 1, session.Character.CharacterId, 79));
                                break;

                            case 5875:
                                session.SendPacket(
                                    UserInterfaceHelper.GenerateGuri(12, 1, session.Character.CharacterId, 75));
                                break;

                            case 5876:
                                session.SendPacket(
                                    UserInterfaceHelper.GenerateGuri(12, 1, session.Character.CharacterId, 76));
                                break;

                            case 5877:
                                session.SendPacket(
                                    UserInterfaceHelper.GenerateGuri(12, 1, session.Character.CharacterId, 77));
                                break;
                        }
                    }
                }
            }
            else
            {
                Logger.Warn(string.Format(Language.Instance.GetMessageFromKey("NO_HANDLER_ITEM"), GetType(), VNum,
                    Effect, EffectValue));
            }
        }

        #endregion
    }
}