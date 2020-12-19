using System.Linq;
using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D19 : INpcDialogAsyncHandler
    {
        public long HandledId => 19;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
           var npc = packet.Npc;
           if (Session.Character.Timespace != null)
           {
               if (Session.Character.MapInstance.InstanceBag.EndState == 10)
               {
                   EventHelper.Instance.RunEvent(new EventContainer(Session.Character.MapInstance, EventActionType.SCRIPTEND, (byte)5));
               }
           }
        }
    }
}