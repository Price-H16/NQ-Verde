using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Basic
{
    public class BlDelPacketHandler : IPacketHandler
    {
        #region Instantiation

        public BlDelPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void BlacklistDelete(BlDelPacket blDelPacket)
        {
            Session.Character.DeleteBlackList(blDelPacket.CharacterId);
            Session.SendPacket(Session.Character.GenerateBlinit());
            Session.SendPacket(
                UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("BLACKLIST_DELETED")));
        }

        #endregion
    }
}