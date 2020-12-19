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
    public class D2002 : INpcDialogAsyncHandler
    {
        public long HandledId => 2002;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
           var npc = packet.Npc;
           if (npc != null)
           {
               var gemNpcVnum = 0;

                switch (npc.NpcVNum)
                {
                    case 935:
                        gemNpcVnum = 932;
                        break;

                    case 936:
                        gemNpcVnum = 933;
                        break;

                    case 937:
                        gemNpcVnum = 934;
                        break;

                    case 952:
                        gemNpcVnum = 948;
                        break;

                    case 953:
                        gemNpcVnum = 954;
                        break;
                    default:
                        gemNpcVnum = npc.NpcVNum;
                        break;
                }

               
               if (ServerManager.Instance.SpecialistGemMapInstances?.FirstOrDefault(s => s.Npcs.Any(n => n.NpcVNum == gemNpcVnum)) is MapInstance specialistGemMapInstance)
               {
                   ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, specialistGemMapInstance.MapInstanceId, 3, 3);
               }
           }
        }
    }
}