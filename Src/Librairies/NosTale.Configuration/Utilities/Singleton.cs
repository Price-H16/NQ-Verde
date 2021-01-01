using System;

namespace NosTale.Configuration.Utilities
{
    /// <summary>
    /// Instanciate a T Thread Safe Singleton
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> where T : class, new()
    {
        #region Members

        private static readonly Lazy<T> Lazy = new Lazy<T>(() => new T());

        #endregion

        #region Properties

        public static T Instance => Lazy.Value;

        #endregion
    }
}