using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.GameObject;

namespace OpenNos.Handler.BasicPacket.CharScreen
{
    public class CharRenPacketHandler : IPacketHandler
    {
        #region Instantiation

        public CharRenPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        private ClientSession Session { get; }

        #endregion

        #region Methods

        public void CharRename(CharacterRenamePacket e)
        {
            if (Session.HasCurrentMapInstance) return;

            if (e == null) return;

            var character =
                DAOFactory.CharacterDAO.LoadBySlot(Session.Account.AccountId, e.Slot);
            if (character == null) return;

            if (!character.IsChangeName) return;

            if (e.Slot > 3) return;

            if (e.Name.Length <= 3 || e.Name.Length >= 15) return;

            var rg = new Regex(
                @"^[\u0021-\u007E\u00A1-\u00AC\u00AE-\u00FF\u4E00-\u9FA5\u0E01-\u0E3A\u0E3F-\u0E5B\u002E]*$");
            if (rg.Matches(e.Name).Count != 1)
            {
                Session.SendPacketFormat($"info {Language.Instance.GetMessageFromKey("INVALID_CHARNAME")}");
                return;
            }

            if (DAOFactory.CharacterDAO.LoadByName(e.Name) != null)
                //&& character.State != CharacterState.Inactive)
            {
                Session.SendPacketFormat($"info {Language.Instance.GetMessageFromKey("ALREADY_TAKEN")}");
                return;
            }

            var BlackListed = new List<string>
            {
                "[",
                "]",
                "[gm]",
                "[nh]"
            };

            if (BlackListed.Any(s => e.Name.ToLower().Contains(s)))
            {
                Session.SendPacketFormat($"info {Language.Instance.GetMessageFromKey("INVALID_CHARNAME")}");
                return;
            }

            character.IsChangeName = !character.IsChangeName;
            character.Name = e.Name;

            DAOFactory.CharacterDAO.InsertOrUpdate(ref character);

            new EntryPointPacketHandler(Session).LoadCharacters(new OpenNosEntryPointPacket
                {PacketData = string.Empty});
        }

        #endregion
    }
}