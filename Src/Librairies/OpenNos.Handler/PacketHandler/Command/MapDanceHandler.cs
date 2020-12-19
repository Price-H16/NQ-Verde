using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class MapDanceHandler : IPacketHandler
    {
        #region Instantiation

        public MapDanceHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void MapDance(MapDancePacket mapDancePacket)
        {
            Session.AddLogsCmd(mapDancePacket);
            if (Session.HasCurrentMapInstance)
            {
                Session.CurrentMapInstance.IsDancing = !Session.CurrentMapInstance.IsDancing;
                if (Session.CurrentMapInstance.IsDancing)
                {
                    Session.Character.Dance();
                    Session.CurrentMapInstance?.Broadcast("dance 2");
                }
                else
                {
                    Session.Character.Dance();
                    Session.CurrentMapInstance?.Broadcast("dance");
                }

                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
        }

        #endregion
    }
}