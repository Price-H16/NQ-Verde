﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;

namespace NosTale.Packets.Packets.ServerPackets
{
    [PacketHeader("remove")]
    public class RemovePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)] public byte EquipSlot { get; set; }

        [PacketIndex(1)] public long MateId { get; set; }

        #endregion
    }
}