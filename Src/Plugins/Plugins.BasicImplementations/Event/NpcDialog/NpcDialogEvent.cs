using ChickenAPI.Events;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.Event.NpcDialog
{
    public class NpcDialogEventHandler : GenericEventHandlerBase<NpcDialogEvent>
    {
        #region Members

        private readonly INpcDialogHandlerContainer _npcDialogHandler;

        #endregion

        #region Instantiation

        public NpcDialogEventHandler(INpcDialogHandlerContainer npcDialogHandler) => _npcDialogHandler = npcDialogHandler;

        #endregion

        #region Methods

        protected override async Task Handle(NpcDialogEvent e, CancellationToken cancellation)
        {
            await Task.Run(() => _npcDialogHandler.Execute(e.Sender, e), cancellation);
        }

        #endregion
    }
}