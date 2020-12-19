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
    public class D105 : INpcDialogAsyncHandler
    {
        public long HandledId => 150;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
           var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Family == null)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NEED_FAMILY"), 0));
                    return;
                }

                if (Session.Character.Family?.LandOfDeath == null)
                {
                    Session.Character.Family.LandOfDeath = ServerManager.GenerateMapInstance(150, MapInstanceType.LodInstance, new InstanceBag());
                }

                if (Session.Character.Level >= 55)
                {
                    ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, Session.Character.Family.LandOfDeath.MapInstanceId, 153, 145);
                }
                else
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("LOD_REQUIERE_LVL"), 0));
                }

                if (Session.Character?.Family?.LandOfDeath != null)
                {
                    ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, Session.Character.Family.LandOfDeath.MapInstanceId, 153, 145);
                }            

            }
            
        }
    }
}