using Autofac;
using ChickenAPI.Plugins;
using OpenNos.Core.Extensions;
using OpenNos.GameObject._BCards;

namespace Plugins.BasicImplementations.BCards
{
    public class BCardPluginCore : ICorePlugin
    {
        public PluginEnableTime EnableTime => PluginEnableTime.PreContainerBuild;

        public string Name => nameof(BCardPluginCore);

        public void OnDisable()
        {
        }

        public void OnEnable()
        {
        }

        public void OnLoad(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(BCardPlugin).Assembly)
                .Where(s => s.ImplementsInterface<IBCardEffectAsyncHandler>());

            builder.Register(_ => new BCardHandlerContainer())
                .As<IBCardEffectHandlerContainer>().SingleInstance();
        }
    }
}