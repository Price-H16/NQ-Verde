using System.Threading.Tasks;
using OpenNos.GameObject;
using OpenNos.GameObject._Guri;
using OpenNos.GameObject._Guri.Event;

namespace Plugins.BasicImplementations.Guri.Handler
{
    public class G5 : IGuriHandler
    {
        public long GuriEffectId => 5;

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            // Useless Just a Simple Dance Packet
        }
    }
} 