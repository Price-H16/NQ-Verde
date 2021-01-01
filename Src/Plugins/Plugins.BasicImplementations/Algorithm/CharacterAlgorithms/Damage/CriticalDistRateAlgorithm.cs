using OpenNos.Domain;

namespace Plugins.BasicImplementations.Algorithm.CharacterAlgorithms.Damage
{
    public class CriticalDistRateAlgorithm : ICharacterStatAlgorithm
    {
        #region Members

        private const int MAX_LEVEL = 256;
        private int[,] _criticalDistRate;

        #endregion

        #region Methods

        public int GetStat(ClassType type, byte level) => _criticalDistRate[(int)type, level - 1 > 0 ? level - 1 : 0];

        public void Initialize()
        {
            _criticalDistRate = new int[(int)ClassType.Unknown, MAX_LEVEL];

            for (var i = 0; i < MAX_LEVEL; i++)
            {
                _criticalDistRate[(int)ClassType.Adventurer, i] = 0; // sure
                _criticalDistRate[(int)ClassType.Swordsman, i] = 0; // approx
                _criticalDistRate[(int)ClassType.Magician, i] = 0; // sure
                _criticalDistRate[(int)ClassType.Archer, i] = 0; // sure
                _criticalDistRate[(int)ClassType.MartialArtist, i] = 0; // sure
            }
        }

        #endregion
    }
}