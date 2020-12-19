using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class ArenaWinnerHandler : IPacketHandler
    {
        #region Instantiation

        public ArenaWinnerHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void ArenaWinner(ArenaWinnerPacket arenaWinner)
        {
            Session.AddLogsCmd(arenaWinner);
            Session.Character.ArenaWinner = Session.Character.ArenaWinner == 0 ? 1 : 0;
            Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateCMode());
            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
        }

        #endregion
    }
}