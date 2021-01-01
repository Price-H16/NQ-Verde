namespace ChickenAPI.Plugins
{
    public interface IConfigurablePlugin : IPlugin
    {
        #region Methods

        /// <summary>
        /// Reloads its own configuration
        /// </summary>
        void ReloadConfig();

        /// <summary>
        /// Saves the configuration
        /// </summary>
        void SaveConfig();

        /// <summary>
        /// Saves the default configuration of the plugin
        /// </summary>
        void SaveDefaultConfig();

        #endregion
    }
}