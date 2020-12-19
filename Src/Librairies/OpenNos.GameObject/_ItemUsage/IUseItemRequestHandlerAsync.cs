using System.Threading.Tasks;
using OpenNos.Domain;
using OpenNos.GameObject._Event;
using OpenNos.GameObject._ItemUsage.Event;

namespace OpenNos.GameObject._ItemUsage
{
    public interface IUseItemRequestHandlerAsync
    {
        /// <summary>
        ///     ItemType for the handler
        /// </summary>
        ItemPluginType Type { get; }

        long EffectId { get; }

        Task HandleAsync(ClientSession session, InventoryUseItemEvent e);
    }
}