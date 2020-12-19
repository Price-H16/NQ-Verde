using System.Threading.Tasks;
using OpenNos.GameObject._Event;
using OpenNos.GameObject._ItemUsage.Event;

namespace OpenNos.GameObject._ItemUsage
{
    public interface IItemUsageHandlerContainer
    {
        Task RegisterItemUsageCallback(IUseItemRequestHandlerAsync handler);

        Task UnregisterAsync(IUseItemRequestHandlerAsync handler);

        void UseItem(EventEntity player, InventoryUseItemEvent e);

        Task UseItemAsync(EventEntity player, InventoryUseItemEvent itemInstance);
    }
}