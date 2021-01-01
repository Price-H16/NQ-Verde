using Autofac;
using ChickenAPI.Plugins;
using OpenNos.Core;
using OpenNos.Core.Extensions;
using OpenNos.GameObject._BCards;
using System;

namespace Plugins.BasicImplementations.BCards
{
    public class BCardPlugin : IGamePlugin
    {
        #region Members

        private readonly IContainer _container;

        private readonly IBCardEffectHandlerContainer _handlers;

        #endregion

        #region Instantiation

        public BCardPlugin(IBCardEffectHandlerContainer handlers, IContainer container)
        {
            _handlers = handlers;
            _container = container;
        }

        #endregion

        #region Properties

        public PluginEnableTime EnableTime => PluginEnableTime.PreContainerBuild;

        public string Name => nameof(BCardPlugin);

        #endregion

        #region Methods

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

        #endregion
    }
}