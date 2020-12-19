using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.GameObject;

namespace OpenNos.Handler.PacketHandler.Family
{
    public class TodayPacketHandler : IPacketHandler
    {
        #region Instantiation

        public TodayPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void FamilyChangeMessage(TodayPacket todayPacket)
        {
            Session.SendPacket("today_stc");
        }

        #endregion
    }
}