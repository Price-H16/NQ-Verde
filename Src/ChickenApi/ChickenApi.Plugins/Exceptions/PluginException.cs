using System;

namespace ChickenAPI.Plugins.Exceptions
{
    public class PluginException : Exception
    {
        #region Instantiation

        public PluginException(string message) : base(message)
        {
        }

        #endregion
    }
}