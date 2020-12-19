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
    public class D5 : INpcDialogAsyncHandler
    {
        public long HandledId => 5;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
           var npc = packet.Npc;
           if (packet.Type == 0 && packet.Value == 1)
           {
               if (Session.Character.MapInstance.Npcs.Any(s => s.NpcVNum == 948 /* SP 3 */ || s.NpcVNum == 954 /* SP 4 */))
               {
                   /**
                    * TODO:
                    *
                    * SP 3
                    *
                    * switch (Session.Character.Class)
                    * {
                    *     case ClassType.Swordsman:
                    *         Session.Character.GiftAdd(909, 1);
                    *         break;
                    *     case ClassType.Archer:
                    *         Session.Character.GiftAdd(911, 1);
                    *         break;
                    *     case ClassType.Magician:
                    *         Session.Character.GiftAdd(913, 1);
                    *         break;
                    * }
                    *
                    * SP 4
                    *
                    * switch (Session.Character.Class)
                    * {
                    *     case ClassType.Swordsman:
                    *         Session.Character.GiftAdd(910, 1);
                    *         break;
                    *     case ClassType.Archer:
                    *         Session.Character.GiftAdd(912, 1);
                    *         break;
                    *     case ClassType.Magician:
                    *         Session.Character.GiftAdd(914, 1);
                    *         break;
                    * }
                    *
                    */

                   return;
               }
           }
        }
    }
}