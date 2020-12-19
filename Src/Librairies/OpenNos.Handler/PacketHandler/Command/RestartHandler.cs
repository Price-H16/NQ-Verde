using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class RestartHandler : IPacketHandler
    {
        #region Instantiation

        public RestartHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Restart(RestartPacket restartPacket)
        {
            var time = restartPacket.Time > 0 ? restartPacket.Time : 1;

            Session.AddLogsCmd(restartPacket);
            if (ServerManager.Instance.TaskShutdown != null)
            {
                ServerManager.Instance.ShutdownStop = true;
                ServerManager.Instance.TaskShutdown = null;
            }
            else
            {
                ServerManager.Instance.IsReboot = true;
                ServerManager.Instance.TaskShutdown = ServerManager.Instance.ShutdownTaskAsync(time);
            }

            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
        }

        #endregion
    }
}