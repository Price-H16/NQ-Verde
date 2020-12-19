﻿using OpenNos.Core;

namespace NosTale.Packets.Packets.FamilyCommandPackets
{
    [PacketHeader("%Hoy", "%Today", "%Aujourd'hui")]
    public class FamilyTodayPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0, serializeToEnd: true)] public string Data { get; set; }

        #endregion
    }
}