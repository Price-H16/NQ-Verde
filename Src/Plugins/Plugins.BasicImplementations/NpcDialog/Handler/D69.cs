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
    public class D69 : INpcDialogAsyncHandler
    {
        public long HandledId => 69;

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
               if (Session.Character.Inventory.CountItem(5910) < 5)
               {
                   Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                   return;
               }
               Session.Character.GiftAdd(5929, 10);
               Session.Character.Inventory.RemoveItemAmount(5910, 5);
           }
        }
    }
}