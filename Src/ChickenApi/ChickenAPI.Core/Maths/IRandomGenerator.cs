// WingsEmu
//
// Developed by NosWings Team

namespace ChickenAPI.Core.Maths
{
    public interface IRandomGenerator
    {
        #region Methods

        int Next(int min, int max);

        int Next(int max);

        double Next();

        #endregion
    }
}