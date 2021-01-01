using OpenNos.Domain;

namespace Plugins.BasicImplementations.Algorithm.NpcMonsterAlgorithms
{
    public class HeroXp : IMonsterRaceStatAlgorithm
    {
        #region Members

        private const int MAX_LEVEL = 256;

        #endregion

        #region Methods

        public int GetStat(NpcMonsterRaceType type, byte level, bool isMonster) => 0;

        public void Initialize()
        {
        }

        #endregion
    }
}