﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using ChickenAPI.Enums;
using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$BlockFExp", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.Administrator})]
    public class BlockFExpPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)] public string CharacterName { get; set; }

        [PacketIndex(1)] public int Duration { get; set; }

        [PacketIndex(2, SerializeToEnd = true)]
        public string Reason { get; set; }

        public static string ReturnHelp() => "$BlockFExp <Nickname> <Duration> <Reason>";

        #endregion
    }
}