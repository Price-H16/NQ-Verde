using System;
using Autofac;
using ChickenAPI.Plugins;
using OpenNos.Core;
using OpenNos.Core.Extensions;
using OpenNos.GameObject._BCards;

namespace Plugins.BasicImplementations.BCards
{
    public class BCardPlugin : IGamePlugin
    {
        public PluginEnableTime EnableTime => PluginEnableTime.PreContainerBuild;

        private readonly IContainer _container;
        private readonly IBCardEffectHandlerContainer _handlers;

        public BCardPlugin(IBCardEffectHandlerContainer handlers, IContainer container)
        {
            _handlers = handlers;
            _container = container;
        }

        public string Name => nameof(BCardPlugin);

        public void OnDisable()
        {
        }

        public void OnEnable()
        {
            Logger.Log.InfoFormat("Loading BCards...");
            foreach (var handlerType in typeof(BCardPlugin).Assembly
                .GetTypesImplementingInterface<IBCardEffectAsyncHandler>())
                try
                {
                    var tmp = _container.Resolve(handlerType);
                    if (!(tmp is IBCardEffectAsyncHandler real)) continue;

                    Logger.Log.Debug($"[BCARD][ADD_HANDLER] {handlerType}");
                    _handlers.RegisterAsync(real).ConfigureAwait(false).GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    Logger.Log.Error("[BCARD][FAIL_ADD]", e);
                }
            Logger.Log.InfoFormat("BCards initialized");
        }

        public void OnLoad()
        {
        }
    }
}