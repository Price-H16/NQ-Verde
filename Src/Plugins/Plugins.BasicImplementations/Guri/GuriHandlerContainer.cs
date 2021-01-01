using OpenNos.Core;
using OpenNos.GameObject._Event;
using OpenNos.GameObject._Guri;
using OpenNos.GameObject._Guri.Event;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.Guri
{
    public class BaseGuriHandler : IGuriHandlerContainer
    {
        #region Members

        protected readonly Dictionary<long, IGuriHandler> HandlersByDialogId;

        #endregion

        #region Instantiation

        public BaseGuriHandler()
        {
            HandlersByDialogId = new Dictionary<long, IGuriHandler>();
        }

        #endregion

        #region Methods

        public void Handle(EventEntity player, GuriEvent args)
        {
            HandleAsync(player, args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task HandleAsync(EventEntity player, GuriEvent args)
        {
            if (!HandlersByDialogId.TryGetValue(args.Type, out var handler))
            {
                Logger.Log.Debug($"[HANDLER_NOT_FOUND] GURI_EFFECT : {args.Type} ");
                return;
            }

            await handler.ExecuteAsync(player.Character.Session, args);
        }

        public async Task Register(IGuriHandler handler)
        {
            if (HandlersByDialogId.ContainsKey(handler.GuriEffectId)) return;

            Logger.Log.Debug($"[GURI][REGISTER_HANDLER] GURI_EFFECT : {handler.GuriEffectId} REGISTERED !");
            HandlersByDialogId.Add(handler.GuriEffectId, handler);
        }

        public async Task Unregister(long guriEffectId)
        {
            HandlersByDialogId.Remove(guriEffectId);
        }

        #endregion
    }
}