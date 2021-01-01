using ChickenAPI.Events;
using OpenNos.GameObject._Guri;
using OpenNos.GameObject._Guri.Event;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.Event.Guri
{
    public class GuriEventHandler : GenericEventHandlerBase<GuriEvent>
    {
        #region Members

        private readonly IGuriHandlerContainer _guriHandler;

        #endregion

        #region Instantiation

        public GuriEventHandler(IGuriHandlerContainer guriHandler) => _guriHandler = guriHandler;

        #endregion

        #region Methods

        protected override async Task Handle(GuriEvent e, CancellationToken cancellation)
        {
            await Task.Run(() => _guriHandler.Handle(e.Sender, e), cancellation);
        }

        #endregion
    }
}