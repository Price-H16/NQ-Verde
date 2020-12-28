using System.Collections.Generic;
using ChickenAPI.Enums.Game.BCard;
using OpenNos.Domain;

namespace OpenNos.GameObject.Helpers
{
    public class PassiveSkillHelper
    {
        #region Members

        private static PassiveSkillHelper _instance;

        #endregion

        #region Properties

        public static PassiveSkillHelper Instance => _instance ?? (_instance = new PassiveSkillHelper());

        #endregion

        #region Methods

        public List<BCard> PassiveSkillToBCards(IEnumerable<CharacterSkill> skills)
        {
            var bcards = new List<BCard>();

            if (skills != null)
                foreach (var skill in skills)
                    switch (skill.Skill.CastId)
                    {
                        case 0:
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeSkill,
                                Type = (byte) BCardType.AttackPower,
                                SubType = (byte) BCardSubTypes.AttackPower.MeleeAttacksIncreased
                            });
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeSkill,
                                Type = (byte) BCardType.Defence,
                                SubType = (byte) BCardSubTypes.Defence.MeleeIncreased
                            });
                            break;

                        case 1:
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeSkill,
                                Type = (byte) BCardType.Target,
                                SubType = (byte) BCardSubTypes.Target.AllHitRateIncreased
                            });
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeSkill,
                                Type = (byte) BCardType.DodgeAndDefencePercent,
                                SubType = (byte) BCardSubTypes.DodgeAndDefencePercent.DodgeIncreased
                            });
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeSkill,
                                Type = (byte) BCardType.Defence,
                                SubType = (byte) BCardSubTypes.Defence.RangedIncreased
                            });
                            break;

                        case 2:
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeSkill,
                                Type = (byte) BCardType.AttackPower,
                                SubType = (byte) BCardSubTypes.AttackPower.MagicalAttacksIncreased
                            });
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeSkill,
                                Type = (byte) BCardType.Defence,
                                SubType = (byte) BCardSubTypes.Defence.MagicalIncreased
                            });
                            break;

                        case 4:
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeSkill,
                                Type = (byte) BCardType.MaxHPMP,
                                SubType = (byte) BCardSubTypes.MaxHPMP.MaximumHPIncreased
                            });
                            break;

                        case 5:
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeSkill,
                                Type = (byte) BCardType.MaxHPMP,
                                SubType = (byte) BCardSubTypes.MaxHPMP.MaximumMPIncreased
                            });
                            break;

                        case 6:
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeSkill,
                                Type = (byte) BCardType.AttackPower,
                                SubType = (byte) BCardSubTypes.AttackPower.AllAttacksIncreased
                            });
                            break;

                        case 7:
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeSkill,
                                Type = (byte) BCardType.Defence,
                                SubType = (byte) BCardSubTypes.Defence.AllIncreased
                            });
                            break;

                        case 8:
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeSkill,
                                Type = (byte) BCardType.Recovery,
                                SubType = (byte) BCardSubTypes.Recovery.HPRecoveryIncreased
                            });
                            break;

                        case 9:
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeSkill,
                                Type = (byte) BCardType.Recovery,
                                SubType = (byte) BCardSubTypes.Recovery.MPRecoveryIncreased
                            });
                            break;

                        case 19:
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeType,
                                Type = (byte) BCardType.SpecialisationBuffResistance,
                                SubType = (byte) BCardSubTypes.SpecialisationBuffResistance.IncreaseDamageInPVP
                            });
                            break;

                        case 20:
                            bcards.Add(new BCard
                            {
                                FirstData = -skill.Skill.UpgradeType,
                                Type = (byte) BCardType.SpecialisationBuffResistance,
                                SubType = (byte) BCardSubTypes.SpecialisationBuffResistance.DecreaseDamageInPVP
                            });
                            break;

                        case 21:
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeType,
                                Type = (byte) BCardType.AttackPower,
                                SubType = (byte) BCardSubTypes.AttackPower.MeleeAttacksIncreased
                            });
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeType,
                                Type = (byte) BCardType.Defence,
                                SubType = (byte) BCardSubTypes.Defence.MeleeIncreased
                            });
                            break;

                        case 22:
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeType,
                                Type = (byte) BCardType.Target,
                                SubType = (byte) BCardSubTypes.Target.AllHitRateIncreased
                            });
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeType,
                                Type = (byte) BCardType.DodgeAndDefencePercent,
                                SubType = (byte) BCardSubTypes.DodgeAndDefencePercent.DodgeIncreased
                            });
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeType,
                                Type = (byte) BCardType.Defence,
                                SubType = (byte) BCardSubTypes.Defence.RangedIncreased
                            });
                            break;

                        case 23:
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeType,
                                Type = (byte) BCardType.AttackPower,
                                SubType = (byte) BCardSubTypes.AttackPower.MagicalAttacksIncreased
                            });
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeType,
                                Type = (byte) BCardType.Defence,
                                SubType = (byte) BCardSubTypes.Defence.MagicalIncreased
                            });
                            break;

                        case 24:
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeType,
                                Type = (byte) BCardType.MaxHPMP,
                                SubType = (byte) BCardSubTypes.MaxHPMP.MaximumHPIncreased
                            });
                            break;

                        case 25:
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeType,
                                Type = (byte) BCardType.MaxHPMP,
                                SubType = (byte) BCardSubTypes.MaxHPMP.MaximumMPIncreased
                            });
                            break;

                        case 26:
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeType,
                                Type = (byte) BCardType.Defence,
                                SubType = (byte) BCardSubTypes.Defence.AllIncreased
                            });
                            break;

                        case 27:
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeType,
                                Type = (byte) BCardType.AttackPower,
                                SubType = (byte) BCardSubTypes.AttackPower.AllAttacksIncreased
                            });
                            break;

                        case 28:
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeType,
                                Type = (byte) BCardType.ElementResistance,
                                SubType = (byte) BCardSubTypes.ElementResistance.AllIncreased
                            });
                            break;

                        case 29:
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeType,
                                Type = (byte) BCardType.Item,
                                SubType = (byte) BCardSubTypes.Item.EXPIncreased
                            });
                            break;

                        case 30:
                            bcards.Add(new BCard
                            {
                                FirstData = skill.Skill.UpgradeType,
                                Type = (byte) BCardType.Item,
                                SubType = (byte) BCardSubTypes.Item.IncreaseEarnedGold
                            });
                            break;
                    }

            return bcards;
        }

        #endregion
    }
}