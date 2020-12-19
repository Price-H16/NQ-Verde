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
    public class D18 : INpcDialogAsyncHandler
    {
        public long HandledId => 18;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
           var npc = packet.Npc;
           if (Session.Character.MapInstance.MapInstanceType != MapInstanceType.BaseMapInstance)
           {
               return;
           }
           Session.SendPacket(Session.Character.GenerateNpcDialog(17));
        }
    }
}