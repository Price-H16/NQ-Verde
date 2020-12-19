using NosTale.Extension.Extension.Command;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class AddPetHandler : IPacketHandler
    {
        #region Instantiation

        public AddPetHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void AddPet(AddPetPacket addPetPacket)
        {
            if (addPetPacket != null)
            {
                Session.AddLogsCmd(addPetPacket);

                Session.AddMate(addPetPacket.MonsterVNum, addPetPacket.Level, MateType.Pet);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(AddPartnerPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}