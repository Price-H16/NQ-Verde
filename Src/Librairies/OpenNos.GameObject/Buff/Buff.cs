﻿using System;
using OpenNos.Data;
using OpenNos.GameObject.Battle;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject
{
    public class Buff
    {
        #region Instantiation

        public Buff(short id, int level = 0, bool isPermaBuff = false, bool isStaticBuff = false)
        {
            Card = ServerManager.GetCard(id);
            Level = level;
            IsPermaBuff = isPermaBuff;
            IsStaticBuff = isStaticBuff;
            Start = DateTime.Now;
        }

        #endregion

        #region Properties

        public Card Card { get; }

        public int Level { get; set; }

        public bool IsStaticBuff { get; }
        public bool IsPermaBuff { get; }
        public bool StaticBuff { get; set; }

        public DateTime Start { get; set; }

        public int RemainingTime { get; set; }
        
        public IDisposable StaticVisualEffect { get; set; }

        public BattleEntity Sender { get; set; }

        public short? SkillVNum { get; set; }

        #endregion
    }
}