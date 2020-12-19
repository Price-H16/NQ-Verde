using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$Mute", PassNonParseablePacket = true, Authorities = new[] {AuthorityType.Supporter, AuthorityType.DSGM, AuthorityType.TGM})]
    public class MutePacket : PacketDefinition
    {
        #region Methods

        public static string ReturnHelp() => "$Mute <Nickname> <Duration> <Reason>";

        #endregion

        #region Properties

        [PacketIndex(0)] public string CharacterName { get; set; }

        [PacketIndex(1)] public int Duration { get; set; }

        [PacketIndex(2, SerializeToEnd = true)]
        public string Reason { get; set; }

        #endregion
    }
}