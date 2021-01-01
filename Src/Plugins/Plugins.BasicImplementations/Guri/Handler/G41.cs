using OpenNos.GameObject;
using OpenNos.GameObject._Guri;
using OpenNos.GameObject._Guri.Event;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.Guri.Handler
{
    public class G41 : IGuriHandler
    {
        #region Properties

        public long GuriEffectId => 41;

        #endregion

        #region Methods

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            Session.SendPacket("scene 41 1");
        }

        #endregion
    }
}