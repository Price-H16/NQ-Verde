﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using ChickenAPI.Enums;
using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Upgrade", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.Administrator})]
    public class UpgradeCommandPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)] public short Slot { get; set; }

        [PacketIndex(1)] public UpgradeMode Mode { get; set; }

        [PacketIndex(2)] public UpgradeProtection Protection { get; set; }

        public static string ReturnHelp() => "$Upgrade <Slot> <Mode> <Protection>";

        #endregion
    }
}