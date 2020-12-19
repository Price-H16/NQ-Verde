using System.Linq;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Basic
{
    public class FDelPacketHandler : IPacketHandler
    {
        #region Instantiation

        public FDelPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void FriendDelete(FDelPacket fDelPacket)
        {
            if (Session.Character.CharacterRelations.Any(s =>
                s.RelatedCharacterId == fDelPacket.CharacterId && s.RelationType == CharacterRelationType.Spouse))
            {
                Session.SendPacket($"info {Language.Instance.GetMessageFromKey("CANT_DELETE_COUPLE")}");
                return;
            }
            Session.Character.DeleteRelation(fDelPacket.CharacterId, CharacterRelationType.Friend);
            Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("FRIEND_DELETED")));
        }

        #endregion
    }
}