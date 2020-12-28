using ChickenAPI.Enums.Game.Character;
using OpenNos.Domain;

namespace Plugins.BasicImplementations.Algorithm.CharacterAlgorithms.Damage
{
    public class HitRateAlgorithm : ICharacterStatAlgorithm
    {
        private const int MAX_LEVEL = 256;
        private int[,] _hitRate;

        public void Initialize()
        {
            _hitRate = new int[(int) CharacterClassType.Unknown, MAX_LEVEL];

            for (var i = 0; i < MAX_LEVEL; i++)
            {
                var add = i % 2 == 0 ? 2 : 4;
                _hitRate[(int) CharacterClassType.Adventurer, i] = i + 9; // approx
                _hitRate[(int) CharacterClassType.Swordsman, i] = i + 27; // approx
                _hitRate[(int) CharacterClassType.Magician, i] = 0; // sure
                _hitRate[(int) CharacterClassType.Archer, 1] = 41;
                _hitRate[(int) CharacterClassType.Archer, i] += add; // approx
                _hitRate[(int) CharacterClassType.MartialArtist, 1] = 41;
                _hitRate[(int) CharacterClassType.MartialArtist, i] += add; // approx
            }
        }

        public int GetStat(CharacterClassType type, byte level) => _hitRate[(int) type, level - 1 > 0 ? level - 1 : 0];
    }
}