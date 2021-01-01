using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._Guri;
using OpenNos.GameObject._Guri.Event;
using OpenNos.GameObject.Helpers;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.Guri.Handler
{
    public class G205 : IGuriHandler
    {
        #region Properties

        public long GuriEffectId => 205;

        #endregion

        #region Methods

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            if (e.Type == 205)
            {
                if (e.Argument == 0 && short.TryParse(e.User.ToString(), out var slot))
                {
                    const int perfumeVnum = 1428;

                    var perfumeInventoryType = (InventoryType)e.Argument;

                    var equipmentInstance = Session.Character.Inventory.LoadBySlotAndType(slot, perfumeInventoryType);

                    if (equipmentInstance?.BoundCharacterId == null || equipmentInstance.BoundCharacterId == Session.Character.CharacterId || equipmentInstance.Item.ItemType != ItemType.Weapon && equipmentInstance.Item.ItemType != ItemType.Armor)
                    {
                        return;
                    }

                    var perfumesNeeded = ShellGeneratorHelper.Instance.PerfumeFromItemLevelAndShellRarity(equipmentInstance.Item.LevelMinimum, (byte)equipmentInstance.Rare);

                    if (Session.Character.Inventory.CountItem(perfumeVnum) < perfumesNeeded)
                    {
                        return;
                    }

                    Session.Character.Inventory.RemoveItemAmount(perfumeVnum, perfumesNeeded);

                    equipmentInstance.BoundCharacterId = Session.Character.CharacterId;

                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("BOUND_TO_YOU"), 0));
                }
            }
        }

        #endregion
    }
}