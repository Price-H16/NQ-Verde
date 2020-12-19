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
    public class D311 : INpcDialogAsyncHandler
    {
        public long HandledId => 311;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null) //npc dialog = 528
            {
                Session.Character.AddQuest(7600, false); // A7 QUEST
            }
        }
    }
}