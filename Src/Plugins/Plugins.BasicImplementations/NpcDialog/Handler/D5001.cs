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
    public class D5001 : INpcDialogAsyncHandler
    {
        public long HandledId => 5001;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
           var npc = packet.Npc;
                               
           if (npc != null)
           {
               MapInstance map = null;
               switch (Session.Character.Faction)
               {
                   case FactionType.None:
                       Session.SendPacket(UserInterfaceHelper.GenerateInfo("You need to be part of a faction to join Act 4!"));
                       return;

                   case FactionType.Angel:
                       map = ServerManager.GetAllMapInstances().Find(s => s.MapInstanceType.Equals(MapInstanceType.Act4ShipAngel));

                       break;

                   case FactionType.Demon:
                       map = ServerManager.GetAllMapInstances().Find(s => s.MapInstanceType.Equals(MapInstanceType.Act4ShipDemon));

                       break;
               }
               if (map == null || npc.EffectActivated)
               {
                   Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SHIP_NOTARRIVED"), 0));
                   return;
               }
               if (3000 > Session.Character.Gold)
               {
                   Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                   return;
               }
               Session.Character.Save();
               Session.Character.Gold -= 3000;
               Session.SendPacket(Session.Character.GenerateGold());
               var pos = map.Map.GetRandomPosition();
               ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, map.MapInstanceId, pos.X, pos.Y);
           }
        }
    }
}