using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class LogUserPacketsHandler : IPacketHandler
    {
        #region Instantiation

        public LogUserPacketsHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        private ClientSession Session { get; }

        #endregion

        public void LogUserPackets(LogUserPacketsPacket packet)
        {
            
        }
        
    }
}