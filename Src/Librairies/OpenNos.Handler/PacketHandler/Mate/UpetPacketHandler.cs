using System;
using System.Linq;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Mate
{
    public class UpetPacketHandler : IPacketHandler
    {
        #region Instantiation

        public UpetPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void SpecialSkill(UpetPacket upetPacket)
        {
            if (upetPacket == null) return;

            var penalty = Session.Account.PenaltyLogs.OrderByDescending(s => s.DateEnd).FirstOrDefault();
            if (Session.Character.IsMuted() && penalty != null)
            {
                if (Session.Character.Gender == GenderType.Female)
                {
                    Session.CurrentMapInstance?.Broadcast(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_FEMALE"), 1));
                    Session.SendPacket(Session.Character.GenerateSay(
                        string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"),
                            (penalty.DateEnd - DateTime.Now).ToString("hh\\:mm\\:ss")), 11));
                }
                else
                {
                    Session.CurrentMapInstance?.Broadcast(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_MALE"), 1));
                    Session.SendPacket(Session.Character.GenerateSay(
                        string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"),
                            (penalty.DateEnd - DateTime.Now).ToString("hh\\:mm\\:ss")), 11));
                }

                return;
            }

            var attacker = Session.Character.Mates.FirstOrDefault(x => x.MateTransportId == upetPacket.MateTransportId);
            if (attacker == null) return;

            NpcMonsterSkill mateSkill = null;
            if (attacker.Monster.Skills.Any())
            {
                mateSkill = attacker.Monster.Skills.FirstOrDefault(sk => MateHelper.Instance.PetSkills.Contains(sk.SkillVNum));
            }

            if (mateSkill == null)
                mateSkill = new NpcMonsterSkill
                {
                    SkillVNum = 200
                };

            if (attacker.IsSitting) return;

            switch (upetPacket.TargetType)
            {
                case UserType.Monster:
                    if (attacker.Hp > 0)
                    {
                        MapMonster target = Session?.CurrentMapInstance?.GetMonsterById(upetPacket.TargetId);
                        attacker.TargetHit(target.BattleEntity, mateSkill);
                        Session.SendPacketAfter("petsr 0", mateSkill.Skill.Cooldown * 100);
                    }

                    return;

                case UserType.Npc:
                    {
                        MapMonster target = Session.CurrentMapInstance?.GetMonsterById(upetPacket.TargetId);
                        attacker.AttackMonster(attacker.Owner.Session, attacker, mateSkill, upetPacket.TargetId, target?.MapX ?? attacker.PositionX, target?.MapY ?? attacker.PositionY);
                        Session.SendPacketAfter("petsr 0", mateSkill.Skill.Cooldown * 100);
                    }
                    return;

                case UserType.Player:
                    if (attacker.Hp > 0)
                    {
                        Character target = Session?.CurrentMapInstance?.GetSessionByCharacterId(upetPacket.TargetId).Character;
                        attacker.TargetHit(target.BattleEntity, mateSkill);
                        Session.SendPacketAfter("petsr 0", mateSkill.Skill.Cooldown * 100);
                    }

                    return;

                case UserType.Object:
                    return;

                default:
                    return;
            }
        }

        #endregion
    }
}