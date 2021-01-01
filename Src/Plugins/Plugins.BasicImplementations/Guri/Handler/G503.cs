using OpenNos.GameObject;
using OpenNos.GameObject._Guri;
using OpenNos.GameObject._Guri.Event;
using OpenNos.GameObject.Networking;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.Guri.Handler
{
    public class G503 : IGuriHandler
    {
        #region Properties

        public long GuriEffectId => 503;

        #endregion

        #region Methods

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            if (e.Type == 503)
            {
                if (ServerManager.Instance.EventInWaiting == true && Session.Character.IsWaitingForEvent == false)
                {
                    Session.SendPacket("bsinfo 0 7 30 0");
                    Session.SendPacket("esf 2");
                    Session.Character.IsWaitingForEvent = true;
                }
            }
        }

        #endregion
    }
}