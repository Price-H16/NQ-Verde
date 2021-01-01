using Autofac;

namespace ChickenAPI.Plugins
{
    public interface ICorePlugin : IPlugin
    {
        #region Methods

        /// <summary>
        /// Loads the plugin with the given container builder to register dependencies
        /// </summary>
        /// <param name="builder"></param>
        void OnLoad(ContainerBuilder builder);

        #endregion
    }
}