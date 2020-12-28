using ChickenAPI.Enums.Game.Character;
using OpenNos.Domain;

namespace Plugins.BasicImplementations.Algorithm.CharacterAlgorithms.HpMp
{
    public class HpMax : ICharacterStatAlgorithm
    {
        private const int MAX_LEVEL = 256;
        private int[,] _stats;

        public void Initialize()
        {
            _stats = new int[(int) CharacterClassType.Unknown, MAX_LEVEL];

            // todo improve that shit
            for (var i = 0; i < MAX_LEVEL; i++)
            {
                var jSwordman = 16;
                var hpSwordman = 946;
                var incSwordman = 85;
                while (jSwordman <= i)
                {
                    if (jSwordman % 5 == 2)
                    {
                        hpSwordman += incSwordman / 2;
                        incSwordman += 2;
                    }
                    else
                    {
                        hpSwordman += incSwordman;
                        incSwordman += 4;
                    }

                    ++jSwordman;
                }

                var hpArcher = 680;
                var incArcher = 35;
                var jArcher = 16;
                while (jArcher <= i)
                {
                    hpArcher += incArcher;
                    ++incArcher;
                    if (jArcher % 10 == 1 || jArcher % 10 == 5 || jArcher % 10 == 8)
                    {
                        hpArcher += incArcher;
                        ++incArcher;
                    }

                    ++jArcher;
                }

                _stats[(int) CharacterClassType.Adventurer, i] = (int) (1 / 2.0 * i * i + 31 / 2.0 * i + 205); // approx
                _stats[(int) CharacterClassType.Swordsman, i] = hpSwordman;
                _stats[(int) CharacterClassType.Magician, i] = (int) ((i + 15) * (i + 15) + i + 15.0 - 465 + 550); // approx
                _stats[(int) CharacterClassType.Archer, i] = hpArcher; // approx
                _stats[(int) CharacterClassType.MartialArtist, i] = hpSwordman; // approx
            }
        }

        public int GetStat(CharacterClassType type, byte level) => _stats[(int) type, level - 1 > 0 ? level - 1 : 0];
    }
}