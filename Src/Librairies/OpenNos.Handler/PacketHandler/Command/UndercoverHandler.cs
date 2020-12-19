using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class UndercoverHandler : IPacketHandler
    {
        #region Instantiation

        public UndercoverHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Undercover(UndercoverPacket undercoverPacket)
        {
            Session.AddLogsCmd(undercoverPacket);
            Session.Character.Undercover = !Session.Character.Undercover;
            ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId,
                Session.CurrentMapInstance.MapInstanceId, Session.Character.PositionX, Session.Character.PositionY);
        }

        #endregion
    }
}