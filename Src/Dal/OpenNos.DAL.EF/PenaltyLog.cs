﻿using System;
using System.ComponentModel.DataAnnotations;
using OpenNos.Domain;

namespace OpenNos.DAL.EF
{
    public class PenaltyLog
    {
        #region Properties

        public virtual Account Account { get; set; }

        public long AccountId { get; set; }

        public string AdminName { get; set; }

        public DateTime DateEnd { get; set; }

        public DateTime DateStart { get; set; }

        public string IP { get; set; }

        public PenaltyType Penalty { get; set; }

        [Key] public int PenaltyLogId { get; set; }

        [MaxLength(255)] public string Reason { get; set; }

        #endregion
    }
}