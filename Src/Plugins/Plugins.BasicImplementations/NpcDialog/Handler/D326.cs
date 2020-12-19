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
    public class D326 : INpcDialogAsyncHandler
    {
        public long HandledId => 326;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
           var npc = packet.Npc;
           //if (npc != null && ServerManager.Instance.Configuration.ChristmasEvent)
           //{
           //    Session.Character.AddQuest(6325);
           //}
        }
    }
}