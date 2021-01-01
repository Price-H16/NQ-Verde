using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Quest", PassNonParseablePacket = true, Authorities = new[] { AuthorityType.Administrator })]
    public class AddQuestPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)] public short QuestId { get; set; }

        #endregion

        #region Methods

        public static string ReturnHelp() => "$Quest <QuestId>";

        #endregion
    }
}