using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D129 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 129;

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
                if (Session.Character.Inventory.CountItem(1611) < 10 || Session.Character.Inventory.CountItem(1612) < 10
                                                                     || Session.Character.Inventory.CountItem(1613) < 20 || Session.Character.Inventory.CountItem(1614) < 10)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                    return;
                }
                Session.Character.GiftAdd(1621, 10);
                Session.Character.Inventory.RemoveItemAmount(1611, 10);
                Session.Character.Inventory.RemoveItemAmount(1612, 10);
                Session.Character.Inventory.RemoveItemAmount(1613, 20);
                Session.Character.Inventory.RemoveItemAmount(1614, 10);
            }
        }

        #endregion
    }
}