﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using ChickenAPI.Enums;
using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$MapDance", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.TGM})]
    public class MapDancePacket : PacketDefinition
    {
        public static string ReturnHelp() => "$MapDance";
    }
}