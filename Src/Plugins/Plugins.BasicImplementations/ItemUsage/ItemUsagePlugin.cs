using Autofac;
using ChickenAPI.Plugins;
using OpenNos.Core;
using OpenNos.Core.Extensions;
using OpenNos.GameObject._ItemUsage;
using System;

namespace Plugins.BasicImplementations.ItemUsage
{
    public class ItemUsagePlugin : IGamePlugin
    {
        #region Members

        private readonly IContainer _container;

        private readonly IItemUsageHandlerContainer _handlers;

        #endregion

        #region Instantiation

        public ItemUsagePlugin(IItemUsageHandlerContainer handlers, IContainer container)
        {
            _handlers = handlers;
            _container = container;
        }

        #endregion

        #region Properties

        public PluginEnableTime EnableTime => PluginEnableTime.PreContainerBuild;

        public string Name => nameof(ItemUsagePlugin);

        #endregion

        #region Methods

        public void OnDisable()
        {
        }

        public void OnEnable()
        {
            Logger.Log.InfoFormat("Loading ItemUsage...");
            foreach (var handlerType in typeof(ItemUsagePlugin).Assembly
                .GetTypesImplementingInterface<IUseItemRequestHandlerAsync>())
                try
                {
                    var tmp = _container.Resolve(handlerType);
                    if (!(tmp is IUseItemRequestHandlerAsync real)) continue;

                    Logger.Log.Debug($"[ITEM_USAGE][ADD_HANDLER] {handlerType}");
                    _handlers.RegisterItemUsageCallback(real).ConfigureAwait(false).GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    Logger.Log.Error("[ITEM_USAGE][FAIL_ADD]", e);
                }
            Logger.Log.InfoFormat("ItemUsage initialized");
        }

        public void OnLoad()
        {
        }

        #endregion
    }
}