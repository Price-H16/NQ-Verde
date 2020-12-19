﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Maintenance", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.Administrator})]
    public class MaintenancePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)] public int Delay { get; set; }

        [PacketIndex(1)] public int Duration { get; set; }

        [PacketIndex(2, SerializeToEnd = true)]
        public string Reason { get; set; }

        public static string ReturnHelp() => "$Maintenance <Delay> <Duration> <Reason>";

        #endregion
    }
}