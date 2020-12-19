using System.Threading;
using System.Threading.Tasks;
using ChickenAPI.Events;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;

namespace Plugins.BasicImplementations.Event.NpcDialog
{
    public class NpcDialogEventHandler : GenericEventHandlerBase<NpcDialogEvent>
    {
        private readonly INpcDialogHandlerContainer _npcDialogHandler;

        public NpcDialogEventHandler(INpcDialogHandlerContainer npcDialogHandler) => _npcDialogHandler = npcDialogHandler;

        protected override async Task Handle(NpcDialogEvent e, CancellationToken cancellation)
        {
            await Task.Run(() => _npcDialogHandler.Execute(e.Sender, e), cancellation);
        }
    }
}