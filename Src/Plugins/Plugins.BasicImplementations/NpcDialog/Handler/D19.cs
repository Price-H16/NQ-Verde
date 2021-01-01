using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D19 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 19;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (Session.Character.Timespace != null)
            {
                if (Session.Character.MapInstance.InstanceBag.EndState == 10)
                {
                    EventHelper.Instance.RunEvent(new EventContainer(Session.Character.MapInstance, EventActionType.SCRIPTEND, (byte)5));
                }
            }
        }

        #endregion
    }
}