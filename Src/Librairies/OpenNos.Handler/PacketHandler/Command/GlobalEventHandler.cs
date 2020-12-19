using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.Master.Library.Client;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class GlobalEventHandler : IPacketHandler
    {
        #region Instantiation

        public GlobalEventHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void StartGlobalEvent(GlobalEventPacket globalEventPacket)
        {
            if (globalEventPacket != null)
            {
                Session.AddLogsCmd(globalEventPacket);
                CommunicationServiceClient.Instance.RunGlobalEvent(globalEventPacket.EventType);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(EventPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}