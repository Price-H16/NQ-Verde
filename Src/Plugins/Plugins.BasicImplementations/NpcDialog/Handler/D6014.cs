using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D6014 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 6014;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (!ServerManager.Instance.Configuration.ChristmasEvent)
            {
                return;
            }
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
                if (Session.Character.Inventory.CountItem(1615) < 1 || Session.Character.Inventory.CountItem(1616) < 2 || Session.Character.Inventory.CountItem(1617) < 1
                    || Session.Character.Inventory.CountItem(1618) < 1 || Session.Character.Inventory.CountItem(1619) < 1 || Session.Character.Inventory.CountItem(1620) < 1)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                    return;
                }
                Session.Character.GiftAdd(1622, 1);
                Session.Character.Inventory.RemoveItemAmount(1615);
                Session.Character.Inventory.RemoveItemAmount(1616, 2);
                Session.Character.Inventory.RemoveItemAmount(1617);
                Session.Character.Inventory.RemoveItemAmount(1618);
                Session.Character.Inventory.RemoveItemAmount(1619);
                Session.Character.Inventory.RemoveItemAmount(1620);
            }
        }

        #endregion
    }
}