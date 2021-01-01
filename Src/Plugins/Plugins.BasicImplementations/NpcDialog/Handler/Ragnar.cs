using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class Ragnar : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 309;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var chara = ServerManager.Instance.GetSessionByCharacterId(packet.Sender.Character.CharacterId);
            if (Session.Character.Gold >= 10000000) //  Pay off 10.000.000 gold (?)
            {
                Session.Character.Gold -= 10000000;
                Session.SendPacket(Session.Character.GenerateGold());
                Session.Character.AddQuest(6147, false); // continue
            }
            else
            {
                Session.Character.AddQuest(6152, false); // take normal questline order
                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_GOLD"), 0));
            }
        }

        #endregion
    }

    public class Ragnar2 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 310;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            Session.Character.AddQuest(6152, false); // take normal questline order
        }

        #endregion
    }
}