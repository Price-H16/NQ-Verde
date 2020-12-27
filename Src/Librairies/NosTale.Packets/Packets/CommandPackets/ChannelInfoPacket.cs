﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using ChickenAPI.Enums;
using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$ChannelInfo", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.TGM})]
    public class ChannelInfoPacket : PacketDefinition
    {
        public static string ReturnHelp() => "$ChannelInfo";
    }
}