using Autofac;
using ChickenAPI.Events;
using ChickenAPI.Plugins;

namespace Plugins.BasicImplementations.Event
{
    public class GenericEventPluginCore : ICorePlugin
    {
        #region Properties

        public PluginEnableTime EnableTime => PluginEnableTime.PreContainerBuild;

        public string Name => nameof(GenericEventPluginCore);

        #endregion

        #region Methods

        public void OnDisable()
        {
        }

        public void OnEnable()
        {
        }

        public void OnLoad(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(GenericEventPlugin).Assembly)
                .AsClosedTypesOf(typeof(GenericEventHandlerBase<>)).PropertiesAutowired();
        }

        #endregion
    }
}