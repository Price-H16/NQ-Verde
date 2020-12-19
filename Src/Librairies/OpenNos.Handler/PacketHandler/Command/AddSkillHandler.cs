using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class AddSkillHandler : IPacketHandler
    {
        #region Instantiation

        public AddSkillHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void AddSkill(AddSkillPacket addSkillPacket)
        {
            if (addSkillPacket != null)
            {
                Session.AddLogsCmd(addSkillPacket);
                Session.Character.AddSkill(addSkillPacket.SkillVNum);
                Session.SendPacket(Session.Character.GenerateSki());
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(AddSkillPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}