﻿using OpenNos.Core;

namespace NosTale.Packets.Packets.FamilyCommandPackets
{
    [PacketHeader("%Aviso", "%Notice", "%Avertissement")]
    public class FamilyMessagePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0, serializeToEnd: true)] public string Data { get; set; }

        #endregion
    }
}