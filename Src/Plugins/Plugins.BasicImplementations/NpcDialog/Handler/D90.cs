using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Networking;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D90 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 90;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null && ServerManager.Instance.Configuration.ChristmasEvent)
            {
                Session.Character.AddQuest(5936);
            }
        }

        #endregion
    }
}