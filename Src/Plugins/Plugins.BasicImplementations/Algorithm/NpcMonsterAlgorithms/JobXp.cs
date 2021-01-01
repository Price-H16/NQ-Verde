using OpenNos.Domain;

namespace Plugins.BasicImplementations.Algorithm.NpcMonsterAlgorithms
{
    public class JobXp : IMonsterRaceStatAlgorithm
    {
        #region Members

        private const int MAX_LEVEL = 256;
        private int[] _stats;

        #endregion

        #region Methods

        public int GetStat(NpcMonsterRaceType type, byte level, bool isMonster)
        {
            return _stats[level];
        }

        public void Initialize()
        {
            _stats = new int[MAX_LEVEL];

            for (var i = 0; i < MAX_LEVEL; i++) _stats[i] = 120;
        }

        #endregion
    }
}