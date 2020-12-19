using System.Threading;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Bazaar
{
    public class CSListPacketHandler : IPacketHandler
    {
        #region Instantiation

        public CSListPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void RefreshPersonalBazarList(CSListPacket csListPacket)
        {
            if (ServerManager.Instance.InShutdown)
            {
                return;
            }

            if (Session.Character.IsMuted())
            {
                Session.SendPacket(UserInterfaceHelper.GenerateMsg("You are sanctioned you cannot do this", 0));
                return;
            }
            if (!Session.Character.CanUseNosBazaar())
            {
                Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("INFO_BAZAAR")));
                return;
            }

            SpinWait.SpinUntil(() => !ServerManager.Instance.InBazaarRefreshMode);
            Session.SendPacket(Session.Character.GenerateRCSList(csListPacket));
        }

        #endregion
    }
}