using System.Threading;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Bazaar
{
    public class CBListPacketHandler : IPacketHandler
    {
        #region Instantiation

        public CBListPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void RefreshBazarList(CBListPacket cbListPacket)
        {
            if (!Session.Character.CanUseNosBazaar())
            {
                Session.SendPacket(
                    UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("INFO_BAZAAR")));
                return;
            }

            SpinWait.SpinUntil(() => !ServerManager.Instance.InBazaarRefreshMode);
            Session.SendPacket(UserInterfaceHelper.GenerateRCBList(cbListPacket));
        }

        #endregion
    }
}