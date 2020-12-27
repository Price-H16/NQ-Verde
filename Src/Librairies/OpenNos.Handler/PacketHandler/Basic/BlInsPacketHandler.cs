using ChickenAPI.Enums;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Basic
{
    public class BlInsPacketHandler : IPacketHandler
    {
        #region Instantiation

        public BlInsPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void BlacklistAdd(BlInsPacket blInsPacket)
        {
            if (Session.Character.CharacterId == blInsPacket.CharacterId) return;

            if (DAOFactory.CharacterDAO.LoadById(blInsPacket.CharacterId) is CharacterDTO character
                && DAOFactory.AccountDAO.LoadById(character.AccountId).Authority >= AuthorityType.DSGM)
            {
                Session.SendPacket(
                    UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("CANT_BLACKLIST_TEAM")));
                return;
            }

            Session.Character.AddRelation(blInsPacket.CharacterId, CharacterRelationType.Blocked);
            Session.SendPacket(
                UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("BLACKLIST_ADDED")));
            Session.SendPacket(Session.Character.GenerateBlinit());
        }

        #endregion
    }
}