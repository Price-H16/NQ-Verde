﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$HairColor", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.TGM})]
    public class HairColorPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)] public HairColorType HairColor { get; set; }

        public static string ReturnHelp() => "$HairColor <Value>";

        #endregion
    }
}