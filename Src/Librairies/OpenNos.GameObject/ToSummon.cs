using OpenNos.GameObject.Battle;
using System.Collections.Concurrent;

namespace OpenNos.GameObject
{
    public class ToSummon
    {
        #region Instantiation

        public ToSummon(short vnum, MapCell spawnCell, IBattleEntity target, bool move, byte summonChance = 100,
            bool isTarget = false, bool isBonusOrProtected = false, bool isHostile = true, bool isBossOrMate = false)
        {
            VNum = vnum;
            SpawnCell = spawnCell;
            Target = target;
            IsTarget = isTarget;
            IsMoving = move;
            IsBonusOrProtected = isBonusOrProtected;
            IsBossOrMate = isBossOrMate;
            IsHostile = isHostile;
            SummonChance = (byte)(summonChance == 0 ? 100 : summonChance);
            DeathEvents = new ConcurrentBag<EventContainer>();
            NoticingEvents = new ConcurrentBag<EventContainer>();
        }

        #endregion

        #region Properties

        public byte SummonChance { get; set; }

        public ConcurrentBag<EventContainer> DeathEvents { get; set; }

        public ConcurrentBag<EventContainer> NoticingEvents { get; set; }

        public bool IsBonusOrProtected { get; set; }

        public bool IsBossOrMate { get; set; }

        public bool IsHostile { get; set; }

        public bool IsProtected { get; }

        public bool IsMoving { get; set; }

        public bool IsTarget { get; set; }

        public MapCell SpawnCell { get; set; }

        public IBattleEntity Target { get; set; }

        public short VNum { get; set; }

        public byte NoticeRange { get; set; }

        #endregion
    }
}
