using System;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.DAL.EF
{
    public class MaintenanceLog
    {
        #region Properties

        public DateTime DateEnd { get; set; }

        public DateTime DateStart { get; set; }

        [Key] public long LogId { get; set; }

        [MaxLength(255)] public string Reason { get; set; }

        #endregion
    }
}