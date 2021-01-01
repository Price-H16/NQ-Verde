using ChickenAPI.Events;
using OpenNos.GameObject._ItemUsage;
using OpenNos.GameObject._ItemUsage.Event;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.Event.ItemUsage
{
    public class ItemUsageEvent : GenericEventHandlerBase<InventoryUseItemEvent>
    {
        #region Members

        private readonly IItemUsageHandlerContainer _itemUsageHandler;

        #endregion

        #region Instantiation

        public ItemUsageEvent(IItemUsageHandlerContainer itemUsageHandler) => _itemUsageHandler = itemUsageHandler;

        #endregion

        #region Methods

        protected override async Task Handle(InventoryUseItemEvent e, CancellationToken cancellation)
        {
            await Task.Run(() => _itemUsageHandler.UseItem(e.Sender, e), cancellation);
        }

        #endregion
    }
}