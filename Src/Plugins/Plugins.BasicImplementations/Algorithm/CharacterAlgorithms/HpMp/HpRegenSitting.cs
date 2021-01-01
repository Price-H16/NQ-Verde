using OpenNos.Domain;
using System;

namespace Plugins.BasicImplementations.Algorithm.CharacterAlgorithms.HpMp
{
    public class HpRegenSitting : ICharacterStatAlgorithm
    {
        #region Methods

        public int GetStat(ClassType type, byte level)
        {
            switch (type)
            {
                case ClassType.Adventurer:
                    return 30;

                case ClassType.Swordsman:
                    return 90;

                case ClassType.Archer:
                    return 60;

                case ClassType.Magician:
                    return 30;

                case ClassType.MartialArtist:
                    return 80;

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