﻿using System.Linq;
using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D5011 : INpcDialogAsyncHandler
    {
        public long HandledId => 5011;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
           var npc = packet.Npc;
           if (npc != null)
           {
               if (30000 > Session.Character.Gold)
               {
                   Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                   return;
               }
               Session.Character.Gold -= 30000;
               Session.SendPacket(Session.Character.GenerateGold());
               ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 170, 127, 46);
           }
        }
    }
}