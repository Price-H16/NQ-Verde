﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Speed", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.Administrator})]
    public class SpeedPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)] public byte Value { get; set; }

        public static string ReturnHelp() => "$Speed <Value>";

        #endregion
    }
}