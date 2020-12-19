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
    public class D1601 : INpcDialogAsyncHandler
    {
        public long HandledId => 1601;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            //var npc = packet.Npc;
            //if (npc != null && Session.Character.Family != null)
            //{
            //    Session.SendPackets(Session.Character.OpenFamilyWarehouseHist());
            //}
            //else
            //{
            //    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NO_FAMILY_FOUND"), 10)); 
            //}
               
        }
    }
}