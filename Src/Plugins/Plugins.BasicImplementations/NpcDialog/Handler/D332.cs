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
    public class D332 : INpcDialogAsyncHandler
    {
        public long HandledId => 332;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
           var npc = packet.Npc;
           if (npc == null)
           {
               return;
           }

           if (Session.Character.HeroLevel < 50)
           {
               Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_DO_THAT"), 10));
               return;
           }

           Session.Character.AddQuest(6500);
        }
    }
}