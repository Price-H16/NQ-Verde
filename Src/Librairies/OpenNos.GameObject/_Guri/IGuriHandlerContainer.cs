using System.Threading.Tasks;
using OpenNos.GameObject._Event;
using OpenNos.GameObject._Guri.Event;

namespace OpenNos.GameObject._Guri
{
    public interface IGuriHandlerContainer
    {
        Task Register(IGuriHandler handler);

        Task Unregister(long guriEffectId);

        void Handle(EventEntity player, GuriEvent args);

        Task HandleAsync(EventEntity player, GuriEvent args);
    }
}