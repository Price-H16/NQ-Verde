using OpenNos.Data;
using System.Collections.Generic;

namespace OpenNos.GameObject
{
    public class Card : CardDTO
    {
        #region Instantiation

        public Card()
        {
            //wtf is that
        }

        public Card(CardDTO input)
        {
            BuffType = input.BuffType;
            CardId = input.CardId;
            Delay = input.Delay;
            Duration = input.Duration;
            EffectId = input.EffectId;
            Level = input.Level;
            Name = input.Name;
            Propability = input.Propability;
            TimeoutBuff = input.TimeoutBuff;
            TimeoutBuffChance = input.TimeoutBuffChance;
        }

        #endregion

        #region Properties

        public List<BCard> BCards { get; set; }

        #endregion
    }
}