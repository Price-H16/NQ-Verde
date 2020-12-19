﻿using System;
using OpenNos.Domain;

namespace OpenNos.Data
{
    [Serializable]
    public class ItemInstanceDTO : SynchronizableBaseDTO
    {
        #region Properties
        public ItemDTO Item { get; set; }
        public byte Ammo { get; set; }

        public short Amount { get; set; }

        public long? BoundCharacterId { get; set; }

        public byte Cellon { get; set; }

        public long CharacterId { get; set; }

        public short CloseDefence { get; set; }

        public short Concentrate { get; set; }

        public short CriticalDodge { get; set; }

        public byte CriticalLuckRate { get; set; }

        public short CriticalRate { get; set; }

        public short DamageMaximum { get; set; }

        public short DamageMinimum { get; set; }

        public byte DarkElement { get; set; }

        public short DarkResistance { get; set; }

        public short DefenceDodge { get; set; }

        public short Design { get; set; }

        public short DistanceDefence { get; set; }

        public short DistanceDefenceDodge { get; set; }

        public int DurabilityPoint { get; set; }

        public short ElementRate { get; set; }

        public Guid EquipmentSerialId { get; set; }

        public byte FireElement { get; set; }

        public short FireResistance { get; set; }

        public short FusionVnum { get; set; }

        public short HitRate { get; set; }

        public bool HasSkin { get; set; }

        public short HoldingVNum { get; set; }

        public short HP { get; set; }

        public bool IsBreaked { get; set; }

        public bool IsEmpty { get; set; }

        public bool IsFixed { get; set; }

        public bool IsPartnerEquipment { get; set; }

        public DateTime? ItemDeleteTime { get; set; }

        public short ItemVNum { get; set; }

        public byte LightElement { get; set; }

        public short LightResistance { get; set; }

        public short MagicDefence { get; set; }

        public short MaxElementRate { get; set; }

        public short MP { get; set; }

        public sbyte Rare { get; set; }

        public byte RuneUpgrade { get; set; }

        public bool RuneBroke { get; set; }

        public byte RuneCount { get; set; }

        public short? ShellRarity { get; set; }

        public short SlDamage { get; set; }

        public short SlDefence { get; set; }

        public short SlElement { get; set; }

        public short SlHP { get; set; }

        public short Slot { get; set; }

        public byte SpDamage { get; set; }

        public byte SpDark { get; set; }

        public byte SpDefence { get; set; }

        public byte SpElement { get; set; }

        public byte SpFire { get; set; }

        public byte SpHP { get; set; }

        public byte SpLevel { get; set; }

        public byte SpLight { get; set; }

        public byte SpStoneUpgrade { get; set; }

        public byte SpWater { get; set; }

        public InventoryType Type { get; set; }

        public byte Upgrade { get; set; }

        public byte WaterElement { get; set; }

        public short WaterResistance { get; set; }

        public long XP { get; set; }
        

        #endregion
    }
}