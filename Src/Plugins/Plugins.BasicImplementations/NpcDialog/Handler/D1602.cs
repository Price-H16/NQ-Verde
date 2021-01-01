using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D1602 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 1602;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            //var npc = packet.Npc;
            //if (Session.Character.Family?.FamilyLevel >= 2 && Session.Character.Family.WarehouseSize < 21)
            //{
            //    if (Session.Character.FamilyCharacter.Authority == FamilyAuthority.Head)
            //    {
            //        if (500000 >= Session.Character.Gold)
            //        {
            //            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
            //            return;
            //        }
            //        Session.Character.Family.WarehouseSize = 21;
            //        Session.Character.Gold -= 500000;
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