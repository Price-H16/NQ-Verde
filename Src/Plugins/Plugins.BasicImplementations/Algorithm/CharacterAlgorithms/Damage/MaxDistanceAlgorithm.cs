using OpenNos.Domain;

namespace Plugins.BasicImplementations.Algorithm.CharacterAlgorithms.Damage
{
    public class MaxDistanceAlgorithm : ICharacterStatAlgorithm
    {
        #region Members

        private const int MAX_LEVEL = 256;
        private int[,] _maxDist;

        #endregion

        #region Methods

        public int GetStat(ClassType type, byte level) => _maxDist[(int)type, level - 1 > 0 ? level - 1 : 0];

        public void Initialize()
        {
            _maxDist = new int[(int)ClassType.Unknown, MAX_LEVEL];

            for (var i = 0; i < MAX_LEVEL; i++)
            {
                _maxDist[(int)ClassType.Adventurer, i] = i + 9; // approx
                _maxDist[(int)ClassType.Swordsman, i] = i + 12; // approx
                _maxDist[(int)ClassType.Magician, i] = 14 + i; // approx
                _maxDist[(int)ClassType.Archer, i] = 2 * i; // approx
                _maxDist[(int)ClassType.MartialArtist, i] = 2 * i; // approx
            }
        }

        #endregion
    }
}