﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using ChickenAPI.Enums;
using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$ArenaWinner", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.Administrator})]
    public class ArenaWinnerPacket : PacketDefinition
    {
        public static string ReturnHelp() => "$ArenaWinner";
    }
}