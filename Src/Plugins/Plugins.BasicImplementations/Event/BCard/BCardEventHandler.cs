using System.Threading;
using System.Threading.Tasks;
using ChickenAPI.Events;
using OpenNos.GameObject._BCards;
using OpenNos.GameObject._BCards.Event;
using OpenNos.GameObject._ItemUsage;
using OpenNos.GameObject._ItemUsage.Event;

namespace Plugins.BasicImplementations.Event.BCard
{
    public class BCardEventHandler : GenericEventHandlerBase<BCardEvent>
    {
        private readonly IBCardEffectHandlerContainer _bCardEventHandler;

        public BCardEventHandler(IBCardEffectHandlerContainer itemUsageHandler)
        {
            _bCardEventHandler = itemUsageHandler;
        }

        protected override async Task Handle(BCardEvent e, CancellationToken cancellation)
        {
            await Task.Run(() => _bCardEventHandler.Execute(e.Target, e.Sender, e.Card), cancellation);
        }
    }
}