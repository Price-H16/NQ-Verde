using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Mate
{
    public class PartnerSkillOpenPacketHandler : IPacketHandler
    {
        #region Instantiation

        public PartnerSkillOpenPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void PartnerSkillOpen(PartnerSkillOpenPacket partnerSkillOpenPacket)
        {
            if (partnerSkillOpenPacket == null || partnerSkillOpenPacket.CastId < 0 || partnerSkillOpenPacket.CastId > 2)

                return;

            var mate = Session?.Character?.Mates?.Find(s => s.IsTeamMember && s.MateType == MateType.Partner);

            if (mate == null) return;

            if (mate.Sp == null || mate.IsUsingSp) return;

            if (!mate.Sp.CanLearnSkill()) return;

            var partnerSkill = mate.Sp.GetSkill(partnerSkillOpenPacket.CastId);

            if (partnerSkill != null) return;

            if (partnerSkillOpenPacket.JustDoIt)
            {
                if (mate.Sp.AddSkill(mate, partnerSkillOpenPacket.CastId))
                {
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateModal(Language.Instance.GetMessageFromKey("PSP_SKILL_LEARNED"), 1));
                    mate.Sp.ResetXp();
                }

                Session.SendPacket(mate.GenerateScPacket());
            }
            else
            {
                if (Session.Account.Authority >= AuthorityType.DEV)
                {
                    if (mate.Sp.AddSkill(mate, partnerSkillOpenPacket.CastId))
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateModal(Language.Instance.GetMessageFromKey("PSP_SKILL_LEARNED"),
                                1));
                        mate.Sp.FullXp();
                    }

                    Session.SendPacket(mate.GenerateScPacket());
                    return;
                }

                Session.SendPacket(
                    $"pdelay 3000 12 #ps_op^{partnerSkillOpenPacket.PetId}^{partnerSkillOpenPacket.CastId}^1");
                Session.CurrentMapInstance?.Broadcast(UserInterfaceHelper.GenerateGuri(2, 2, mate.MateTransportId),
                    mate.PositionX, mate.PositionY);
            }
        }

        #endregion
    }
}