using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D1603 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 1603;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            //var npc = packet.Npc;
            //if (Session.Character.Family?.FamilyLevel >= 7 && Session.Character.Family.WarehouseSize < 49)
            //{
            //    if (Session.Character.FamilyCharacter.Authority == FamilyAuthority.Head)
            //    {
            //        if (2000000 >= Session.Character.Gold)
            //        {
            //            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
            //            return;
            //        }
            //        Session.Character.Family.WarehouseSize = 49;
            //        Session.Character.Gold -= 2000000;
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

        #endregion
    }
}