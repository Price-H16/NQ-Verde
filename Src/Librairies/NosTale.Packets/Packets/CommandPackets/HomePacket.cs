﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using ChickenAPI.Enums;
using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Home", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.Administrator})]
    public class HomePacket : PacketDefinition
    {
        #region Properties

        public static string ReturnHelp() => "$Home";

        #endregion
    }
}