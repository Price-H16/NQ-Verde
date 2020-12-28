using ChickenAPI.Enums.Game.Character;
using OpenNos.Domain;

namespace Plugins.BasicImplementations.Algorithm.CharacterAlgorithms.HpMp
{
    public class MpMax : ICharacterStatAlgorithm
    {
        private const int MAX_LEVEL = 256;
        private int[,] _stats;

        public void Initialize()
        {
            _stats = new int[(int) CharacterClassType.Unknown, MAX_LEVEL];

            // todo improve that shit
            var actual = 60;
            var baseAdventurer = 9;
            for (var i = 0; i < MAX_LEVEL; i++)
            {
                if (i % 3 == 0) baseAdventurer++;

                if (i % 4 == 0) baseAdventurer++;

                actual += baseAdventurer;

                _stats[(int) CharacterClassType.Adventurer, i] = actual; // approx
                _stats[(int) CharacterClassType.Swordsman, i] = actual;
                _stats[(int) CharacterClassType.Magician, i] = 3 * actual; // approx
                _stats[(int) CharacterClassType.Archer, i] = actual + baseAdventurer; // approx
                _stats[(int) CharacterClassType.MartialArtist, i] = actual; // approx
            }
        }

        public int GetStat(CharacterClassType type, byte level)
        {
            return _stats[(int) type, level - 1 > 0 ? level - 1 : 0];
        }
    }
}