using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Basic
{
    public class NpinfoPacketHandler : IPacketHandler
    {
        #region Instantiation

        public NpinfoPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void GetStats(NpinfoPacket npinfoPacket)
        {
            Session.SendPackets(Session.Character.GenerateStatChar());

            if (npinfoPacket.Page != Session.Character.ScPage)
            {
                Session.Character.ScPage = npinfoPacket.Page;
                Session.SendPacket(UserInterfaceHelper.GeneratePClear());
                Session.SendPackets(Session.Character.GenerateScP(npinfoPacket.Page));
                Session.SendPackets(Session.Character.GenerateScN());
            }
        }

        #endregion
    }
}