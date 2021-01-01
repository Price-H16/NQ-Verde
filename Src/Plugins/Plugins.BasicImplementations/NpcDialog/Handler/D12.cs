using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D12 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 12;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            Session.SendPacket($"wopen {packet.Type} 0");
        }

        #endregion
    }
}