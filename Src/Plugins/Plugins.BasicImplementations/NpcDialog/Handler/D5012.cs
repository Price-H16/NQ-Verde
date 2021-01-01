using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Networking;
using System.Linq;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D5012 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 5012;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            var tp = npc?.Teleporters?.FirstOrDefault(s => s.Index == packet.Type);
            if (tp != null)
            {
                ServerManager.Instance.ChangeMap(Session.Character.CharacterId, tp.MapId, tp.MapX, tp.MapY);
            }
        }

        #endregion
    }
}