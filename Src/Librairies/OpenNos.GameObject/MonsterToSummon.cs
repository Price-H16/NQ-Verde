﻿using System.Collections.Generic;
using OpenNos.GameObject.Battle;

namespace OpenNos.GameObject
{
    public class MonsterToSummon
    {
        #region Instantiation

        public MonsterToSummon(short vnum, MapCell spawnCell, BattleEntity target, bool move, bool isTarget = false,
            bool isBonus = false, bool isHostile = true, bool isBoss = false, BattleEntity owner = null,
            int aliveTime = 0, int aliveTimeMp = 0, byte noticeRange = 0, float hasDelay = 0, int maxHp = 0,
            int maxMp = 0)
        {
            VNum = vnum;
            SpawnCell = spawnCell;
            Target = target;
            IsMoving = move;
            IsTarget = isTarget;
            IsBonus = isBonus;
            IsBoss = isBoss;
            IsHostile = isHostile;
            DeathEvents = new List<EventContainer>();
            NoticingEvents = new List<EventContainer>();
            UseSkillOnDamage = new List<UseSkillOnDamage>();
            SpawnEvents = new List<EventContainer>();
            AfterSpawnEvents = new List<EventContainer>();
            Owner = owner;
            AliveTime = aliveTime;
            AliveTimeMp = aliveTimeMp;
            NoticeRange = noticeRange;
            HasDelay = hasDelay;
            MaxHp = maxHp;
            MaxMp = maxMp;
        }

        #endregion

        #region Properties

        public List<EventContainer> AfterSpawnEvents { get; set; }

        public int AliveTime { get; set; }

        public int AliveTimeMp { get; set; }

        public short Damage { get; set; }

        public List<EventContainer> DeathEvents { get; set; }

        public float HasDelay { get; set; }

        public bool IsBonus { get; set; }

        public bool IsBoss { get; set; }

        public bool IsHostile { get; set; }

        public bool IsMeteorite { get; set; }

        public bool IsMoving { get; set; }

        public bool IsTarget { get; set; }

        public bool IsVessel { get; set; }

        public int MaxHp { get; set; }

        public int MaxMp { get; set; }

        public byte NoticeRange { get; internal set; }

        public List<EventContainer> NoticingEvents { get; set; }

        public BattleEntity Owner { get; set; }

        public MapCell SpawnCell { get; set; }

        public List<EventContainer> SpawnEvents { get; set; }

        public BattleEntity Target { get; set; }

        public List<UseSkillOnDamage> UseSkillOnDamage { get; set; }

        public short VNum { get; set; }

        #endregion
    }
}