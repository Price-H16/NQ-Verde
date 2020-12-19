using NosTale.Extension.GameExtension.Character;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using System;

namespace OpenNos.Handler.BasicPacket.CharScreen
{
    internal class CreateCharacterPacketHandler : IPacketHandler
    {
        #region Instantiation

        public CreateCharacterPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        private ClientSession Session { get; }

        #endregion

        #region Methods

        public void CreateCharacter(CharacterCreatePacket characterCreatePacket)
        {
            if (Session.HasCurrentMapInstance) return;

            var slot = characterCreatePacket.Slot;
            var characterName = characterCreatePacket.Name;

            if (!Session.CanCreateCharacter(slot, characterName)) return;

            Session.CreateCharacter(characterCreatePacket.Gender, characterCreatePacket.HairColor,
                characterCreatePacket.HairStyle, characterCreatePacket.Name,
                characterCreatePacket.Slot, false);

            new EntryPointPacketHandler(Session).LoadCharacters(new OpenNosEntryPointPacket
                {PacketData = characterCreatePacket.OriginalContent});
        }

        #endregion
    }
}