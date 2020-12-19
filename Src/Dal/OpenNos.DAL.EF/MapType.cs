﻿using System.Collections.Generic;

namespace OpenNos.DAL.EF
{
    public class MapType
    {
        #region Instantiation

        public MapType()
        {
            MapTypeMap = new HashSet<MapTypeMap>();
            Drops = new HashSet<Drop>();
        }

        #endregion

        #region Properties

        public virtual ICollection<Drop> Drops { get; set; }

        public short MapTypeId { get; set; }

        public virtual ICollection<MapTypeMap> MapTypeMap { get; set; }

        public string MapTypeName { get; set; }

        public short PotionDelay { get; set; }

        public virtual RespawnMapType RespawnMapType { get; set; }

        public long? RespawnMapTypeId { get; set; }

        public virtual RespawnMapType ReturnMapType { get; set; }

        public long? ReturnMapTypeId { get; set; }

        #endregion
    }
}