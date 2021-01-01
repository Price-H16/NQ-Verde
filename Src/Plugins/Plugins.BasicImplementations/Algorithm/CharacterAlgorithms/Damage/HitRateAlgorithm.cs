using OpenNos.Domain;

namespace Plugins.BasicImplementations.Algorithm.CharacterAlgorithms.Damage
{
    public class HitRateAlgorithm : ICharacterStatAlgorithm
    {
        #region Members

        private const int MAX_LEVEL = 256;
        private int[,] _hitRate;

        #endregion

        #region Methods

        public int GetStat(ClassType type, byte level) => _hitRate[(int)type, level - 1 > 0 ? level - 1 : 0];

        public void Initialize()
        {
            _hitRate = new int[(int)ClassType.Unknown, MAX_LEVEL];

            for (var i = 0; i < MAX_LEVEL; i++)
            {
                var add = i % 2 == 0 ? 2 : 4;
                _hitRate[(int)ClassType.Adventurer, i] = i + 9; // approx
                _hitRate[(int)ClassType.Swordsman, i] = i + 27; // approx
                _hitRate[(int)ClassType.Magician, i] = 0; // sure
                _hitRate[(int)ClassType.Archer, 1] = 41;
                _hitRate[(int)ClassType.Archer, i] += add; // approx
                _hitRate[(int)ClassType.MartialArtist, 1] = 41;
                _hitRate[(int)ClassType.MartialArtist, i] += add; // approx
            }
        }

        #endregion
    }
}