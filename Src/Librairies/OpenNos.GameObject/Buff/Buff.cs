using System;
using OpenNos.GameObject.Battle;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject
{
    public class Buff
    {
        #region Members

        public int Level;

        public bool IsPermaBuff { get; set; }

        #endregion

        #region Instantiation

        public Buff(short id, int level, bool isPermaBuff = false, BattleEntity sender = null)
        {
            Card = ServerManager.GetCard(id);
            Level = level;
            IsPermaBuff = isPermaBuff;
            Sender = sender;
        }

        #endregion

        #region Properties

        public Card Card { get; set; }

        public int RemainingTime { get; set; }

        public DateTime Start { get; set; }

        public bool StaticBuff { get; set; }

        public IDisposable StaticVisualEffect { get; set; }

        public BattleEntity Sender { get; set; }

        public short? SkillVNum { get; set; }

        #endregion
    }
}