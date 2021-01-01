using OpenNos.GameObject;
using OpenNos.GameObject._Guri;
using OpenNos.GameObject._Guri.Event;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.Guri.Handler
{
    public class G513 : IGuriHandler
    {
        #region Properties

        public long GuriEffectId => 513;

        #endregion

        #region Methods

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            if (e.Type == 513)
            {
                if (Session?.Character?.MapInstance == null)
                {
                    return;
                }

                if (Session.Character.IsLaurenaMorph())
                {
                    Session.Character.MapInstance.Broadcast(Session.Character.GenerateEff(4054));
                    Session.Character.ClearLaurena();
                }
            }
        }

        #endregion
    }
}