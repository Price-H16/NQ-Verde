using ChickenAPI.Events;
using OpenNos.GameObject._BCards;
using OpenNos.GameObject._BCards.Event;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.Event.BCard
{
    public class BCardEventHandler : GenericEventHandlerBase<BCardEvent>
    {
        #region Members

        private readonly IBCardEffectHandlerContainer _bCardEventHandler;

        #endregion

        #region Instantiation

        public BCardEventHandler(IBCardEffectHandlerContainer itemUsageHandler)
        {
            _bCardEventHandler = itemUsageHandler;
        }

        #endregion

        #region Methods

        protected override async Task Handle(BCardEvent e, CancellationToken cancellation)
        {
            await Task.Run(() => _bCardEventHandler.Execute(e.Target, e.Sender, e.Card), cancellation);
        }

        #endregion
    }
}