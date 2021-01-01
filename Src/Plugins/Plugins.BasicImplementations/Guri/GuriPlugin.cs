using Autofac;
using ChickenAPI.Plugins;
using OpenNos.Core;
using OpenNos.Core.Extensions;
using OpenNos.GameObject._Guri;
using System;

namespace Plugins.BasicImplementations.Guri
{
    public class GuriPlugin : IGamePlugin
    {
        #region Members

        private readonly IContainer _container;

        private readonly IGuriHandlerContainer _handlers;

        #endregion

        #region Instantiation

        public GuriPlugin(IGuriHandlerContainer handlers, IContainer container)
        {
            _handlers = handlers;
            _container = container;
        }

        #endregion

        #region Properties

        public PluginEnableTime EnableTime => PluginEnableTime.PreContainerBuild;

        public string Name => nameof(GuriPlugin);

        #endregion

        #region Methods

        public void OnDisable()
        {
        }

        public void OnEnable()
        {
            Logger.Log.InfoFormat("Loading Guri...");
            foreach (var handlerType in typeof(GuriPlugin).Assembly.GetTypesImplementingInterface<IGuriHandler>())
                try
                {
                    var tmp = _container.Resolve(handlerType);
                    if (!(tmp is IGuriHandler real)) continue;

                    Logger.Log.Debug($"[GURI][ADD_HANDLER] {handlerType}");
                    _handlers.Register(real).ConfigureAwait(false).GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    Logger.Log.Error("[GURI][FAIL_ADD]", e);
                }
            Logger.Log.InfoFormat("Guri initialized");
        }

        public void OnLoad()
        {
        }

        #endregion
    }
}