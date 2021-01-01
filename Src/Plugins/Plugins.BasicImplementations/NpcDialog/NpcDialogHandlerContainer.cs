using OpenNos.Core;
using OpenNos.GameObject._Event;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog
{
    public class NpcDialogHandlerContainer : INpcDialogHandlerContainer
    {
        #region Members

        private readonly Dictionary<long, INpcDialogAsyncHandler> _handlers;

        #endregion

        #region Instantiation

        public NpcDialogHandlerContainer()
        {
            _handlers = new Dictionary<long, INpcDialogAsyncHandler>();
        }

        #endregion

        #region Methods

        public void Execute(EventEntity player, NpcDialogEvent e)
        {
            ExecuteAsync(player, e).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task ExecuteAsync(EventEntity player, NpcDialogEvent e)
        {
            if (!_handlers.TryGetValue(e.Runner, out var handler))
            {
                Logger.Log.Debug($"[HANDLER_NOT_FOUND] NPC_DIALOG : {e.Runner} ");
                return;
            }

            await handler.Execute(player.Character.Session, e);
        }

        public async Task RegisterAsync(INpcDialogAsyncHandler handler)
        {
            _handlers.Add(handler.HandledId, handler);
            Logger.Log.Debug($"[NPC_DIALOG][REGISTER_HANDLER] DIALOG_ID : {handler.HandledId} REGISTERED !");
        }

        public async Task UnregisterAsync(INpcDialogAsyncHandler handler)
        {
            _handlers.Remove(handler.HandledId);
        }

        #endregion
    }
}