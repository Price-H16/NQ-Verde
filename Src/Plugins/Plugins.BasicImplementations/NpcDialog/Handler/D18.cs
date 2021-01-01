using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D18 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 18;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (Session.Character.MapInstance.MapInstanceType != MapInstanceType.BaseMapInstance)
            {
                return;
            }
            Session.SendPacket(Session.Character.GenerateNpcDialog(17));
        }

        #endregion
    }
}