using OpenNos.Domain;
using System;

namespace Plugins.BasicImplementations.Algorithm.CharacterAlgorithms.HpMp
{
    public class MpRegen : ICharacterStatAlgorithm
    {
        #region Methods

        public int GetStat(ClassType type, byte level)
        {
            switch (type)
            {
                case ClassType.Adventurer:
                    return 5;

                case ClassType.Swordsman:
                    return 16;

                case ClassType.Archer:
                    return 28;

                case ClassType.Magician:
                    return 40;

                case ClassType.MartialArtist:
                    return 50;

                case ClassType.Unknown:
                    return 50;

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