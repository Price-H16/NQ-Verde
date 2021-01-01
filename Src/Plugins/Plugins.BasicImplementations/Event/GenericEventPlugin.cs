using Autofac;
using ChickenAPI.Events;
using ChickenAPI.Plugins;
using OpenNos.Core;
using OpenNos.Core.Extensions;
using System;

namespace Plugins.BasicImplementations.Event
{
    public class GenericEventPlugin : IGamePlugin
    {
        #region Members

        private readonly IContainer _container;

        private readonly IEventPipeline _handlers;

        #endregion

        #region Instantiation

        public GenericEventPlugin(IEventPipeline handlers, IContainer container)
        {
            _handlers = handlers;
            _container = container;
        }

        #endregion

        #region Properties

        public PluginEnableTime EnableTime => PluginEnableTime.PreContainerBuild;

        public string Name => nameof(GenericEventPlugin);

        #endregion

        #region Methods

        public void OnDisable()
        {
        }

        public void OnEnable()
        {
            Logger.Log.InfoFormat("Loading GenericEvents...");
            foreach (var handlerType in typeof(GenericEventPlugin).Assembly.GetTypesImplementingGenericClass(
                typeof(GenericEventHandlerBase<>)))
                try
                {
                    var handler = _container.Resolve(handlerType);
                    if (!(handler is IEventHandler postProcessor)) continue;

                    var type = handlerType.BaseType.GenericTypeArguments[0];

                    _handlers.RegisterPostProcessorAsync(postProcessor, type);
                }
                catch (Exception e)
                {
                    Logger.Log.Error("[EVENT_HANDLER]", e);
                }
            Logger.Log.InfoFormat("GenericEvents initialized");
        }

        public void OnLoad()
        {
        }

        #endregion
    }
}