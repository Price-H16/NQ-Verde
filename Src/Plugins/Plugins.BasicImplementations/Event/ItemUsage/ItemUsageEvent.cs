using System.Threading;
using System.Threading.Tasks;
using ChickenAPI.Events;
using OpenNos.GameObject._ItemUsage;
using OpenNos.GameObject._ItemUsage.Event;

namespace Plugins.BasicImplementations.Event.ItemUsage
{
    public class ItemUsageEvent : GenericEventHandlerBase<InventoryUseItemEvent>
    {
        private readonly IItemUsageHandlerContainer _itemUsageHandler;

        public ItemUsageEvent(IItemUsageHandlerContainer itemUsageHandler) => _itemUsageHandler = itemUsageHandler;

        protected override async Task Handle(InventoryUseItemEvent e, CancellationToken cancellation)
        {
            await Task.Run(() => _itemUsageHandler.UseItem(e.Sender, e), cancellation);
        }
    }
}