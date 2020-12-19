using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class AddUserLogHandler : IPacketHandler
    {
        #region Instantiation

        public AddUserLogHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void AddUserLog(AddUserLogPacket addUserLogPacket)
        {
            if (addUserLogPacket == null
                || string.IsNullOrEmpty(addUserLogPacket.Username))
                return;

            Session.AddLogsCmd(addUserLogPacket);
            ClientSession.UserLog.Add(addUserLogPacket.Username);

            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
        }

        #endregion
    }
}