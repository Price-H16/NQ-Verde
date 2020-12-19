using System.Linq;
using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D1605 : INpcDialogAsyncHandler
    {
        public long HandledId => 1605;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
           //var npc = packet.Npc;
           //if (Session.Character.Family?.FamilyLevel >= 9 && Session.Character.Family.MaxSize < 100)
           //{
           //    if (Session.Character.FamilyCharacter.Authority == FamilyAuthority.Head)
           //    {
           //        if (10000000 >= Session.Character.Gold)
           //        {
           //            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
           //            return;
           //        }
           //        Session.Character.Family.MaxSize = 100;
           //        Session.Character.Gold -= 10000000;
           //        Session.SendPacket(Session.Character.GenerateGold());
           //        FamilyDTO fam = Session.Character.Family;
           //        DAOFactory.FamilyDAO.InsertOrUpdate(ref fam);
           //        ServerManager.Instance.FamilyRefresh(Session.Character.Family.FamilyId);
           //    }
           //    else
           //    {
           //        Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NO_FAMILY_HEAD"), 10));
           //        Session.SendPacket(UserInterfaceHelper.GenerateModal(Language.Instance.GetMessageFromKey("NO_FAMILY_HEAD"), 1));
           //    }
           //}
        }
    }
}