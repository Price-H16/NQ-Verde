using OpenNos.Core;
using OpenNos.Domain;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$KickAccSave", PassNonParseablePacket = true, Authorities = new[] { AuthorityType.DSGM, AuthorityType.Administrator })]
    public class KickAccountSaveListPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int AccountId { get; set; }

        #endregion

        #region Methods

        public static string ReturnHelp() => "$KickAccSave [AccountID]";

        #endregion

        // kekw
    }
}