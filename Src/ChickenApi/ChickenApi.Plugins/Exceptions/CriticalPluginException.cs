namespace ChickenAPI.Plugins.Exceptions
{
    /// <summary>
    /// This exception should be thrown only if you need to stop the software
    /// </summary>
    public class CriticalPluginException : PluginException
    {
        #region Instantiation

        public CriticalPluginException(IPlugin plugin, string message = "Critical Plugin Exception") : base(
            $"[{plugin.Name}] {message}") => Plugin = plugin;

        #endregion

        #region Properties

        public IPlugin Plugin { get; }

        #endregion
    }
}