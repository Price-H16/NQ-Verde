using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D325 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 325;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc == null || !ServerManager.Instance.Configuration.ChristmasEvent)
            {
                return;
            }

            if (packet.Type == 0)
            {
                Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
            }
            else
            {
                if (Session.Character.Inventory.CountItem(5712) < 1 && Session.Character.Inventory.CountItem(9138) < 1 || Session.Character.Inventory.CountItem(4406) < 1 && Session.Character.Inventory.CountItem(8369) < 1)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                    return;
                }

                if (Session.Character.Inventory.CountItem(9138) > 0)
                {
                    Session.Character.Inventory.RemoveItemAmount(9138, 1);
                    Session.Character.GiftAdd(9140, 1);
                }
                else
                {
                    Session.Character.Inventory.RemoveItemAmount(5712, 1);
                    Session.Character.GiftAdd(5714, 1);
                }

                if (Session.Character.Inventory.CountItem(8369) > 0)
                {
                    Session.Character.Inventory.RemoveItemAmount(8369, 1);
                }
                else
                {
                    Session.Character.Inventory.RemoveItemAmount(4406, 1);
                }
            }
        }

        #endregion
    }
}