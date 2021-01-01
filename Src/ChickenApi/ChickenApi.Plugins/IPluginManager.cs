using System.IO;

namespace ChickenAPI.Plugins
{
    public interface IPluginManager
    {
        #region Methods

        IPlugin[] LoadPlugin(FileInfo file);

        IPlugin[] LoadPlugins(DirectoryInfo directory);

        #endregion
    }
}