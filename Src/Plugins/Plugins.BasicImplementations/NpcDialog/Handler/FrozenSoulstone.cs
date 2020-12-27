using System.Linq;
using System.Threading.Tasks;
using ChickenAPI.Enums.Game.Character;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

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

            if (Session.Character.Class == CharacterClassType.Swordsman && Session.Character.Inventory.CountItem(4494) >= 1)
            {
                Session.Character.Inventory.RemoveItemAmount(4494, 1);

            }

            else if (Session.Character.Class == CharacterClassType.Archer && Session.Character.Inventory.CountItem(4495) >= 1)
            {
                Session.Character.Inventory.RemoveItemAmount(4495, 1);
            }

            else if (Session.Character.Class == CharacterClassType.Magician && Session.Character.Inventory.CountItem(4496) >= 1)
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