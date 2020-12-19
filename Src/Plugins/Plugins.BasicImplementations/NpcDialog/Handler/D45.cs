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
    public class D45 : INpcDialogAsyncHandler
    {
        public long HandledId => 45;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
           var npc = packet.Npc;
           var tp = npc?.Teleporters?.FirstOrDefault(s => s.Index == packet.Type);
           if (tp != null)
           {
               if (Session.Character.Gold >= 500)
               {
                   Session.Character.Gold -= 500;
                   Session.SendPacket(Session.Character.GenerateGold());
                   ServerManager.Instance.ChangeMap(Session.Character.CharacterId, tp.MapId, tp.MapX, tp.MapY);
               }
               else
               {
                   Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
               }
           }
        }
    }
}