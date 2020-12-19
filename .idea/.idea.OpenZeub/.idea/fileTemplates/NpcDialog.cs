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
    public class $NAME : INpcDialogAsyncHandler
    {
        public long HandledId => ;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
           var npc = packet.Npc;
        }
    }
}