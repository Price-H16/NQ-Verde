using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System.Linq;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D131 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 131;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet) //ICE COLD main quest
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                Session.Character.AddQuest(5982, false);
            }
        }

        #endregion
    }

    public class D132 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 132;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet) //glacerus the ice cold
        {
            var npc = packet.Npc;
            var tp = npc?.Teleporters?.FirstOrDefault(s => s.Index == packet.Type);
            if (tp != null)
            {
                ServerManager.Instance.ChangeMap(Session.Character.CharacterId, tp.MapId, tp.MapX, tp.MapY);
            }
        }

        #endregion
    }

    public class D133 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 133;

        #endregion

        #region Methods

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
                if (Session.Character.Inventory.CountItem(1012) < 5 || Session.Character.Inventory.CountItem(5911) < 5 || Session.Character.Inventory.CountItem(2307) < 5)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                    return;
                }
                Session.Character.GiftAdd(5512, 1);
                Session.Character.Inventory.RemoveItemAmount(1012, 5);
                Session.Character.Inventory.RemoveItemAmount(5911, 5);
                Session.Character.Inventory.RemoveItemAmount(2307, 5);
            }
        }

        #endregion
    }
}