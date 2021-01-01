using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D311 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 311;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null) //npc dialog = 528
            {
                Session.Character.AddQuest(7600, false); // A7 QUEST
            }
        }

        #endregion
    }
}