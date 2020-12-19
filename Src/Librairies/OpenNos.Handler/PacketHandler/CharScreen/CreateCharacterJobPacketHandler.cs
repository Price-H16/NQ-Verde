using NosTale.Extension.GameExtension.Character;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.GameObject;

namespace OpenNos.Handler.BasicPacket.CharScreen
{
    internal class CreateCharacterJobPacketHandler : IPacketHandler
    {
        #region Instantiation

        public CreateCharacterJobPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        private ClientSession Session { get; }

        #endregion

        #region Methods

        public void CreateCharacterJob(CharacterJobCreatePacket characterCreatePacket)
        {
            if (Session.HasCurrentMapInstance) return;

            var slot = characterCreatePacket.Slot;
            var characterName = characterCreatePacket.Name;

            if (!Session.CanCreateCharacter(slot, characterName)) return;

            Session.CreateCharacter(characterCreatePacket.Gender, characterCreatePacket.HairColor,
                characterCreatePacket.HairStyle, characterCreatePacket.Name,
                characterCreatePacket.Slot, true);

            new EntryPointPacketHandler(Session).LoadCharacters(new OpenNosEntryPointPacket
                {PacketData = characterCreatePacket.OriginalContent});
        }

        #endregion
    }
}