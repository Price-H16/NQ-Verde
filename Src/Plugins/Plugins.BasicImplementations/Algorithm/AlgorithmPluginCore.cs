using Autofac;
using ChickenAPI.Plugins;
using OpenNos.Core;
using OpenNos.GameObject._Algorithm;

namespace Plugins.BasicImplementations.Algorithm
{
    public class AlgorithmPluginCore : ICorePlugin
    {
        public PluginEnableTime EnableTime => PluginEnableTime.PreContainerBuild;

        public string Name => nameof(AlgorithmPluginCore);

        public void OnDisable()
        {
        }

        public void OnEnable()
        {
        }

        public void OnLoad(ContainerBuilder builder)
        {
            Logger.Log.InfoFormat("Loading Algorithms...");
            builder.Register(s => new AlgorithmService()).As<IAlgorithmService>();
            builder.Register(s => new NpcMonsterAlgorithmService()).As<INpcMonsterAlgorithmService>();
            builder.Register(s => new DamageAlgorithm()).As<IDamageAlgorithm>();
            Logger.Log.InfoFormat("Algorithms initialized");
        }
    }
}