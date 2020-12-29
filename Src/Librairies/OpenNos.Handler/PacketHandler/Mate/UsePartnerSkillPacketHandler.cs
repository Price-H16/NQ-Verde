using NosTale.Extension.Extension.Packet;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Battle;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;


namespace OpenNos.Handler.PacketHandler.Mate
{
    public class UsePartnerSkillPacketHandler : IPacketHandler
    {
        #region Instantiation

        public UsePartnerSkillPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void UseSkill(UsePartnerSkillPacket usePartnerSkillPacket)
        {
            #region Invalid packet

            if (usePartnerSkillPacket == null)
            {
                return;
            }

            #endregion

            #region Mate not found (or invalid)

            GameObject.Mate mate = Session?.Character?.Mates?.ToList().FirstOrDefault(s => s.IsTeamMember && s.MateType == MateType.Partner && s.MateTransportId == usePartnerSkillPacket.TransportId);

            if (mate?.Monster == null)
            {
                return;
            }

            #endregion

            #region Not using PSP

            if (mate.Sp == null || !mate.IsUsingSp)
            {
                return;
            }

            #endregion

            #region Skill not found

            PartnerSkill partnerSkill = mate.Sp.GetSkill(usePartnerSkillPacket.CastId);

            if (partnerSkill == null)
            {
                return;
            }

            #endregion

            #region Convert PartnerSkill to Skill

            Skill skill = PartnerSkillHelper.ConvertToNormalSkill(partnerSkill);

            #endregion

            #region Battle entities

            BattleEntity battleEntityAttacker = mate.BattleEntity;
            BattleEntity battleEntityDefender = null;

            switch (usePartnerSkillPacket.TargetType)
            {
                case UserType.Player:
                    {
                        Character target = Session.Character.MapInstance?.GetCharacterById(usePartnerSkillPacket.TargetId);
                        battleEntityDefender = target?.BattleEntity;
                    }
                    break;

                case UserType.Npc:
                    {
                        GameObject.Mate target = Session.Character.MapInstance?.GetMate(usePartnerSkillPacket.TargetId);
                        battleEntityDefender = target?.BattleEntity;
                    }
                    break;

                case UserType.Monster:
                    {
                        MapMonster target = Session.Character.MapInstance?.GetMonsterById(usePartnerSkillPacket.TargetId);
                        battleEntityDefender = target?.BattleEntity;
                    }
                    break;
            }

            #endregion

            #region Attack

            #region Partner Skills
            switch (partnerSkill.SkillVNum)
            {
                //Venus
                case 1451:
                    Session.Character.AddBuff(new Buff((short)(2405 + partnerSkill.Level), 5), Session.Character.BattleEntity);
                    break;


                //Perti
                case 662:
                    Session.Character.AddBuff(new Buff((short)(2552 + partnerSkill.Level), 5), Session.Character.BattleEntity);
                    break;

                //Akhenaton
                case 783:
                    Session.Character.AddBuff(new Buff((short)(2580 + partnerSkill.Level), 5), Session.Character.BattleEntity);
                    break;


                //Yuna
                case 1604:
                    Session.Character.AddBuff(new Buff((short)(2482 + partnerSkill.Level), 5), Session.Character.BattleEntity);
                    break;

                //Foxy & Fiona
                case 1294:
                    Session.Character.AddBuff(new Buff((short)(2111 + partnerSkill.Level), 5), Session.Character.BattleEntity);
                    break;

                //Maru & Maru in Mothers Fur
                case 1301:
                    Session.Character.AddBuff(new Buff((short)(2132 + partnerSkill.Level), 5), Session.Character.BattleEntity);
                    break;

                //Hongbi & Cheongbi
                case 1317:
                    Session.Character.AddBuff(new Buff((short)(2153 + partnerSkill.Level), 5), Session.Character.BattleEntity);
                    break;

                //Aegir
                case 1239:
                    Session.Character.AddBuff(new Buff((short)(1999 + partnerSkill.Level), 5), Session.Character.BattleEntity);
                    break;

            }
            #endregion

            PartnerSkillTargetHit(battleEntityAttacker, battleEntityDefender, skill);

            #endregion
        }

        private void PartnerSkillTargetHit(BattleEntity battleEntityAttacker, BattleEntity battleEntityDefender, Skill skill, bool isRecursiveCall = false)
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

            if (((skill.TargetType != 1 || !battleEntityDefender.Equals(battleEntityAttacker)) && !battleEntityAttacker.CanAttackEntity(battleEntityDefender))
                || battleEntityAttacker.HasBuff(BCardType.CardType.SpecialAttack, (byte)AdditionalTypes.SpecialAttack.NoAttack))
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

                    battleEntityAttacker.MapInstance.Broadcast(StaticPacketHelper.GenerateEff(battleEntityAttacker.UserType, battleEntityAttacker.MapEntityId, 5005));

                    #endregion
                }

                #region Calculate damage

                int hitMode = 0;
                bool onyxWings = false;
                bool zephyrWings = false;
                bool hasAbsorbed = false;

                int damage = DamageHelper.Instance.CalculateDamage(battleEntityAttacker, battleEntityDefender,
                    skill, ref hitMode, ref onyxWings/*, ref hasAbsorbed*/, ref zephyrWings);

                #endregion

                if (hitMode != 4)
                {
                    #region ConvertDamageToHPChance

                    if (battleEntityDefender.Character is Character target)
                    {
                        int[] convertDamageToHpChance = target.GetBuff(BCardType.CardType.DarkCloneSummon, (byte)AdditionalTypes.DarkCloneSummon.ConvertDamageToHPChance);

                        if (ServerManager.RandomNumber() < convertDamageToHpChance[0])
                        {
                            int amount = damage;

                            if (target.Hp + amount > target.HPLoad())
                            {
                                amount = (int)target.HPLoad() - target.Hp;
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
                        int[] inflictDamageToMp = battleEntityDefender.GetBuff(BCardType.CardType.LightAndShadow, (byte)AdditionalTypes.LightAndShadow.InflictDamageToMP);

                        if (inflictDamageToMp[0] != 0)
                        {
                            int amount = Math.Min((int)(damage / 100D * inflictDamageToMp[0]), battleEntityDefender.Mp);
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

                int castTime = 0;

                if (!isRecursiveCall && skill.CastEffect != 0)
                {
                    battleEntityAttacker.MapInstance.Broadcast(StaticPacketHelper.GenerateEff(battleEntityAttacker.UserType, battleEntityAttacker.MapEntityId,
                        skill.CastEffect), battleEntityAttacker.PositionX, battleEntityAttacker.PositionY);

                    castTime = skill.CastTime * 100;
                }

                #endregion

                #region Use skill

                Observable.Timer(TimeSpan.FromMilliseconds(castTime)).Subscribe(o => PartnerSkillTargetHit2(battleEntityAttacker, battleEntityDefender, skill,
                    isRecursiveCall, damage, hitMode, hasAbsorbed));

                #endregion
            }
        }

        private void PartnerSkillTargetHit2(BattleEntity battleEntityAttacker, BattleEntity battleEntityDefender, Skill skill, bool isRecursiveCall, int damage, int hitMode, bool hasAbsorbed)
        {
            #region BCards

            List<BCard> bcards = new List<BCard>();

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

            Character attackerOwner = battleEntityAttacker.Mate.Owner;

            #endregion

            lock (battleEntityDefender.PVELockObject)
            {
                #region Battle logic

                if (isRecursiveCall || skill.TargetType == 0)
                {
                    battleEntityDefender.GetDamage(damage, battleEntityAttacker);

                    battleEntityAttacker.MapInstance.Broadcast(StaticPacketHelper.SkillUsed(battleEntityAttacker.UserType, battleEntityAttacker.MapEntityId,
                        battleEntityDefender.UserType, battleEntityDefender.MapEntityId, skill.SkillVNum, skill.Cooldown, skill.AttackAnimation, skill.Effect,
                        battleEntityDefender.PositionX, battleEntityDefender.PositionY, battleEntityDefender.Hp > 0, battleEntityDefender.HpPercent(),
                        damage, hitMode, skill.SkillType));

                    if (battleEntityDefender.Character != null)
                    {
                        battleEntityDefender.Character.Session?.SendPacket(battleEntityDefender.Character.GenerateStat());
                    }

                    if (battleEntityDefender.MapMonster != null && attackerOwner.BattleEntity != null)
                    {
                        battleEntityDefender.MapMonster.AddToDamageList(attackerOwner.BattleEntity, damage);
                    }

                    bcards.ForEach(bcard =>
                    {
                        if (bcard.Type == Convert.ToByte(BCardType.CardType.Buff) && new Buff(Convert.ToInt16(bcard.SecondData), battleEntityAttacker.Level).Card?.BuffType != BuffType.Bad)
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
                                if (new Buff(Convert.ToInt16(bcard.SecondData), battleEntityDefender.Level).Card?.BuffType != BuffType.Bad)
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
                else if (skill.HitType == 1 && skill.TargetRange > 0)
                {
                    battleEntityAttacker.MapInstance.Broadcast(StaticPacketHelper.SkillUsed(battleEntityAttacker.UserType, battleEntityAttacker.MapEntityId,
                        battleEntityAttacker.UserType, battleEntityAttacker.MapEntityId, skill.SkillVNum, skill.Cooldown, skill.AttackAnimation, skill.Effect,
                        battleEntityAttacker.PositionX, battleEntityAttacker.PositionY, battleEntityAttacker.Hp > 0, battleEntityAttacker.HpPercent(),
                        damage, hitMode, skill.SkillType));

                    if (battleEntityAttacker.Hp > 0)
                    {
                        bcards.ForEach(bcard =>
                        {
                            if (bcard.Type == Convert.ToByte(BCardType.CardType.Buff) && new Buff(Convert.ToInt16(bcard.SecondData), battleEntityAttacker.Level).Card?.BuffType != BuffType.Bad)
                            {
                                bcard.ApplyBCards(battleEntityAttacker, battleEntityAttacker);
                            }
                        });
                    }

                    battleEntityAttacker.MapInstance.GetBattleEntitiesInRange(battleEntityAttacker.GetPos(), skill.TargetRange).ToList()
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

                if (!isRecursiveCall && (skill.Class == 28 || skill.Class == 29))
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
                                Character target = battleEntityDefender.Character;

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