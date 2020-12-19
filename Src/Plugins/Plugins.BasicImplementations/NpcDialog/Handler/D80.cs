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
    public class D80 : INpcDialogAsyncHandler
    {
        public long HandledId => 80;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
           var npc = packet.Npc;
           //if (npc != null && ServerManager.Instance.Configuration.HalloweenEvent)
           //{
           //    Session.Character.AddQuest(5930);
           //}
        }
    }
}