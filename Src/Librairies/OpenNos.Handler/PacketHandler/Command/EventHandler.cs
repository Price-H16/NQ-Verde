using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class EventHandler : IPacketHandler
    {
        #region Instantiation

        public EventHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void StartEvent(EventPacket eventPacket)
        {
            if (eventPacket != null)
            {
                Session.AddLogsCmd(eventPacket);
                if (eventPacket.LvlBracket >= 0)
                    EventHelper.GenerateEvent(eventPacket.EventType, eventPacket.LvlBracket);
                else
                    EventHelper.GenerateEvent(eventPacket.EventType);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(EventPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}