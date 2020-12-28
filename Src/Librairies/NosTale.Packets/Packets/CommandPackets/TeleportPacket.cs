using ChickenAPI.Enums;
using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Teleport", "$tp", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.TGM})]
    public class TeleportPacket : PacketDefinition
    {
        #region Methods

        public static string ReturnHelp() => "$Teleport <Nickname|ToMapId> <?ToX> <?ToY>";

        #endregion

        #region Properties

        [PacketIndex(0)] public string Data { get; set; }

        [PacketIndex(1)] public short X { get; set; }

        [PacketIndex(2)] public short Y { get; set; }

        #endregion
    }
}