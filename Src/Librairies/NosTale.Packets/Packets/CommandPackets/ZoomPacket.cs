﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using ChickenAPI.Enums;
using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Zoom", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.Administrator})]
    public class ZoomPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)] public byte Value { get; set; }

        public static string ReturnHelp()
        {
            return "$Zoom <Value>";
        }

        #endregion
    }
}