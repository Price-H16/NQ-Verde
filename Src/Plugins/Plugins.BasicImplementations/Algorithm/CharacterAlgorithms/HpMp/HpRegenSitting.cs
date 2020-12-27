using System;
using ChickenAPI.Enums.Game.Character;
using OpenNos.Domain;

namespace Plugins.BasicImplementations.Algorithm.CharacterAlgorithms.HpMp
{
    public class HpRegenSitting : ICharacterStatAlgorithm
    {
        public void Initialize()
        {
        }

        public int GetStat(CharacterClassType type, byte level)
        {
            switch (type)
            {
                case CharacterClassType.Adventurer:
                    return 30;
                case CharacterClassType.Swordsman:
                    return 90;
                case CharacterClassType.Archer:
                    return 60;
                case CharacterClassType.Magician:
                    return 30;
                case CharacterClassType.MartialArtist:
                    return 80;
                case CharacterClassType.Unknown:
                    return 50;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}