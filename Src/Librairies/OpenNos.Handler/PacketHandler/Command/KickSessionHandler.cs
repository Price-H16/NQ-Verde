using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.Master.Library.Client;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class KickSessionHandler : IPacketHandler
    {
        #region Instantiation

        public KickSessionHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void KickSession(KickSessionPacket kickSessionPacket)
        {
            if (kickSessionPacket != null)
            {
                Session.AddLogsCmd(kickSessionPacket);
                if (kickSessionPacket.SessionId.HasValue) //if you set the sessionId, remove account verification
                    kickSessionPacket.AccountName = "";

                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
                var account = DAOFactory.AccountDAO.LoadByName(kickSessionPacket.AccountName);
                CommunicationServiceClient.Instance.KickSession(account?.AccountId, kickSessionPacket.SessionId);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(KickSessionPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}