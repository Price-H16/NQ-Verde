﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Miniland", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.TGM})]
    public class MinilandPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)] public string CharacterName { get; set; }

        public static string ReturnHelp() => "$Miniland <?Nickname>";

        #endregion
    }
}