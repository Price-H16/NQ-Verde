using System.Linq;
using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D111 : INpcDialogAsyncHandler
    {
        public long HandledId => 111;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc == null)
            {
                return;
            }
            if (packet.Type == 0)
            {
                Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
            }
            else
            {
                if (Session.Character.Inventory.CountItem(1012) < 5 || Session.Character.Inventory.CountItem(1013) < 5 || Session.Character.Inventory.CountItem(1027) < 5)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                    return;
                }
                Session.Character.GiftAdd(5500, 1);
                Session.Character.Inventory.RemoveItemAmount(1012, 5);
                Session.Character.Inventory.RemoveItemAmount(1013, 5);
                Session.Character.Inventory.RemoveItemAmount(1027, 5);
            }
        }
    }

    public class D113 : INpcDialogAsyncHandler
    {
        public long HandledId => 113; //teleport Fire Heroes

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 216, 28, 15);
            }
        }
    }

    public class D110 : INpcDialogAsyncHandler
    {
        public long HandledId => 110; //Fire Heroes start quest

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;   
            if (npc != null)
            {
                Session.Character.AddQuest(5954, false);
            }
        }
    }
}
