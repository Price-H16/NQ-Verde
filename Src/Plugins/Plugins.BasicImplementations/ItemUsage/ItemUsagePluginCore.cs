using Autofac;
using ChickenAPI.Plugins;
using OpenNos.Core.Extensions;
using OpenNos.GameObject._ItemUsage;

namespace Plugins.BasicImplementations.ItemUsage
{
    public class ItemUsagePluginCore : ICorePlugin
    {
        public PluginEnableTime EnableTime => PluginEnableTime.PreContainerBuild;

        public string Name => nameof(ItemUsagePluginCore);

        public void OnDisable()
        {
        }

        public void OnEnable()
        {
        }

        public void OnLoad(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(ItemUsagePlugin).Assembly)
                .Where(s => s.ImplementsInterface<IUseItemRequestHandlerAsync>());
            builder.Register(_ => new ItemUsageHandlerContainer()).As<IItemUsageHandlerContainer>().SingleInstance();
        }
    }
}