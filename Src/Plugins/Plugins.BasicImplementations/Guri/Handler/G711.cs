using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._Guri;
using OpenNos.GameObject._Guri.Event;
using OpenNos.GameObject.Networking;
using System.Linq;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.Guri.Handler
{
    public class G711 : IGuriHandler
    {
        #region Properties

        public long GuriEffectId => 711;

        #endregion

        #region Methods

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            if (e.Type == 711)
            {
                var tp = Session.CurrentMapInstance.Npcs.FirstOrDefault(n => n.Teleporters.Any(t => t?.TeleporterId == e.Argument))?.Teleporters.FirstOrDefault(t => t?.Type == TeleporterType.TeleportOnOtherMap);
                if (tp == null)
                {
                    return;
                }
                ServerManager.Instance.ChangeMap(Session.Character.CharacterId, tp.MapId, tp.MapX, tp.MapY);
            }
        }

        #endregion
    }
}