using OpenNos.GameObject;
using OpenNos.GameObject._Guri;
using OpenNos.GameObject._Guri.Event;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.Guri.Handler
{
    public class G710 : IGuriHandler
    {
        #region Properties

        public long GuriEffectId => 710;

        #endregion

        #region Methods

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            if (e.Type == 710)
            {
                if (e.Value != null)
                {
                    // TODO: MAP TELEPORTER
                }
            }
        }

        #endregion
    }
}