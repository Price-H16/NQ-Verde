using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Battle;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace NosTale.Extension.Extension.Packet
{
    public static class MateExt
    {
        #region Methods

        public static void PartnerSkillTargetHit(this BattleEntity battleEntityAttacker,
            BattleEntity battleEntityDefender, Skill skill, bool isRecursiveCall = false)
        {
            #region Invalid entities

            if (battleEntityAttacker?.MapInstance == null
                || battleEntityAttacker.Mate?.Owner?.BattleEntity == null
                || battleEntityAttacker.Mate.Monster == null)
            {
                return;
            }

            if (battleEntityDefender?.MapInstance == null)
            {
                return;
            }

            #endregion

            #region Maps NOT matching

            if (battleEntityAttacker.MapInstance != battleEntityDefender.MapInstance)
            {
                return;
            }

            #endregion

            #region Invalid skill

            if (skill == null)
            {
                return;
            }

            #endregion

            #region Invalid state

            if (battleEntityAttacker.Hp < 1 || battleEntityAttacker.Mate.IsSitting)
            {
                return;
            }

            if (battleEntityDefender.Hp < 1)
            {
                return;
            }

            #endregion

            #region Can NOT attack

            if ((skill.TargetType != 1 || !battleEntityDefender.Equals(battleEntityAttacker)) &&
                !battleEntityAttacker.CanAttackEntity(battleEntityDefender, skill: skill)
                || battleEntityAttacker.HasBuff(BCardType.CardType.SpecialAttack,
                    (byte) AdditionalTypes.SpecialAttack.NoAttack))
            {
                return;
            }

            #endregion

            #region Cooldown

            if (!isRecursiveCall && skill.PartnerSkill != null && !skill.PartnerSkill.CanBeUsed())
            {
                return;
            }

            #endregion

            #region Enemy too far

            if (skill.TargetType == 0 && battleEntityAttacker.GetDistance(battleEntityDefender) > skill.Range)
            {
                return;
            }

            #endregion

            #region Mp NOT enough

            if (!isRecursiveCall && battleEntityAttacker.Mp < skill.MpCost)
            {
                return;
            }

            #endregion

            lock (battleEntityDefender.PVELockObject)
            {
                if (!isRecursiveCall)
                {
                    #region Update skill LastUse

                    if (skill.PartnerSkill != null)
                    {
                        skill.PartnerSkill.LastUse = DateTime.Now;
                    }

                    #endregion

                    #region Decrease MP

                    battleEntityAttacker.DecreaseMp(skill.MpCost);

                    #endregion

                    #region Cast on target

                    battleEntityAttacker.MapInstance.Broadcast(StaticPacketHelper.CastOnTarget(
                        battleEntityAttacker.UserType, battleEntityAttacker.MapEntityId,
                        battleEntityDefender.UserType, battleEntityDefender.MapEntityId,
                        skill.CastAnimation, skill.CastEffect,
                        skill.SkillVNum));

                    #endregion

                    #region Show icon

                    battleEntityAttacker.MapInstance.Broadcast(
                        StaticPacketHelper.GenerateEff(battleEntityAttacker.UserType, battleEntityAttacker.MapEntityId,
                            5005));

                    #endregion
                }

                #region Calculate damage

                var hitMode = 0;
                var onyxWings = false;
                var hasAbsorbed = false;
                var zephyrWings = false;

                var damage = DamageHelper.Instance.CalculateDamage(battleEntityAttacker, battleEntityDefender,
                    skill, ref hitMode, ref onyxWings, ref zephyrWings /*, ref hasAbsorbed*/);

                #endregion

                if (hitMode != 4)
                {
                    #region ConvertDamageToHPChance

                    if (battleEntityDefender.Character is Character target)
                    {
                        var convertDamageToHpChance = target.GetBuff(BCardType.CardType.DarkCloneSummon,
                            (byte) AdditionalTypes.DarkCloneSummon.ConvertDamageToHPChance);

                        if (ServerManager.RandomNumber() < convertDamageToHpChance[0])
                        {
                            var amount = damage;

                            if (target.Hp + amount > target.HPLoad())
                            {
                                amount = (int) target.HPLoad() - target.Hp;
                            }

                            target.Hp += amount;
                            target.ConvertedDamageToHP += amount;
                            target.MapInstance.Broadcast(target.GenerateRc(amount));
                            target.Session?.SendPacket(target.GenerateStat());

                            damage = 0;
                        }
                    }

                    #endregion

                    #region InflictDamageToMP

                    if (damage > 0)
                    {
                        var inflictDamageToMp = battleEntityDefender.GetBuff(BCardType.CardType.LightAndShadow,
                            (byte) AdditionalTypes.LightAndShadow.InflictDamageToMP);

                        if (inflictDamageToMp[0] != 0)
                        {
                            var amount = Math.Min((int) (damage / 100D * inflictDamageToMp[0]),
                                battleEntityDefender.Mp);
                            battleEntityDefender.DecreaseMp(amount);

                            damage -= amount;
                        }
                    }

                    #endregion
                }

                #region Stand up

                battleEntityDefender.Character?.StandUp();

                #endregion

                #region Cast effect

                var castTime = 0;

                if (!isRecursiveCall && skill.CastEffect != 0)
                {
                    battleEntityAttacker.MapInstance.Broadcast(StaticPacketHelper.GenerateEff(
                        battleEntityAttacker.UserType, battleEntityAttacker.MapEntityId,
                        skill.CastEffect), battleEntityAttacker.PositionX, battleEntityAttacker.PositionY);

                    castTime = skill.CastTime * 100;
                }

                #endregion

                #region Use skill

                Observable.Timer(TimeSpan.FromMilliseconds(castTime)).Subscribe(o =>
                    PartnerSkillTargetHit2(battleEntityAttacker, battleEntityDefender, skill,
                        isRecursiveCall, damage, hitMode, hasAbsorbed));

                #endregion
            }
        }

        public static void PartnerSkillTargetHit2(BattleEntity battleEntityAttacker, BattleEntity battleEntityDefender,
            Skill skill, bool isRecursiveCall, int damage, int hitMode, bool hasAbsorbed)
        {
            #region BCards

            var bcards = new List<BCard>();

            if (battleEntityAttacker.Mate.Monster.BCards != null)
            {
                bcards.AddRange(battleEntityAttacker.Mate.Monster.BCards.ToList());
            }

            if (skill.BCards != null)
            {
                bcards.AddRange(skill.BCards.ToList());
            }

            #endregion

            #region Owner

            var attackerOwner = battleEntityAttacker.Mate.Owner;

            #endregion

            lock (battleEntityDefender.PVELockObject)
            {
                #region Battle logic

                if (isRecursiveCall || skill.TargetType == 0)
                {
                    battleEntityDefender.GetDamage(damage, battleEntityAttacker);

                    battleEntityAttacker.MapInstance.Broadcast(StaticPacketHelper.SkillUsed(
                        battleEntityAttacker.UserType, battleEntityAttacker.MapEntityId,
                        battleEntityDefender.UserType, battleEntityDefender.MapEntityId, skill.SkillVNum,
                        skill.Cooldown, skill.AttackAnimation, skill.Effect,
                        battleEntityDefender.PositionX, battleEntityDefender.PositionY, battleEntityDefender.Hp > 0,
                        battleEntityDefender.HpPercent(),
                        damage, hitMode, skill.SkillType));

                    if (battleEntityDefender.Character != null)
                    {
                        battleEntityDefender.Character.Session?.SendPacket(
                                battleEntityDefender.Character.GenerateStat());
                    }

                    if (battleEntityDefender.MapMonster != null && attackerOwner.BattleEntity != null)
                    {
                        battleEntityDefender.MapMonster.AddToDamageList(attackerOwner.BattleEntity, damage);
                    }

                    bcards.ForEach(bcard =>
                    {
                        if (bcard.Type == Convert.ToByte(BCardType.CardType.Buff) &&
                            new Buff(Convert.ToInt16(bcard.SecondData), battleEntityAttacker.Level).Card?.BuffType !=
                            BuffType.Bad)
                        {
                            if (!isRecursiveCall)
                            {
                                bcard.ApplyBCards(battleEntityAttacker, battleEntityAttacker);
                            }
                        }
                        else if (battleEntityDefender.Hp > 0)
                        {
                            if (hitMode != 4 && !hasAbsorbed)
                            {
                                bcard.ApplyBCards(battleEntityDefender, battleEntityAttacker);
                            }
                        }
                    });

                    if (battleEntityDefender.Hp > 0 && hitMode != 4 && !hasAbsorbed)
                    {
                        battleEntityDefender.BCards?.ToList().ForEach(bcard =>
                        {
                            if (bcard.Type == Convert.ToByte(BCardType.CardType.Buff))
                            {
                                if (new Buff(Convert.ToInt16(bcard.SecondData), battleEntityDefender.Level).Card
                                                                                                           ?.BuffType != BuffType.Bad)
                                {
                                    bcard.ApplyBCards(battleEntityDefender, battleEntityDefender);
                                }
                                else
                                {
                                    bcard.ApplyBCards(battleEntityAttacker, battleEntityDefender);
                                }
                            }
                        });
                    }
                }
                else if (skill.TargetType == 2 && skill.HitType == 0)
                {
                    battleEntityAttacker.MapInstance.Broadcast(StaticPacketHelper.SkillUsed(
                        battleEntityAttacker.UserType, battleEntityAttacker.MapEntityId,
                        battleEntityDefender.UserType, battleEntityDefender.MapEntityId, skill.SkillVNum,
                        skill.Cooldown, skill.AttackAnimation, skill.Effect,
                        battleEntityDefender.PositionX, battleEntityDefender.PositionY, battleEntityDefender.Hp > 0,
                        battleEntityDefender.HpPercent(),
                        damage, hitMode, skill.SkillType));

                    battleEntityAttacker.MapInstance.Broadcast(StaticPacketHelper.CastOnTarget(UserType.Npc,
                        battleEntityAttacker.MapEntityId,
                        UserType.Npc, battleEntityAttacker.MapEntityId, skill.CastAnimation, skill.CastEffect,
                        skill.SkillVNum));

                    skill.BCards.ToList().ForEach(s =>
                    {
                        // Apply skill bcards to owner and pet
                        s.ApplyBCards(attackerOwner.BattleEntity, battleEntityAttacker);
                        s.ApplyBCards(battleEntityAttacker, battleEntityAttacker);
                    });

                    Observable.Timer(TimeSpan.FromMilliseconds(skill.Cooldown * 100))
                        .Subscribe(o => attackerOwner.Session?.SendPacket($"psr {skill.CastId}"));
                }
                else if (skill?.TargetType == 1 && skill.HitType != 1)
                {
                    battleEntityAttacker.MapInstance.Broadcast(StaticPacketHelper.SkillUsed(
                        battleEntityAttacker.UserType, battleEntityAttacker.MapEntityId,
                        battleEntityDefender.UserType, battleEntityDefender.MapEntityId, skill.SkillVNum,
                        skill.Cooldown, skill.AttackAnimation, skill.Effect,
                        battleEntityDefender.PositionX, battleEntityDefender.PositionY, battleEntityDefender.Hp > 0,
                        battleEntityDefender.HpPercent(),
                        damage, hitMode, skill.SkillType));

                    battleEntityAttacker.MapInstance.Broadcast(StaticPacketHelper.CastOnTarget(UserType.Npc,
                        battleEntityAttacker.MapEntityId,
                        UserType.Npc, battleEntityAttacker.MapEntityId, skill.CastAnimation, skill.CastEffect,
                        skill.SkillVNum));

                    var level = 0;
                    switch (skill.HitType)
                    {
                        case 2:
                            IEnumerable<MapMonster> entityInRange =
                                battleEntityAttacker.Mate.Owner.MapInstance?.GetMonsterInRangeList(
                                    battleEntityAttacker.PositionX, battleEntityAttacker.PositionY, skill.TargetRange);
                            foreach (var sb in skill.BCards)
                            {
                                if (sb.Type != (short) BCardType.CardType.Buff)
                                {
                                    continue;
                                }

                                var bf = new Buff((short) sb.SecondData, battleEntityAttacker.Level);
                                if (bf.Card.BuffType != BuffType.Good)
                                {
                                    continue;
                                }

                                sb.ApplyBCards(battleEntityAttacker, battleEntityAttacker);
                                sb.ApplyBCards(battleEntityAttacker.Mate.Owner.BattleEntity, battleEntityAttacker);
                            }

                            if (entityInRange != null)
                            {
                                foreach (var target in entityInRange)
                                foreach (var s in skill.BCards)
                                {
                                    if (s.Type != (short) BCardType.CardType.Buff)
                                    {
                                        s.ApplyBCards(target.BattleEntity, battleEntityAttacker);
                                        continue;
                                    }

                                    switch (battleEntityAttacker.Mate.Owner.MapInstance.MapInstanceType)
                                    {
                                        default:
                                            s.ApplyBCards(target.BattleEntity, battleEntityAttacker);
                                            break;
                                    }
                                }
                            }

                            break;

                        case 4:
                        case 0:
                            foreach (var bc in skill.BCards)
                            {
                                var bf = new Buff((short) bc.SecondData, battleEntityAttacker.Level);

                                if (bc.Type == (short) BCardType.CardType.Buff && bf.Card?.BuffType == BuffType.Good)
                                {
                                    bc.ApplyBCards(battleEntityAttacker, battleEntityAttacker);
                                    bc.ApplyBCards(battleEntityAttacker.Mate.Owner.BattleEntity, battleEntityAttacker);
                                }
                                else
                                {
                                    bc.ApplyBCards(battleEntityAttacker, battleEntityAttacker);
                                }
                            }

                            break;
                    }

                    Observable.Timer(TimeSpan.FromMilliseconds(skill.Cooldown * 100))
                        .Subscribe(o => attackerOwner.Session?.SendPacket($"psr {skill.CastId}"));
                }
                else if (skill.HitType == 1 && skill.TargetRange > 0)
                {
                    battleEntityAttacker.MapInstance.Broadcast(StaticPacketHelper.SkillUsed(
                        battleEntityAttacker.UserType, battleEntityAttacker.MapEntityId,
                        battleEntityAttacker.UserType, battleEntityAttacker.MapEntityId, skill.SkillVNum,
                        skill.Cooldown, skill.AttackAnimation, skill.Effect,
                        battleEntityAttacker.PositionX, battleEntityAttacker.PositionY, battleEntityAttacker.Hp > 0,
                        battleEntityAttacker.HpPercent(),
                        damage, hitMode, skill.SkillType));

                    if (battleEntityAttacker.Hp > 0)
                    {
                        bcards.ForEach(bcard =>
                        {
                            if (bcard.Type == Convert.ToByte(BCardType.CardType.Buff) &&
                                new Buff(Convert.ToInt16(bcard.SecondData), battleEntityAttacker.Level).Card
                                                                                                       ?.BuffType !=
                                BuffType.Bad)
                            {
                                bcard.ApplyBCards(battleEntityAttacker, battleEntityAttacker);
                            }
                        });
                    }

                    if (hitMode != 1)
                    {
                        battleEntityAttacker.TryToApplyBeachBuff(battleEntityDefender);
                    }

                    battleEntityAttacker.MapInstance
                                        .GetBattleEntitiesInRange(battleEntityAttacker.GetPos(), skill.TargetRange).ToList()
                                        .ForEach(battleEntityInRange =>
                                        {
                                            if (!battleEntityInRange.Equals(battleEntityAttacker))
                                            {
                                                PartnerSkillTargetHit(battleEntityAttacker, battleEntityInRange, skill, true);
                                            }
                                        });
                }

                #endregion

                #region Skill reset

                if (!isRecursiveCall)
                {
                    Observable.Timer(TimeSpan.FromMilliseconds(skill.Cooldown * 100))
                              .Subscribe(o => attackerOwner.Session?.SendPacket($"psr {skill.CastId}"));
                }

                #endregion

                #region Hp <= 0

                if (battleEntityDefender.Hp <= 0)
                {
                    switch (battleEntityDefender.EntityType)
                    {
                        case EntityType.Player:
                        {
                            var target = battleEntityDefender.Character;

                            if (target != null)
                            {
                                if (target.IsVehicled)
                                {
                                    target.RemoveVehicle();
                                }

                                Observable.Timer(TimeSpan.FromMilliseconds(1000))
                                          .Subscribe(o => ServerManager.Instance.AskPvpRevive(target.CharacterId));
                            }
                        }
                            break;

                        case EntityType.Mate:
                            break;

                        case EntityType.Npc:
                            battleEntityDefender.MapNpc?.RunDeathEvent();
                            break;

                        case EntityType.Monster:
                        {
                            battleEntityDefender.MapMonster?.SetDeathStatement();
                            attackerOwner.GenerateKillBonus(battleEntityDefender.MapMonster, battleEntityAttacker);
                        }
                            break;
                    }
                }

                #endregion
            }
        }

        #endregion
    }
}