using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class UserLogHandler : IPacketHandler
    {
        #region Instantiation

        public UserLogHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void UserLog(UserLogPacket userLogPacket)
        {
            if (userLogPacket == null) return;

            Session.AddLogsCmd(userLogPacket);
            var n = 1;

            foreach (var username in ClientSession.UserLog)
                Session.SendPacket(Session.Character.GenerateSay($"{n++}- {username}", 12));

            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
        }

        #endregion
    }
}