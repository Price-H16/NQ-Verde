using OpenNos.Domain;

namespace Plugins.BasicImplementations.Algorithm.CharacterAlgorithms
{
    public interface ICharacterStatAlgorithm
    {
        #region Methods

        int GetStat(ClassType type, byte level);

        void Initialize();

        #endregion
    }
}