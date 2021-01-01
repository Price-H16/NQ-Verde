using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D1 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 1;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (Session.Character.Class != (byte)ClassType.Adventurer)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ADVENTURER"), 0));
                return;
            }
            if (Session.Character.Level < 15 || Session.Character.JobLevel < 20)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("LOW_LVL"), 0));
                return;
            }
            if (packet.Type > 3 || packet.Type < 1)
            {
                return;
            }
            if (packet.Type == (byte)Session.Character.Class)
            {
                return;
            }
            if (Session.Character.Inventory.All(i => i.Type != InventoryType.Wear))
            {
                Session.Character.Inventory.AddNewToInventory((short)(4 + packet.Type * 14), 1, InventoryType.Wear, 4, 5);
                Session.Character.Inventory.AddNewToInventory((short)(81 + packet.Type * 13), 1, InventoryType.Wear, 4, 5);

                switch (packet.Type)
                {
                    case 1:
                        Session.Character.Inventory.AddNewToInventory(68, 1, InventoryType.Wear, 4, 5);
                        break;

                    case 2:
                        Session.Character.Inventory.AddNewToInventory(78, 1, InventoryType.Wear, 4, 5);
                        break;

                    case 3:
                        Session.Character.Inventory.AddNewToInventory(86, 1, InventoryType.Wear, 4, 5);
                        break;
                }

                Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateEq());
                Session.SendPacket(Session.Character.GenerateEquipment());
                Session.Character.ChangeClass((ClassType)packet.Type, false);

                if (Session.Character.Class == ClassType.Archer)
                {
                    Session.Character.Inventory.AddNewToInventory(2083, 20, InventoryType.Etc); // arrows
                }

                if (Session.Character.Class == ClassType.Swordsman)
                {
                    Session.Character.Inventory.AddNewToInventory(2082, 20, InventoryType.Etc); // bolts
                }
            }
            else
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("EQ_NOT_EMPTY"), 0));
            }
        }

        #endregion
    }
}