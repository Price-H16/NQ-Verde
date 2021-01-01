using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$LogUserPackets", PassNonParseablePacket = true, Authorities = new[] { AuthorityType.Administrator })]
    public class LogUserPacketsPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)] public string CharacterName { get; set; }

        #endregion

        #region Methods

        public static string ReturnHelp() => "$LogUserPackets <Nickname>";

        #endregion
    }
}