﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using ChickenAPI.Enums;
using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$WigColor", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.TGM, AuthorityType.DSGM})]
    public class WigColorPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)] public byte Color { get; set; }

        public static string ReturnHelp() => "$WigColor <Value>";

        #endregion
    }
}