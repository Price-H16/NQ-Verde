using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Event.ACT7;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class A7Ship : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 334;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc == null)
            {
                return;
            }

            //if (!DAOFactory.QuestLogDAO.LoadByCharacterId(Session.Character.CharacterId).Any(s => s.QuestId == 6500))
            if (Session.Character.HeroLevel < 45)
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("A7_SHIP_REQUIREMENT"), 0));
                return;
            }

            if (Session.Character.Gold >= 50000)
            {
                Session.Character.Gold -= 50000;
                Session.Character.GenerateGold();
                ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, ServerManager.Instance.Act7Ship.MapInstanceId, 5, 32);
                Act7Ship.Run(Session);
            }
        }

        #endregion
    }
}