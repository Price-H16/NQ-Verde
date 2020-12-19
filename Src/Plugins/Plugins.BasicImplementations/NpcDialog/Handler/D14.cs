using System.Collections.Generic;
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
    public class D14 : INpcDialogAsyncHandler
    {
        public long HandledId => 14;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
           var npc = packet.Npc;
           Session.SendPacket("wopen 27 0");
           var recipelist = "m_list 2";
           if (npc != null)
           {
               var tps = npc.Recipes;
               recipelist = tps.Where(s => s.Amount > 0).Aggregate(recipelist, (current, s) => current + $" {s.ItemVNum}");
               recipelist += " -100";
               Session.SendPacket(recipelist);
           }
        }
    }
}