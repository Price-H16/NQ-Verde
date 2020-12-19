﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpenNos.Domain;

namespace OpenNos.DAL.EF
{
    public class Card
    {
        #region Instantiation

        public Card() => BCards = new HashSet<BCard>();

        #endregion

        #region Properties

        public virtual ICollection<BCard> BCards { get; set; }

        public BuffType BuffType { get; set; }

        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short CardId { get; set; }

        public int Delay { get; set; }

        public int Duration { get; set; }

        public int EffectId { get; set; }

        public byte Level { get; set; }

        [MaxLength(255)] public string Name { get; set; }

        public byte Propability { get; set; }

        public virtual ICollection<StaticBuff> StaticBuff { get; set; }

        public short TimeoutBuff { get; set; }

        public byte TimeoutBuffChance { get; set; }

        #endregion
    }
}