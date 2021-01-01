using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D312 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 312;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                Session.Character.AddQuest(3239, false); // kill leeches
            }
        }

        #endregion
    }
}