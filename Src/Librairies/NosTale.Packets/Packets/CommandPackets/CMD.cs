﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$CMD", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.User})]
    public class CMDPacket : PacketDefinition
    {
        [PacketIndex(0, SerializeToEnd = true)]

        public static string Contents { get; set; }
    }
}