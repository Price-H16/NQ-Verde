using System;
using System.Collections.Generic;

namespace NosTale.Configuration.Utilities
{
    public class DependencyContainer
    {
        #region Members

        private static readonly Lazy<DependencyContainer> Lazy = new Lazy<DependencyContainer>(() => new DependencyContainer());

        private readonly Dictionary<Type, object> _objects = new Dictionary<Type, object>();

        #endregion

        #region Properties

        public static DependencyContainer Instance => Lazy.Value;

        #endregion

        #region Methods

        public T GetInstance<T>() where T : class => !_objects.TryGetValue(typeof(T), out object instance) ? null : instance as T;

        public void Register<T>(T instance) where T : class
        {
            _objects[typeof(T)] = instance;
        }

        #endregion
    }
}