using Autofac;
using ChickenAPI.Plugins;
using OpenNos.Core.Extensions;
using OpenNos.GameObject._Guri;

namespace Plugins.BasicImplementations.Guri
{
    public class GuriPluginCore : ICorePlugin
    {
        public PluginEnableTime EnableTime => PluginEnableTime.PreContainerBuild;

        public string Name => nameof(GuriPluginCore);

        public void OnDisable()
        {
        }

        public void OnEnable()
        {
        }

        public void OnLoad(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(GuriPlugin).Assembly)
                .Where(s => s.ImplementsInterface<IGuriHandler>());

            builder.Register(_ => new BaseGuriHandler())
                .As<IGuriHandlerContainer>().SingleInstance();
        }
    }
}