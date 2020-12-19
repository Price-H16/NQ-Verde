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
    public class D532 : INpcDialogAsyncHandler
    {
        public long HandledId => 532;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
           var npc = packet.Npc;
           if (npc != null)
           {
               if (Session.Character.Level >= 99) //  REQUIRED LEVEL
               {
                   ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 260, 57, 348);
               }
               else
               {
                   Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("LEVEL_REQUIRED"), 0));
               }
           }
        }
    }
}