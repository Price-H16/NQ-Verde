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
    public class D5004 : INpcDialogAsyncHandler
    {
        public long HandledId => 5004;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
           var npc = packet.Npc;
           if (npc != null)
           {
                Session.Character.Save();
                ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 145, 52, 41);
           }
        }
    }
}