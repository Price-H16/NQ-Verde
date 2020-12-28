﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Portal", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.GM})]
    public class PortalToPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)] public short DestinationMapId { get; set; }

        [PacketIndex(1)] public short DestinationX { get; set; }

        [PacketIndex(2)] public short DestinationY { get; set; }

        [PacketIndex(3)] public PortalType? PortalType { get; set; }

        public static string ReturnHelp() => "$Portal <ToMapId> <ToX> <ToY> <?PortalType>";

        #endregion
    }
}