using OpenNos.Domain;

namespace Plugins.BasicImplementations.Algorithm.CharacterAlgorithms.Damage
{
    public class MinDistanceAlgorithm : ICharacterStatAlgorithm
    {
        private const int MAX_LEVEL = 256;
        private int[,] _minDist;

        public void Initialize()
        {
            _minDist = new int[(int) ClassType.Unknown, MAX_LEVEL];

            for (var i = 0; i < MAX_LEVEL; i++)
            {
                _minDist[(int) ClassType.Adventurer, i] = i + 9; // approx
                _minDist[(int) ClassType.Swordsman, i] = i + 12; // approx
                _minDist[(int) ClassType.Magician, i] = 14 + i; // approx
                _minDist[(int) ClassType.Archer, i] = 2 * i; // approx
                _minDist[(int) ClassType.MartialArtist, i] = 2 * i; // approx
            }
        }

        public int GetStat(ClassType type, byte level) => _minDist[(int) type, level - 1 > 0 ? level - 1 : 0];
    }
}