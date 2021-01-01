using OpenNos.Domain;

namespace Plugins.BasicImplementations.Algorithm.CharacterAlgorithms.Distance
{
    public class RangedDefenceAlgorithm : ICharacterStatAlgorithm
    {
        #region Members

        private const int MAX_LEVEL = 256;
        private int[,] _stats;

        #endregion

        #region Methods

        public int GetStat(ClassType type, byte level) => _stats[(int)type, level - 1 > 0 ? level - 1 : 0];

        public void Initialize()
        {
            _stats = new int[(int)ClassType.Unknown, MAX_LEVEL];

            for (var i = 0; i < MAX_LEVEL; i++)
            {
                _stats[(int)ClassType.Adventurer, i] = (i + 9) / 2; // approx
                _stats[(int)ClassType.Swordsman, i] = i; // approx
                _stats[(int)ClassType.Magician, i] = i + 20; // approx
                _stats[(int)ClassType.Archer, i] = i; // approx
                _stats[(int)ClassType.MartialArtist, i] = i; // approx
            }
        }

        #endregion
    }
}