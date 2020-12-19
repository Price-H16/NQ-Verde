using System;
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
    public class D135 : INpcDialogAsyncHandler
    {
        public long HandledId => 135;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
           var npc = packet.Npc;
            
           if (!ServerManager.Instance.StartedEvents.Contains(EventType.TALENTARENA))
           {
               var time = ServerManager.Instance.Schedules.ToList().FirstOrDefault(s => s.Event == EventType.TALENTARENA)?.Time ?? TimeSpan.FromSeconds(0);
               Session.SendPacket(npc?.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("ARENA_NOT_OPEN"), string.Format("{0:D2}:{1:D2} - {2:D2}:{3:D2}", time.Hours, time.Minutes, (time.Hours + 4) % 24, time.Minutes)), 10));
           }
           else
           {
               if (Session.Character.Level < 30)
               {
                   Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("LOW_LVL_30")));
                   return;
               }

               var tickets = 10 - Session.Character.GeneralLogs.CountLinq(s => s.LogType == "TalentArena" && s.Timestamp.Date == DateTime.Today);

               if (tickets > 0)
               {
                   if (ServerManager.Instance.ArenaMembers.ToList().All(s => s.Session != Session))
                   {
                       if (ServerManager.Instance.IsCharacterMemberOfGroup(Session.Character.CharacterId))
                       {
                           Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TALENT_ARENA_GROUP"), 0));
                           Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("TALENT_ARENA_GROUP"), 10));
                       }
                       else
                       {
                           Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("ARENA_TICKET_LEFT"), tickets), 10));

                           ServerManager.Instance.ArenaMembers.Add(new ArenaMember
                           {
                               ArenaType = EventType.TALENTARENA,
                               Session = Session,
                               GroupId = null,
                               Time = 0
                           });
                       }
                   }
               }
               else
               {
                   Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TALENT_ARENA_NO_MORE_TICKET"), 0));
                   Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("TALENT_ARENA_NO_MORE_TICKET"), 10));
               }
           }
        }
    }
}