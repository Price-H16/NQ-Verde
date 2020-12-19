using Autofac;
using ChickenAPI.Core.Logging;
using ChickenAPI.Plugins;

namespace NosQuest.Plugins.Logging
{
    public class LoggingPlugin : ICorePlugin
    {
        public PluginEnableTime EnableTime => PluginEnableTime.PreContainerBuild;

        public string Name => nameof(LoggingPlugin);

        public void OnDisable()
        {
        }

        public void OnEnable()
        {
        }

        public void OnLoad(ContainerBuilder builder)
        {
            builder.RegisterType<SerilogLogger>().As<ILogger>();
        }
    }
}