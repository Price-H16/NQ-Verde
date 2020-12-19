using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class ShutdownAllHandler : IPacketHandler
    {
        #region Instantiation

        public ShutdownAllHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void ShutdownAll(ShutdownAllPacket shutdownAllPacket)
        {
            if (shutdownAllPacket != null)
            {
                Session.AddLogsCmd(shutdownAllPacket);
                if (!string.IsNullOrEmpty(shutdownAllPacket.WorldGroup))
                    CommunicationServiceClient.Instance.Shutdown(shutdownAllPacket.WorldGroup);
                else
                    CommunicationServiceClient.Instance.Shutdown(ServerManager.Instance.ServerGroup);

                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ShutdownAllPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}