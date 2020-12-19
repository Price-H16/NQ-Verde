﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$GoldRate", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.Administrator})]
    public class GoldRatePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)] public int Value { get; set; }

        public static string ReturnHelp() => "$GoldRate <Value>";

        #endregion
    }
}