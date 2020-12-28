using OpenNos.Core;
using OpenNos.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChickenAPI.Enums;

namespace NosTale.Packets.Packets.CommandPackets
{
    [PacketHeader("$KickAccSave", PassNonParseablePacket = true, Authorities = new[] { AuthorityType.DSGM, AuthorityType.Administrator })]
    public class KickAccountSaveListPacket : PacketDefinition
    {

        #region Properties

        [PacketIndex(0)]
        public int AccountId { get; set; }

        public static string ReturnHelp() => "$KickAccSave [AccountID]"; // kekw

            #endregion
        
    }
}
