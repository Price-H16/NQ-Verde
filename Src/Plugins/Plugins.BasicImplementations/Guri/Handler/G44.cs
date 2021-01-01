using OpenNos.GameObject;
using OpenNos.GameObject._Guri;
using OpenNos.GameObject._Guri.Event;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.Guri.Handler
{
    public class G44 : IGuriHandler
    {
        #region Properties

        public long GuriEffectId => 44;

        #endregion

        #region Methods

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            Session.SendPacket("scene 44 1");
        }

        #endregion
    }
}