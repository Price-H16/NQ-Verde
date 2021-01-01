using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D1600 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 1600;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            //var npc = packet.Npc;
            // if (!Session.Character.VerifiedLock)
            // {
            //     Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ACTION_NOT_POSSIBLE_USE_UNLOCK"), 0));
            //     return;
            // }

            // if (Session.Account.IsLimited)
            //{
            //    Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("LIMITED_ACCOUNT")));
            //    return;
            //}

            //Session.SendPacket(Session.Character.OpenFamilyWarehouse());
        }

        #endregion
    }
}