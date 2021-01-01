using OpenNos.GameObject;
using OpenNos.GameObject._Guri;
using OpenNos.GameObject._Guri.Event;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.Guri.Handler
{
    public class G5 : IGuriHandler
    {
        #region Properties

        public long GuriEffectId => 5;

        #endregion

        #region Methods

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            // Useless Just a Simple Dance Packet
        }

        #endregion
    }
}