using Autofac;

namespace ChickenAPI.Core.IoC
{
    public static class ChickenContainer
    {
        #region Members

        public static ContainerBuilder Builder = new ContainerBuilder();

        #endregion

        #region Properties

        public static IContainer Instance { get; private set; }

        #endregion

        #region Methods

        public static void Initialize()
        {
            Instance = Builder.Build();
        }

        #endregion
    }
}