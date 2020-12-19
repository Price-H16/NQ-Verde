using System.Linq;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class PartnerSpXpHandler : IPacketHandler
    {
        #region Instantiation

        public PartnerSpXpHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void PartnerSpXp(PartnerSpXpPacket partnerSpXpPacket)
        {
            if (partnerSpXpPacket == null) return;
            Session.AddLogsCmd(partnerSpXpPacket);
            var mate = Session.Character.Mates?.ToList()
                .FirstOrDefault(s => s.IsTeamMember && s.MateType == MateType.Partner);

            if (mate?.Sp != null)
            {
                mate.Sp.FullXp();
                Session.SendPacket(mate.GenerateScPacket());
            }
        }

        #endregion
    }
}