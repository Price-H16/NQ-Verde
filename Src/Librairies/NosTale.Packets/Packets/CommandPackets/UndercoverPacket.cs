﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Undercover", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.TGM})]
    public class UndercoverPacket : PacketDefinition
    {
        public static string ReturnHelp() => "$Undercover";
    }
}