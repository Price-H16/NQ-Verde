using System;
using OpenNos.GameObject.Battle;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject
{
    public class Buff
    {
        #region Members

        public int Level;

        #endregion

        #region Instantiation

        public Buff(short id, int level, bool isPermaBuff = false)
        {
            Card = ServerManager.GetCard(id);
            Level = level;
            IsPermaBuff = isPermaBuff;
        }

        #endregion

        #region Properties

        public Card Card { get; set; }

        public bool IsPermaBuff { get; set; }

        public int RemainingTime { get; set; }

        public BattleEntity Sender { get; set; }

        public short? SkillVNum { get; set; }

        public DateTime Start { get; set; }

        public bool StaticBuff { get; set; }

        public IDisposable StaticVisualEffect { get; set; }

        #endregion
    }
}