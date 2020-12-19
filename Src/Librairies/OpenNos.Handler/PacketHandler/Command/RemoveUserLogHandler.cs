using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class RemoveUserLogHandler : IPacketHandler
    {
        #region Instantiation

        public RemoveUserLogHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void RemoveUserLog(RemoveUserLogPacket removeUserLogPacket)
        {
            if (removeUserLogPacket == null
                || string.IsNullOrEmpty(removeUserLogPacket.Username))
                return;

            Session.AddLogsCmd(removeUserLogPacket);
            if (ClientSession.UserLog.Contains(removeUserLogPacket.Username))
                ClientSession.UserLog.RemoveAll(username => username == removeUserLogPacket.Username);

            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
        }

        #endregion
    }
}