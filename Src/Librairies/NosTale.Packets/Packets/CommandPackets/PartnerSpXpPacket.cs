using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$PspXp", PassNonParseablePacket = true, Authorities = new[] { AuthorityType.Administrator })]
    public class PartnerSpXpPacket : PacketDefinition
    {
        #region Methods

        public static string ReturnHelp() => "$PspXp";

        #endregion
    }
}