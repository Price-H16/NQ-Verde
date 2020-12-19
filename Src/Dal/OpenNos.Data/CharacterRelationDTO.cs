﻿using System;
using OpenNos.Domain;

namespace OpenNos.Data
{
    [Serializable]
    public class CharacterRelationDTO
    {
        #region Properties

        public long CharacterId { get; set; }

        public long CharacterRelationId { get; set; }

        public long RelatedCharacterId { get; set; }

        public CharacterRelationType RelationType { get; set; }

        #endregion
    }
}