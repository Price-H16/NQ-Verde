using OpenNos.Domain;

namespace Plugins.BasicImplementations.Algorithm.NpcMonsterAlgorithms
{
    public interface IMonsterRaceStatAlgorithm
    {
        #region Methods

        int GetStat(NpcMonsterRaceType type, byte level, bool isMonster);

        void Initialize();

        #endregion
    }
}