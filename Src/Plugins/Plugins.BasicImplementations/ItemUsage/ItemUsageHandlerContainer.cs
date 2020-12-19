using System.Collections.Generic;
using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._Event;
using OpenNos.GameObject._ItemUsage;
using OpenNos.GameObject._ItemUsage.Event;

namespace Plugins.BasicImplementations.ItemUsage
{
    public class ItemUsageHandlerContainer : IItemUsageHandlerContainer
    {
        private readonly Dictionary<(long, ItemPluginType), IUseItemRequestHandlerAsync> _handlers
            = new Dictionary<(long, ItemPluginType), IUseItemRequestHandlerAsync>();

        public async Task RegisterItemUsageCallback(IUseItemRequestHandlerAsync handler)
        {
            _handlers.Add((handler.EffectId, handler.Type), handler);
            Logger.Log.Debug($"[ITEM_USAGE][REGISTER_HANDLER] UI_EFFECT : {handler.EffectId} && TYPE : {handler.Type} REGISTERED !");
        }

        public async Task UnregisterAsync(IUseItemRequestHandlerAsync handler)
        {
            if (!_handlers.ContainsKey((handler.EffectId, handler.Type)))
            {
                return;
            }

            _handlers.Remove((handler.EffectId, handler.Type));
        }

        public void UseItem(EventEntity player, InventoryUseItemEvent e)
        {
            UseItemAsync(player, e).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task UseItemAsync(EventEntity player, InventoryUseItemEvent e)
        {
            if (!_handlers.TryGetValue((e.Item.Item.Effect, e.Item.Item.PluginType), out var handler))
            {
                Logger.Log.Debug($"[HANDLER_NOT_FOUND] USE_ITEM : PluginType: {e.Item.Item.PluginType} Effect: {e.Item.Item.Effect} ItemType: {e.Item.Item.ItemType}");
                return;
            }

            await handler.HandleAsync(player.Character.Session, e);
        }
    }
}