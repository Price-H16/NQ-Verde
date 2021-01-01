using OpenNos.GameObject;
using OpenNos.GameObject._Guri;
using OpenNos.GameObject._Guri.Event;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.Guri.Handler
{
    public class G43 : IGuriHandler
    {
        #region Properties

        public long GuriEffectId => 43;

        #endregion

        #region Methods

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            Session.SendPacket("scene 43 1");
        }

        #endregion
    }
}