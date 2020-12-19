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
    public class D23 : INpcDialogAsyncHandler
    {
        public long HandledId => 23;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
           var npc = packet.Npc;
           if (packet.Type == 0)
           {
               if (Session.Character.Group?.SessionCount == 3)
               {
                   foreach (var s in Session.Character.Group.Sessions.GetAllItems())
                   {
                       if (s.Character.Family != null)
                       {
                           Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("GROUP_MEMBER_ALREADY_IN_FAMILY")));
                           return;
                       }
                   }
               }
               if (Session.Character.Group == null || Session.Character.Group.SessionCount != 3)
               {
                   Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("FAMILY_GROUP_NOT_FULL")));
                   return;
               }
               Session.SendPacket(UserInterfaceHelper.GenerateInbox($"#glmk^ {14} 1 {Language.Instance.GetMessageFromKey("CREATE_FAMILY").Replace(' ', '^')}"));
           }
           else
           {
               if (Session.Character.Family == null)
               {
                   Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("NOT_IN_FAMILY")));
                   return;
               }
               if (Session.Character.Family != null && Session.Character.FamilyCharacter != null && Session.Character.FamilyCharacter.Authority != FamilyAuthority.Head)
               {
                   Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("NOT_FAMILY_HEAD")));
                   return;
               }
               Session.SendPacket($"qna #glrm^1 {Language.Instance.GetMessageFromKey("DISSOLVE_FAMILY")}");
           }

        }
    }
}