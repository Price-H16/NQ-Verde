﻿using ChickenAPI.Enums.Game.Character;
using OpenNos.Domain;

namespace Plugins.BasicImplementations.Algorithm.CharacterAlgorithms.Close
{
    public class CloseDodgeAlgorithm : ICharacterStatAlgorithm
    {
        private const int MAX_LEVEL = 256;
        private int[,] _stats;

        public void Initialize()
        {
            _stats = new int[(int) CharacterClassType.Unknown, MAX_LEVEL];


            for (var i = 0; i < MAX_LEVEL; i++)
            {
                _stats[(int) CharacterClassType.Adventurer, i] = i + 9; // approx
                _stats[(int) CharacterClassType.Swordsman, i] = i + 12; // approx
                _stats[(int) CharacterClassType.Magician, i] = i + 24; // approx
                _stats[(int) CharacterClassType.Archer, i] = i + 41; // approx
                _stats[(int) CharacterClassType.MartialArtist, i] = i + 12; // approx
            }
        }

        public int GetStat(CharacterClassType type, byte level) => _stats[(int) type, level - 1 > 0 ? level - 1 : 0];
    }
}