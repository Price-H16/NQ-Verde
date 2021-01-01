using OpenNos.GameObject;
using OpenNos.GameObject._Guri;
using OpenNos.GameObject._Guri.Event;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.Guri.Handler
{
    public class G202 : IGuriHandler
    {
        #region Properties

        public long GuriEffectId => 202;

        #endregion

        #region Methods

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            if (e.Type == 202)
            {
                //Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PARTNER_BACKPACK"), 10));
                Session.SendPacket(Session.Character.GeneratePStashAll());
            }
        }

        #endregion
    }
}