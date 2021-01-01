using Autofac;
using Autofac.Core;
using System.Linq;

namespace ChickenAPI.Plugins.Modules
{
    public class CoreContainerModule : Module
    {
        #region Members

        private readonly IContainer _container;

        #endregion

        #region Instantiation

        public CoreContainerModule(IContainer container) => _container = container;

        #endregion

        #region Methods

        private void OnComponentPreparing(object sender, PreparingEventArgs e)
        {
            e.Parameters = e.Parameters.Union(
                new[]
                {
                    new ResolvedParameter(
                        (p, i) => !e.Context.IsRegistered(p.ParameterType),
                        (p, i) => _container.Resolve(p.ParameterType))
                });
        }

        #endregion

        /* protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
         {
             registration.Preparing += OnComponentPreparing;
         }*/
    }
}