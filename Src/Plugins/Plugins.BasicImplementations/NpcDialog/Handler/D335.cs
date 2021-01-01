using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Networking;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D335 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 335;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 2644, 4, 56);
        }

        #endregion
    }
}