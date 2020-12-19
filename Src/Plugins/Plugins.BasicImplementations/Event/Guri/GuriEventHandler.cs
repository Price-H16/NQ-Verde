using System;
using System.Threading;
using System.Threading.Tasks;
using ChickenAPI.Events;
using OpenNos.GameObject._Guri;
using OpenNos.GameObject._Guri.Event;

namespace Plugins.BasicImplementations.Event.Guri
{
    public class GuriEventHandler : GenericEventHandlerBase<GuriEvent>
    {
        private readonly IGuriHandlerContainer _guriHandler;

        public GuriEventHandler(IGuriHandlerContainer guriHandler) => _guriHandler = guriHandler;

        protected override async Task Handle(GuriEvent e, CancellationToken cancellation)
        {         
            await Task.Run(() => _guriHandler.Handle(e.Sender, e), cancellation);
        }
    }
}