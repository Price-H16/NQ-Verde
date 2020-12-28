﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using ChickenAPI.Enums;
using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Clear", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.Administrator})]
    public class ClearInventoryPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)] public InventoryType InventoryType { get; set; }

        public static string ReturnHelp() => "$Clear <InventoryType>";

        #endregion
    }
}