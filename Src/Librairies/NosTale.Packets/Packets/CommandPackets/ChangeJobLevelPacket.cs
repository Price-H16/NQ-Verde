﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using ChickenAPI.Enums;
using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$JLvl", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.Administrator, AuthorityType.BetaTester})]
    public class ChangeJobLevelPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)] public byte JobLevel { get; set; }

        public static string ReturnHelp() => "$JLvl <Value>";

        #endregion
    }
}