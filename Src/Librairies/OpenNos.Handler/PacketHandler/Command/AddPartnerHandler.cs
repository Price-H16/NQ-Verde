using NosTale.Extension.Extension.Command;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class AddPartnerHandler : IPacketHandler
    {
        #region Instantiation

        public AddPartnerHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void AddPartner(AddPartnerPacket addPartnerPacket)
        {
            if (addPartnerPacket != null)
            {
                Session.AddLogsCmd(addPartnerPacket);

                Session.AddMate(addPartnerPacket.MonsterVNum, addPartnerPacket.Level, MateType.Partner);
#pragma warning disable 4014
                DiscordWebhookHelper.DiscordEventlog($"AdministradorLog: {Session.Character.Name} NpcMonsterVNum: {addPartnerPacket.MonsterVNum} Level: {addPartnerPacket.Level} {addPartnerPacket} Comando usado!");
            }
            else
            {

                Session.SendPacket(Session.Character.GenerateSay(AddPartnerPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}