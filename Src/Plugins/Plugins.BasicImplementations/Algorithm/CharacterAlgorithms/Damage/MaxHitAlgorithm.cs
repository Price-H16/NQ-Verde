using OpenNos.Domain;

namespace Plugins.BasicImplementations.Algorithm.CharacterAlgorithms.Damage
{
    public class MaxHitAlgorithm : ICharacterStatAlgorithm
    {
        #region Members

        private const int MAX_LEVEL = 256;
        private int[,] _maxHit;

        #endregion

        #region Methods

        public int GetStat(ClassType type, byte level) => _maxHit[(int)type, level - 1 > 0 ? level - 1 : 0];

        public void Initialize()
        {
            _maxHit = new int[(int)ClassType.Unknown, MAX_LEVEL];

            for (var i = 0; i < MAX_LEVEL; i++)
            {
                _maxHit[(int)ClassType.Adventurer, i] = i + 9; // approx
                _maxHit[(int)ClassType.Swordsman, i] = 2 * i + 5; // approx Numbers n such that 10n+9 is prime.
                _maxHit[(int)ClassType.Magician, i] = 2 * i + 9; // approx Numbers n such that n^2 is of form x^2+40y^2 with positive x,y.
                _maxHit[(int)ClassType.Archer, i] = 9 + i * 3; // approx
                _maxHit[(int)ClassType.MartialArtist, i] = 9 + i * 3; // nop
            }
        }

        #endregion
    }
}