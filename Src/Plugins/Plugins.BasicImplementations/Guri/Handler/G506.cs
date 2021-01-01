using OpenNos.GameObject;
using OpenNos.GameObject._Guri;
using OpenNos.GameObject._Guri.Event;
using OpenNos.GameObject.Networking;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.Guri.Handler
{
    public class G506 : IGuriHandler
    {
        #region Properties

        public long GuriEffectId => 506;

        #endregion

        #region Methods

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            if (e.Type == 506)
            {
                Session.Character.IsWaitingForEvent |= ServerManager.Instance.EventInWaiting;
            }
        }

        #endregion
    }
}