using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class ChangeSexHandler : IPacketHandler
    {
        #region Instantiation

        public ChangeSexHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void ChangeGender(ChangeSexPacket changeSexPacket)
        {
            Session.AddLogsCmd(changeSexPacket);
            Session.Character.ChangeSex();
        }

        #endregion
    }
}