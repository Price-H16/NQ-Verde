using System.Linq;
using NosTale.Configuration;
using NosTale.Configuration.Utilities;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class CreateItemHandler : IPacketHandler
    {
        #region Instantiation

        public CreateItemHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void CreateItem(CreateItemPacket createItemPacket)
        {
            if (createItemPacket != null)
            {
                Session.AddLogsCmd(createItemPacket);
                var vnum = createItemPacket.VNum;
                sbyte rare = 0;
                byte upgrade = 0, design = 0;
                short amount = 1;

                if (vnum == 1046)
                {
                    return; // cannot create gold as item, use $Gold instead
                }

                var iteminfo = ServerManager.GetItem(vnum);
                if (iteminfo != null)
                {
                    if (iteminfo.IsColored || iteminfo.ItemType == ItemType.Box && iteminfo.ItemSubType == 3)
                    {
                        if (createItemPacket.Design.HasValue)
                        {
                            rare = (sbyte) ServerManager.RandomNumber();
                            if (rare > 90)
                            {
                                rare = 7;
                            }
                            else if (rare > 80)
                            {
                                rare = 6;
                            }
                            else
                            {
                                rare = (sbyte) ServerManager.RandomNumber(1, 6);
                            }

                            design = createItemPacket.Design.Value;
                        }
                    }
                    else if (iteminfo.Type == 0)
                    {
                        if (createItemPacket.Upgrade.HasValue)
                        {
                            if (iteminfo.EquipmentSlot != EquipmentType.Sp)
                            {
                                upgrade = createItemPacket.Upgrade.Value;
                            }
                            else
                            {
                                design = createItemPacket.Upgrade.Value;
                            }

                            if (iteminfo.EquipmentSlot != EquipmentType.Sp
                             && upgrade == 0
                             && iteminfo.BasicUpgrade != 0)
                            {
                                upgrade = iteminfo.BasicUpgrade;
                            }
                        }

                        if (createItemPacket.Design.HasValue)
                        {
                            if (iteminfo.EquipmentSlot == EquipmentType.Sp)
                            {
                                upgrade = createItemPacket.Design.Value;
                                amount = 1;
                            }
                            else
                            {
                                rare = (sbyte) createItemPacket.Design.Value;
                            }
                        }
                    }

                    if (createItemPacket.Amount.HasValue && !createItemPacket.Upgrade.HasValue)
                    {
                        amount =
                            createItemPacket.Amount.Value > DependencyContainer.Instance.GetInstance<JsonItemConfiguration>().Inventory
                                .MaxItemPerSlot
                                ? DependencyContainer.Instance.GetInstance<JsonItemConfiguration>().Inventory.MaxItemPerSlot
                                : createItemPacket.Amount.Value;
                        amount = createItemPacket.Amount.Value > 999 ? (short) 999 : createItemPacket.Amount.Value;
                    }

                    var inv = Session.Character.Inventory
                        .AddNewToInventory(vnum, amount, Rare: rare, Upgrade: upgrade, Design: design).FirstOrDefault();
                    if (inv != null)
                    {
                        var wearable = Session.Character.Inventory.LoadBySlotAndType(inv.Slot, inv.Type);
                        if (wearable != null)
                        {
                            switch (wearable.Item.EquipmentSlot)
                            {
                                case EquipmentType.Armor:
                                case EquipmentType.MainWeapon:
                                case EquipmentType.SecondaryWeapon:
                                    wearable.SetRarityPoint();
                                    if (wearable.Item.IsHeroic)
                                    {
                                        wearable.ShellEffects.Clear();
                                        DAOFactory.ShellEffectDAO.DeleteByEquipmentSerialId(wearable.EquipmentSerialId);
                                        wearable.GenerateHeroicShell(RarifyProtection.RandomHeroicAmulet);
                                    }

                                    break;

                                case EquipmentType.Boots:
                                case EquipmentType.Gloves:
                                    wearable.FireResistance  = (short) (wearable.Item.FireResistance  * upgrade);
                                    wearable.DarkResistance  = (short) (wearable.Item.DarkResistance  * upgrade);
                                    wearable.LightResistance = (short) (wearable.Item.LightResistance * upgrade);
                                    wearable.WaterResistance = (short) (wearable.Item.WaterResistance * upgrade);
                                    break;
                            }
                        }

                        Session.SendPacket(Session.Character.GenerateSay(
                                                           $"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {iteminfo.Name} x {amount}", 12));
                    }
                    else
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"),
                                0));
                    }
                }
                else
                {
                    UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_ITEM"), 0);
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(CreateItemPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}