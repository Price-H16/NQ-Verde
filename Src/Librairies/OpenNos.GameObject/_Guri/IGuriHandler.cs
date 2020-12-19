using System.Threading.Tasks;
using OpenNos.GameObject._Guri.Event;

namespace OpenNos.GameObject._Guri
{
    public interface IGuriHandler
    {
        long GuriEffectId { get; }

        Task ExecuteAsync(ClientSession player, GuriEvent e);
    }
}