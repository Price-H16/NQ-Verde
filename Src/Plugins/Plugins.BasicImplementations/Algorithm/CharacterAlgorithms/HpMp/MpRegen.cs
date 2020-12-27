using System;
using ChickenAPI.Enums.Game.Character;
using OpenNos.Domain;

namespace Plugins.BasicImplementations.Algorithm.CharacterAlgorithms.HpMp
{
    public class MpRegen : ICharacterStatAlgorithm
    {
        public void Initialize()
        {
        }

        public int GetStat(CharacterClassType type, byte level)
        {
            switch (type)
            {
                case CharacterClassType.Adventurer:
                    return 5;
                case CharacterClassType.Swordsman:
                    return 16;
                case CharacterClassType.Archer:
                    return 28;
                case CharacterClassType.Magician:
                    return 40;
                case CharacterClassType.MartialArtist:
                    return 50;
                case CharacterClassType.Unknown:
                    return 50;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}