using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class ShoutHandler : IPacketHandler
    {
        #region Instantiation

        public ShoutHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Shout(ShoutPacket shoutPacket)
        {
            if (shoutPacket != null)
            {
                Session.AddLogsCmd(shoutPacket);
                CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                {
                    DestinationCharacterId = null,
                    SourceCharacterId = Session.Character.CharacterId,
                    SourceWorldId = ServerManager.Instance.WorldId,
                    Message = shoutPacket.Message,
                    Type = MessageType.Shout
                });
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ShoutPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}