using OpenNos.GameObject;
using OpenNos.GameObject._Guri;
using OpenNos.GameObject._Guri.Event;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.Guri.Handler
{
    public class G42 : IGuriHandler
    {
        #region Properties

        public long GuriEffectId => 42;

        #endregion

        #region Methods

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            Session.SendPacket("scene 42 1");
        }

        #endregion
    }
}