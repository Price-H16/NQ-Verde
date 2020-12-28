using System;
using ChickenAPI.Enums.Game.Character;
using OpenNos.Domain;

namespace Plugins.BasicImplementations.Algorithm.CharacterAlgorithms.HpMp
{
    public class HpRegen : ICharacterStatAlgorithm
    {
        public void Initialize()
        {
        }

        public int GetStat(CharacterClassType type, byte level)
        {
            switch (type)
            {
                case CharacterClassType.Adventurer:
                    return 25;
                case CharacterClassType.Swordsman:
                    return 26;
                case CharacterClassType.Archer:
                    return 32;
                case CharacterClassType.Magician:
                    return 20;
                case CharacterClassType.MartialArtist:
                    return 20;
                case CharacterClassType.Unknown:
                    return 10;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}