namespace Plugins.BasicImplementations.Algorithm
{
    public interface ILevelBasedDataAlgorithm
    {
        #region Properties

        long[] Data { get; set; }

        #endregion

        #region Methods

        void Initialize();

        #endregion
    }
}