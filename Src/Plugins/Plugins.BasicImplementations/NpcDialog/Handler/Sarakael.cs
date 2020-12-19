using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class Sarakael : INpcDialogAsyncHandler
    {
        public long HandledId => 196;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc == null)
            {
                return;
            }
            //if (packet.Type == 0)
            //{
            //    Session.SendPacket($"qna #n_run^{packet.Runner}^56^{packet.Value}^{packet.NpcId} {Language.Instance.GetMessageFromKey("ASK_TRADE")}");
            //}
            if (Session.Character.Inventory.CountItem(5987) >= 1)
            {
                Session.Character.GiftAdd(5977, 1);
                Session.Character.Inventory.RemoveItemAmount(5987, 1);
               
            }       
            else
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                return;
            }
        }
    }
}
