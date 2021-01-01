using OpenNos.GameObject;
using OpenNos.GameObject._Guri;
using OpenNos.GameObject._Guri.Event;
using OpenNos.GameObject.Helpers;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.Guri.Handler
{
    public class G2 : IGuriHandler
    {
        #region Properties

        public long GuriEffectId => 2;

        #endregion

        #region Methods

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            if (e.Type == 2)
            {
                Session.CurrentMapInstance?.Broadcast(
                    UserInterfaceHelper.GenerateGuri(2, 1, Session.Character.CharacterId),
                    Session.Character.PositionX, Session.Character.PositionY);
            }
        }

        #endregion
    }
}