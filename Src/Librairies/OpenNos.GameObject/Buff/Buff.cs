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

        public Buff(short id, int level, bool isPermaBuff = false)
        {
            Card = ServerManager.GetCard(id);
            Level = level;
            IsPermaBuff = isPermaBuff;
        }

        #endregion

        #region Properties

        public Card Card { get; set; }

<<<<<<< HEAD
        public int Level { get; set; }

        public bool IsStaticBuff { get; }
        public bool IsPermaBuff { get; }
        public bool StaticBuff { get; set; }
=======
        public int RemainingTime { get; set; }
>>>>>>> parent of d7ef289... Bcard Cleaning

        public DateTime Start { get; set; }

        public bool StaticBuff { get; set; }

        public IDisposable StaticVisualEffect { get; set; }

        public BattleEntity Sender { get; set; }

        public short? SkillVNum { get; set; }

        #endregion
    }
}