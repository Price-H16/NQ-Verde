using Autofac;
using OpenNos.GameObject._Algorithm;

namespace Plugins.BasicImplementations.Algorithm.IoC
{
    public class AlgorithmDependenciesInjector
    {
        #region Methods

        public static void InjectDependencies(ContainerBuilder builder)
        {
            builder.Register(s => new AlgorithmService()).As<IAlgorithmService>();
            builder.Register(s => new NpcMonsterAlgorithmService()).As<INpcMonsterAlgorithmService>();
            builder.Register(s => new DamageAlgorithm()).As<IDamageAlgorithm>();
        }

        #endregion
    }
}