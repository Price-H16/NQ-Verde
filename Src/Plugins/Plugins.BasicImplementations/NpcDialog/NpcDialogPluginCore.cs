using Autofac;
using ChickenAPI.Plugins;
using OpenNos.Core.Extensions;
using OpenNos.GameObject._NpcDialog;

namespace Plugins.BasicImplementations.NpcDialog
{
    public class NpcDialogPluginCore : ICorePlugin
    {
        public PluginEnableTime EnableTime => PluginEnableTime.PreContainerBuild;

        public string Name => nameof(NpcDialogPluginCore);

        public void OnDisable()
        {
        }

        public void OnEnable()
        {
        }

        public void OnLoad(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(NpcDialogPlugin).Assembly)
                .Where(s => s.ImplementsInterface<INpcDialogAsyncHandler>());
            builder.Register(_ => new NpcDialogHandlerContainer())
                .As<INpcDialogHandlerContainer>().SingleInstance();
        }
    }
}