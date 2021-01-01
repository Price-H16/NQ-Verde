using OpenNos.Domain;
using System;

namespace Plugins.BasicImplementations.Algorithm.CharacterAlgorithms.HpMp
{
    public class HpRegen : ICharacterStatAlgorithm
    {
        #region Methods

        public int GetStat(ClassType type, byte level)
        {
            switch (type)
            {
                case ClassType.Adventurer:
                    return 25;

                case ClassType.Swordsman:
                    return 26;

                case ClassType.Archer:
                    return 32;

                case ClassType.Magician:
                    return 20;

                case ClassType.MartialArtist:
                    return 20;

                case ClassType.Unknown:
                    return 10;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void Initialize()
        {
        }

        #endregion
    }
}