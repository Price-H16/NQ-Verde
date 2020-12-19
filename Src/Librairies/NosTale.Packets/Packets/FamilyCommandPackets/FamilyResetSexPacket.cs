﻿using OpenNos.Core;

namespace NosTale.Packets.Packets.FamilyCommandPackets
{
    [PacketHeader("%Género", "%Sex", "%Sexe")]
    public class FamilyResetSexPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0, serializeToEnd: true)] public string Data { get; set; }

        #endregion
    }
}