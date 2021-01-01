using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Warp", PassNonParseablePacket = true, Authorities = new[] { AuthorityType.User })]
    public class WarpPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)] public int MapId { get; set; }

        #endregion

        #region Methods

        public static string ReturnHelp() => "$Warp [MapId]";

        #endregion
    }
}