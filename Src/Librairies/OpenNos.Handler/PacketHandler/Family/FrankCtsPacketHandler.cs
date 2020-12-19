using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Family
{
    public class FrankCtsPacketHandler : IPacketHandler
    {
        #region Instantiation

        public FrankCtsPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void FamilyRank(FrankCtsPacket frankCtsPacket)
        {
            Session.SendPacket(UserInterfaceHelper.GenerateFrank(frankCtsPacket.Type, Session));
        }

        #endregion
    }
}