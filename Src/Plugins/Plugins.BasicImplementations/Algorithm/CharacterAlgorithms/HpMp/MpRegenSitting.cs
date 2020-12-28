﻿using System;
using ChickenAPI.Enums.Game.Character;
using OpenNos.Domain;

namespace Plugins.BasicImplementations.Algorithm.CharacterAlgorithms.HpMp
{
    public class MpRegenSitting : ICharacterStatAlgorithm
    {
        public void Initialize()
        {
        }

        public int GetStat(CharacterClassType type, byte level)
        {
            switch (type)
            {
                case CharacterClassType.Adventurer:
                    return 10;
                case CharacterClassType.Swordsman:
                    return 30;
                case CharacterClassType.Archer:
                    return 50;
                case CharacterClassType.Magician:
                    return 80;
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