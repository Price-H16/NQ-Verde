﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;

namespace NosTale.Packets.Packets.ClientPackets
{
    [PacketHeader("blins")]
    public class BlInsPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)] public long CharacterId { get; set; }

        #endregion
    }
}