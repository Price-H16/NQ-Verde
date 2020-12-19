using System;
using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D336 : INpcDialogAsyncHandler
    {
        public long HandledId => 336;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
           var npc = packet.Npc;
           if (npc == null)
           {
               return;
           }

           short goldPrice = 0;
           var mapTuple = new Tuple<short, short, short>(0, 0, 0);
           switch (packet.Type)
           {
               case 170:
                   goldPrice = 25000;
                   mapTuple = new Tuple<short, short, short>(145, 50, 44);
                   break;

               case 145:
                   goldPrice = 30000;
                   mapTuple = new Tuple<short, short, short>(170, 125, 68);
                   break;

               default:
                   return;
           }

           if (Session.Character.Gold < goldPrice)
           {
               Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
               return;
           }

           Session.GoldLess(goldPrice);
           ServerManager.Instance.ChangeMap(Session.Character.CharacterId, mapTuple.Item1, mapTuple.Item2, mapTuple.Item3);
        }
    }
}