using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$GiveMall", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.Administrator})]
    public class GiveMallPacket : PacketDefinition
    {
        #region Methods

        public static string ReturnHelp() => "$GiveMall <Amount> <Nickname>";

        #endregion

        #region Properties

        [PacketIndex(0)] public short Amount { get; set; }

        [PacketIndex(1)] public string CharacterName { get; set; }

        #endregion
    }
}