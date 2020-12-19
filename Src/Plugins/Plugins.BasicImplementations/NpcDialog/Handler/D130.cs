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
    public class D130 : INpcDialogAsyncHandler
    {
        public long HandledId => 130;

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
               if (Session.Character.Inventory.CountItem(1615) < 10 || Session.Character.Inventory.CountItem(1616) < 20 || Session.Character.Inventory.CountItem(1617) < 10
                   || Session.Character.Inventory.CountItem(1618) < 10 || Session.Character.Inventory.CountItem(1619) < 10 || Session.Character.Inventory.CountItem(1620) < 10)
               {
                   Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_INGREDIENTS"), 0));
                   return;
               }
               Session.Character.GiftAdd(1622, 10);
               Session.Character.Inventory.RemoveItemAmount(1615, 10);
               Session.Character.Inventory.RemoveItemAmount(1616, 20);
               Session.Character.Inventory.RemoveItemAmount(1617, 10);
               Session.Character.Inventory.RemoveItemAmount(1618, 10);
               Session.Character.Inventory.RemoveItemAmount(1619, 10);
               Session.Character.Inventory.RemoveItemAmount(1620, 10);
           }
        }
    }
}