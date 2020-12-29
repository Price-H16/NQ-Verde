using System.Linq;
using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class FrozenSoulstone : INpcDialogAsyncHandler
    {
        public long HandledId => 136;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc == null)
            {
                return;
            }

            if (Session.Character.Class == ClassType.Swordsman && Session.Character.Inventory.CountItem(4494) >= 1)
            {
                Session.Character.Inventory.RemoveItemAmount(4494, 1);

            }

            else if (Session.Character.Class == ClassType.Archer && Session.Character.Inventory.CountItem(4495) >= 1)
            {
                Session.Character.Inventory.RemoveItemAmount(4495, 1);
            }

            else if (Session.Character.Class == ClassType.Magician && Session.Character.Inventory.CountItem(4496) >= 1)
            {
                Session.Character.Inventory.RemoveItemAmount(4496, 1);
            }

            else
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                return;
            }
        }
    }
}