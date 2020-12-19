using OpenNos.Domain;
using OpenNos.GameObject.Networking;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.GameObject.Helpers;

namespace OpenNos.GameObject.Extension
{
    public static class MateExtension
    {
        public static void HitTrainer(this Mate e, int trainerVnum, int amount = 1)
        {
            bool canDown = trainerVnum != 636 && trainerVnum != 971;

            e.TrainerHits += amount;
            if (e.TrainerHits >= MateHelper.Instance.TrainerUpgradeHits[e.Attack])
            {
                e.TrainerHits = 0;

                int UpRate = MateHelper.Instance.TrainerUpRate[e.Attack];
                int DownRate = MateHelper.Instance.TrainerDownRate[e.Attack];

                int rnd = ServerManager.RandomNumber();

                if (DownRate < UpRate)
                {
                    if (rnd < DownRate && canDown)
                    {
                        DownAttack();
                    }
                    else if (rnd < UpRate)
                    {
                        UpAttack();
                    }
                    else
                    {
                        EqualAttack();
                    }
                }
                else
                {
                    if (rnd < UpRate)
                    {
                        UpAttack();
                    }
                    else if (rnd < DownRate && canDown)
                    {
                        DownAttack();
                    }
                    else
                    {
                        EqualAttack();
                    }
                }

                void UpAttack()
                {
                    if (e.Attack < 10)
                    {
                        e.Attack++;
                        e.BattleEntity.AttackUpgrade++;
                        e.Owner.Session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("MATE_ATTACK_CHANGED"), e.Attack), 0));
                        e.Owner.Session.SendPacket(e.Owner.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MATE_ATTACK_CHANGED"), e.Attack), 12));
                    }
                    else
                    {
                        e.Owner.Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("MATE_MAX_ATTACK"), 0));
                    }
                }
                void DownAttack()
                {
                    if (e.Attack > 0)
                    {
                        e.Attack--;
                        e.BattleEntity.AttackUpgrade--;
                        e.Owner.Session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("MATE_ATTACK_CHANGED"), e.Attack), 0));
                        e.Owner.Session.SendPacket(e.Owner.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MATE_ATTACK_CHANGED"), e.Attack), 12));
                    }
                    else
                    {
                        e.Owner.Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("MATE_MIN_ATTACK"), 0));
                    }
                }
                void EqualAttack()
                {
                    e.Owner.Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("MATE_ATTACK_EQUAL"), 0));
                    e.Owner.Session.SendPacket(e.Owner.GenerateSay(Language.Instance.GetMessageFromKey("MATE_ATTACK_EQUAL"), 12));
                }

                e.Owner.Session.SendPacket(UserInterfaceHelper.GeneratePClear());
                e.Owner.Session.SendPackets(e.Owner.GenerateScP());
                e.Owner.Session.SendPackets(e.Owner.GenerateScN());
            }
        }

        public static void RemovePartnerSkill(this Mate e)
        {
            if (e.Owner == null)
            {
                return;
            }

            if (e.MateType != MateType.Partner)
            {
                return;
            }

            NpcMonster mateNpc = ServerManager.GetNpcMonster(e.NpcMonsterVNum);
            if (mateNpc.Skills == null)
            {
                return;
            }

            foreach (NpcMonsterSkill skill in mateNpc.Skills)
            {
                e.Owner.Skills.Remove(skill.SkillVNum);
            }

            e.Owner.Session.SendPacket(e.Owner.GenerateSki());
        }

        public static void DefendTrainer(this Mate e, int trainerVnum, int amount = 1)
        {
            bool canDown = trainerVnum != 636 && trainerVnum != 971;

            e.TrainerDefences += amount;
            if (e.TrainerDefences >= MateHelper.Instance.TrainerUpgradeHits[e.Defence])
            {
                e.TrainerDefences = 0;
                int UpRate = MateHelper.Instance.TrainerUpRate[e.Defence];
                int DownRate = MateHelper.Instance.TrainerDownRate[e.Defence];

                int rnd = ServerManager.RandomNumber();

                if (DownRate < UpRate)
                {
                    if (rnd < DownRate && canDown)
                    {
                        DownDefence();
                    }
                    else if (rnd < UpRate)
                    {
                        UpDefence();
                    }
                    else
                    {
                        EqualDefence();
                    }
                }
                else
                {
                    if (rnd < UpRate)
                    {
                        UpDefence();
                    }
                    else if (rnd < DownRate && canDown)
                    {
                        DownDefence();
                    }
                    else
                    {
                        EqualDefence();
                    }
                }

                void UpDefence()
                {
                    if (e.Defence < 10)
                    {
                        e.Defence++;
                        e.BattleEntity.DefenseUpgrade++;
                        e.Owner.Session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("MATE_DEFENCE_CHANGED"), e.Defence), 0));
                        e.Owner.Session.SendPacket(e.Owner.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MATE_DEFENCE_CHANGED"), e.Defence), 12));
                    }
                    else
                    {
                        e.Owner.Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("MATE_MAX_DEFENCE"), 0));
                    }
                }
                void DownDefence()
                {
                    if (e.Defence > 0)
                    {
                        e.Defence--;
                        e.BattleEntity.DefenseUpgrade--;
                        e.Owner.Session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("MATE_DEFENCE_CHANGED"), e.Defence), 0));
                        e.Owner.Session.SendPacket(e.Owner.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MATE_DEFENCE_CHANGED"), e.Defence), 12));
                    }
                    else
                    {
                        e.Owner.Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("MATE_MIN_DEFENCE"), 0));
                    }
                }
                void EqualDefence()
                {
                    e.Owner.Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("MATE_DEFENCE_EQUAL"), 0));
                    e.Owner.Session.SendPacket(e.Owner.GenerateSay(Language.Instance.GetMessageFromKey("MATE_DEFENCE_EQUAL"), 12));
                }

                e.Owner.Session.SendPacket(UserInterfaceHelper.GeneratePClear());
                e.Owner.Session.SendPackets(e.Owner.GenerateScP());
                e.Owner.Session.SendPackets(e.Owner.GenerateScN());
            }
        }

        public static bool CanUseBasicSkill(this Mate e)
        {
            return e.Monster != null && e.LastBasicSkillUse.AddMilliseconds(e.Monster.BasicCooldown * 100) < DateTime.Now;
        }

        public static void StartSpCooldown(this Mate e)
        {
            if (e.Sp == null) return;

            e.SpCooldown = e.Sp.GetCooldown();

            Observable.Timer(TimeSpan.FromSeconds(e.SpCooldown)).Subscribe(o => e.Owner?.Session?.SendPacket("psd 0"));

            e.Owner?.Session?.SendPacket($"psd {e.SpCooldown}");

            e.LastSpCooldown = DateTime.Now;
        }

        public static bool CanUseSp(this Mate e)
        {
            return e.LastSpCooldown.AddSeconds(e.SpCooldown) < DateTime.Now;
        }

        public static int GetSpRemainingCooldown(this Mate e)
        {
            return (int)(e.LastSpCooldown - DateTime.Now).TotalSeconds + e.SpCooldown;
        }

        public static void GenerateDpski(this Mate e)
        {
            e.Owner.Session.SendPacket("dpski");
        }

        public static void RemoveSp(this Mate e, bool isBackToMiniland = false)
        {
            if (e.Owner?.Session == null || e.Owner.MapInstance == null)
            {
                return;
            }

            e.IsUsingSp = false;

            e.Owner.Session.SendPacket(e.GenerateScPacket());

            if (!e.IsTeamMember) return;

            e.Owner.MapInstance.Broadcast(e.GenerateCMode(-1));
            e.GenerateDpski();
            e.Owner.Session.SendPacket(e.GenerateCond());
            e.Owner.MapInstance.Broadcast(e.GenerateOut());

            if (!isBackToMiniland)
            {
                bool isAct4 = ServerManager.Instance.ChannelId == 51;

                Parallel.ForEach(e.Owner.MapInstance.Sessions.Where(s => s.Character != null), s =>
                {
                    if (!isAct4 || e.Owner.Faction == s.Character.Faction)
                    {
                        s.SendPacket(e.GenerateIn(false, isAct4));
                    }
                    else
                    {
                        s.SendPacket(e.GenerateIn(true, isAct4, s.Account.Authority));
                    }
                });
            }

            e.Owner.Session.SendPacket(e.Owner.GeneratePinit());
        }

        public static string GenerateCMode(this Mate e, short morphId)
        {
            return $"c_mode 2 {e.MateTransportId} {morphId} 0 0 10 0";
        }

        public static string GenerateCond(this Mate e)
        {
            return
                $"cond 2 {e.MateTransportId} " +
                $"{(e.HasBuff(BCardType.CardType.SpecialAttack, (byte)AdditionalTypes.SpecialAttack.NoAttack) || e.Loyalty <= 0 ? 1 : 0)} " +
                $"{(e.HasBuff(BCardType.CardType.Move, (byte)AdditionalTypes.Move.MovementImpossible) || e.Loyalty <= 0 ? 1 : 0)} {e.Speed}";
        }

        public static string GeneratePst(this Mate mate)
        {
            return "pst " +
                   "2 " +
                   $"{mate.MateTransportId} " +
                   $"{(int) mate.MateType} " +
                   $"{(int) (mate.Hp / (float) mate.MaxHp * 100)} " +
                   $"{(int) (mate.Mp / (float) mate.MaxMp * 100)} " +
                   $"{mate.Hp} " +
                   $"{mate.Mp} " +
                   "0 " +
                   "0 " +
                   "0 " +
                   $"{mate.Buff.GetAllItems().Aggregate(string.Empty, (current, buff) => current + $" {buff.Card.CardId}.{buff.Level}")}";
        }


        public static string GenerateEInfo(this Mate e) =>
            $"e_info 10 " +
            $"{e.NpcMonsterVNum} " +
            $"{e.Level} " +
            $"{(e.MateType == MateType.Pet ? e.Monster.Element : /*SP Element*/0)} " +
            $"{e.Monster.AttackClass} " +
            $"{e.Monster.ElementRate} " +
            $"{e.Attack + (e.WeaponInstance?.Upgrade ?? 0)} " +
            $"{e.DamageMinimum + (e.WeaponInstance?.Item.DamageMinimum ?? 0)} " +
            $"{e.DamageMaximum + (e.WeaponInstance?.Item.DamageMaximum ?? 0)} " +
            $"{e.Concentrate + (e.WeaponInstance?.Item.HitRate ?? 0)} " +
            $"{e.Monster.CriticalChance + (e.WeaponInstance?.Item.CriticalLuckRate ?? 0)} " +
            $"{e.Monster.CriticalRate + (e.WeaponInstance?.Item.CriticalRate ?? 0)} " +
            $"{e.Defence + (e.ArmorInstance?.Upgrade ?? 0)} " +
            $"{e.MeleeDefense + (e.ArmorInstance?.Item.CloseDefence ?? 0) + (e.GlovesInstance?.Item.CloseDefence ?? 0) + (e.BootsInstance?.Item.CloseDefence ?? 0)} " +
            $"{e.MeleeDefenseDodge + (e.ArmorInstance?.Item.DefenceDodge ?? 0) + (e.GlovesInstance?.Item.DefenceDodge ?? 0) + (e.BootsInstance?.Item.DefenceDodge ?? 0)} " +
            $"{e.RangeDefense + (e.ArmorInstance?.Item.DistanceDefence ?? 0) + (e.GlovesInstance?.Item.DistanceDefence ?? 0) + (e.BootsInstance?.Item.DistanceDefence ?? 0)} " +
            $"{e.RangeDefenseDodge + (e.ArmorInstance?.Item.DistanceDefenceDodge ?? 0) + (e.GlovesInstance?.Item.DistanceDefenceDodge ?? 0) + (e.BootsInstance?.Item.DistanceDefenceDodge ?? 0)} " +
            $"{e.MagicalDefense + (e.ArmorInstance?.Item.MagicDefence ?? 0) + (e.GlovesInstance?.Item.MagicDefence ?? 0) + (e.BootsInstance?.Item.MagicDefence ?? 0)} " +
            $"{e.EquipmentFireResistance + e.Monster.FireResistance + (e.GlovesInstance?.FireResistance ?? 0) + (e.GlovesInstance?.Item.FireResistance ?? 0) + (e.BootsInstance?.FireResistance ?? 0) + (e.BootsInstance?.Item.FireResistance ?? 0)} " +
            $"{e.EquipmentWaterResistance + e.Monster.WaterResistance + (e.GlovesInstance?.WaterResistance ?? 0) + (e.GlovesInstance?.Item.WaterResistance ?? 0) + (e.BootsInstance?.WaterResistance ?? 0) + (e.BootsInstance?.Item.WaterResistance ?? 0)} " +
            $"{e.EquipmentLightResistance + e.Monster.LightResistance + (e.GlovesInstance?.LightResistance ?? 0) + (e.GlovesInstance?.Item.LightResistance ?? 0) + (e.BootsInstance?.LightResistance ?? 0) + (e.BootsInstance?.Item.LightResistance ?? 0)} " +
            $"{e.EquipmentDarkResistance + e.Monster.DarkResistance + (e.GlovesInstance?.DarkResistance ?? 0) + (e.GlovesInstance?.Item.DarkResistance ?? 0) + (e.BootsInstance?.DarkResistance ?? 0) + (e.BootsInstance?.Item.DarkResistance ?? 0)} " +
            $"{e.MaxHp} " +
            $"{e.MaxMp} " +
            $"-1 " +
            $"{e.Name.Replace(' ', '^')}";

        public static string GenerateIn(this Mate e, bool hideNickname = false, bool isAct4 = false, AuthorityType receiverAuthority = AuthorityType.User)
        {
            if (!e.IsTemporalMate && (e.Owner.Invisible || e.Owner.InvisibleGm || e.Owner.IsVehicled || e.Owner.IsSeal || !e.IsAlive) && e.Owner.MapInstance.Map.MapId != 20001)
            {
                return "";
            }

            string name = e.IsUsingSp ? e.Sp.GetName() : e.Name.Replace(' ', '^');

            

            int faction = isAct4 ? (byte)e.Owner.Faction + 2 : 0;

            if (e.IsUsingSp && e.Sp != null)
            {
                string spName = e.IsUsingSp ? e.Sp.GetName() : e.Name.Replace(' ', '^');
                //NRE should be fixed, if not active this commented packet and remove the actual.
                //return $"in 2 {NpcMonsterVNum} {MateTransportId} {PositionX} {PositionY} {Direction} {(int)(Hp / MaxHp * 100)} {(int)(Mp / MaxMp * 100)} 0 {faction} 3 {CharacterId} 1 0 {(IsUsingSp && Sp != null ? Sp.Instance.Item.Morph : (Skin != 0 ? Skin : -1))} {name} {(Sp != null ? 1 : 0)} {(IsUsingSp ? 1 : 0)} {(IsUsingSp ? 1 : 0)}{(IsUsingSp ? Sp.GenerateSkills(false) : " 0 0 0")} 0 0 0 0";
                return
                $"in 2 {e.NpcMonsterVNum} {e.MateTransportId} {(e.IsTeamMember ? e.PositionX : e.MapX)} {(e.IsTeamMember ? e.PositionY : e.MapY)} " +
                $"{e.Direction} {(int)(e.Hp / (float)e.MaxHp * 100)} {(int)(e.Mp / (float)e.MaxMp * 100)} 0 {faction} 3 {e.CharacterId} 1 0 " +
                $"{(e.IsUsingSp && e.Sp != null ? e.Sp.Instance.Item.Morph: e.Skin != 0 ? e.Skin : -1)} {spName} 1 1 1 " +
                $"{(e.IsUsingSp ? e.Sp.GenerateSkills(false) : " 0 0 0")}" +
                $"{(e.IsUsingSp ? e.Sp.GenerateSkills(false, true) : " 0 0 0")} 0";
            }

            if (hideNickname)
            {
                name = "!§$%&/()=?*+~#";
            }

            return $"in 2 {e.NpcMonsterVNum} {e.MateTransportId} {e.PositionX} {e.PositionY} {e.Direction} " +
                   $"{(int)(e.Hp / e.MaxHp * 100)} {(int)(e.Mp / e.MaxMp * 100)} 0 {faction} 3 {e.CharacterId} " +
                   $"1 0 {(e.IsUsingSp && e.Sp != null ? e.Sp.Instance.Item.Morph : (e.Skin != 0 ? e.Skin : -1))} {name} " +
                   $"{(e.MateType == MateType.Pet ? 2 : e.Sp != null ? 1 : 0)} {(e.IsUsingSp ? 1 : 0)} {(e.IsUsingSp ? 1 : 0)}{(e.IsUsingSp ? e.Sp.GenerateSkills(false) : " 0 0 0")} 0 0 0 0";
        }

        public static string GenerateOut(this Mate e)
        {
            return $"out 2 {e.MateTransportId}";
        }

        public static string GenerateRest(this Mate e, bool ownerSit)
        {
            e.IsSitting = ownerSit ? e.Owner.IsSitting : !e.IsSitting;
            return $"rest 2 {e.MateTransportId} {(e.IsSitting ? 1 : 0)}";
        }

        public static string GenerateScPacket(this Mate e)
        {
            if (e.IsTemporalMate)
            {
                return "";
            }

            double xp = e.XpLoad();

            if (xp > int.MaxValue)
            {
                xp = (int)(xp / 100);
            }

            switch (e.MateType)
            {
                case MateType.Partner:
                    return
                        $"sc_n " +
                        $"{e.PetId} " +
                        $"{e.NpcMonsterVNum} " +
                        $"{e.MateTransportId} " +
                        $"{e.Level} " +
                        $"{e.Loyalty} " +
                        $"{e.Experience} " +
                        $"{(e.WeaponInstance != null ? $"{e.WeaponInstance.ItemVNum}.{e.WeaponInstance.Rare}.{e.WeaponInstance.Upgrade}" : "-1")} " +
                        $"{(e.ArmorInstance != null ? $"{e.ArmorInstance.ItemVNum}.{e.ArmorInstance.Rare}.{e.ArmorInstance.Upgrade}" : "-1")} " +
                        $"{(e.GlovesInstance != null ? $"{e.GlovesInstance.ItemVNum}.0.0" : "-1")} " +
                        $"{(e.BootsInstance != null ? $"{e.BootsInstance.ItemVNum}.0.0" : "-1")} " +
                        $"0 0 1 " +
                        $"{e.WeaponInstance?.Upgrade ?? 0} " +
                        $"{e.DamageMinimum + (e.WeaponInstance?.Item.DamageMinimum ?? 0)} " +
                        $"{e.DamageMaximum + (e.WeaponInstance?.Item.DamageMaximum ?? 0)} " +
                        $"{e.Concentrate + (e.WeaponInstance?.Item.HitRate ?? 0)} " +
                        $"{e.Monster.CriticalChance + (e.WeaponInstance?.Item.CriticalLuckRate ?? 0)} " +
                        $"{e.Monster.CriticalRate + (e.WeaponInstance?.Item.CriticalRate ?? 0)} " +
                        $"{e.ArmorInstance?.Upgrade ?? 0} {e.Monster.CloseDefence + e.MeleeDefense + (e.ArmorInstance?.Item.CloseDefence ?? 0) + (e.GlovesInstance?.Item.CloseDefence ?? 0) + (e.BootsInstance?.Item.CloseDefence ?? 0)} " +
                        $"{e.MeleeDefenseDodge + (e.ArmorInstance?.Item.DefenceDodge ?? 0) + (e.GlovesInstance?.Item.DefenceDodge ?? 0) + (e.BootsInstance?.Item.DefenceDodge ?? 0)} " +
                        $"{e.RangeDefense + (e.ArmorInstance?.Item.DistanceDefence ?? 0) + (e.GlovesInstance?.Item.DistanceDefence ?? 0) + (e.BootsInstance?.Item.DistanceDefence ?? 0)} " +
                        $"{e.RangeDefenseDodge + (e.ArmorInstance?.Item.DistanceDefenceDodge ?? 0) + (e.GlovesInstance?.Item.DistanceDefenceDodge ?? 0) + (e.BootsInstance?.Item.DistanceDefenceDodge ?? 0)} " +
                        $"{e.MagicalDefense + (e.ArmorInstance?.Item.MagicDefence ?? 0) + (e.GlovesInstance?.Item.MagicDefence ?? 0) + (e.BootsInstance?.Item.MagicDefence ?? 0)} " +
                        $"{(e.IsUsingSp ? e.Sp.Instance.Item.Element : 0)} " +
                        $"{e.EquipmentFireResistance + e.Monster.FireResistance + (e.GlovesInstance?.FireResistance ?? 0) + (e.GlovesInstance?.Item.FireResistance ?? 0) + (e.BootsInstance?.FireResistance ?? 0) + (e.BootsInstance?.Item.FireResistance ?? 0)} " +
                        $"{e.EquipmentWaterResistance + e.Monster.WaterResistance + (e.GlovesInstance?.WaterResistance ?? 0) + (e.GlovesInstance?.Item.WaterResistance ?? 0) + (e.BootsInstance?.WaterResistance ?? 0) + (e.BootsInstance?.Item.WaterResistance ?? 0)} " +
                        $"{e.EquipmentLightResistance + e.Monster.LightResistance + (e.GlovesInstance?.LightResistance ?? 0) + (e.GlovesInstance?.Item.LightResistance ?? 0) + (e.BootsInstance?.LightResistance ?? 0) + (e.BootsInstance?.Item.LightResistance ?? 0)} " +
                        $"{e.EquipmentDarkResistance + e.Monster.DarkResistance + (e.GlovesInstance?.DarkResistance ?? 0) + (e.GlovesInstance?.Item.DarkResistance ?? 0) + (e.BootsInstance?.DarkResistance ?? 0) + (e.BootsInstance?.Item.DarkResistance ?? 0)} " +
                        $"{e.Hp} " +
                        $"{e.MaxHp} " +
                        $"{e.Mp} " +
                        $"{e.MaxMp} " +
                        $"{(e.IsTeamMember ? "1" : "0")} " +
                        $"{xp} " +
                        $"{(e.IsUsingSp ? e.Sp.GetName() : e.Name.Replace(' ', '^'))} " +
                        $"{(e.IsUsingSp && e.Sp != null ? e.Sp.Instance.Item.Morph : e.Skin != 0 ? e.Skin : -1)} " +
                        $"{(e.IsSummonable ? 1 : 0)} " +
                        $"{(e.Sp != null ? $"{e.Sp.Instance.ItemVNum}.{e.Sp.GetXpPercent()}" : "-1")}" +
                        $"{(e.Sp != null ? e.Sp.GenerateSkills() : " -1 -1 -1")}";

                case MateType.Pet:
                    return
                        $"sc_p " +
                        $"{e.PetId} " +
                        $"{e.NpcMonsterVNum} " +
                        $"{e.MateTransportId} " +
                        $"{e.Level} " +
                        $"{e.Loyalty} " +
                        $"{e.Experience} " +
                        $"0 " +
                        $"{e.Attack} " +
                        $"{e.DamageMinimum} " +
                        $"{e.DamageMaximum} " +
                        $"{e.Concentrate} " +
                        $"{e.Monster.CriticalChance} " +
                        $"{e.Monster.CriticalRate} " +
                        $"{e.Defence} " +
                        $"{e.MeleeDefense} " +
                        $"{e.MeleeDefenseDodge} " +
                        $"{e.RangeDefense} " +
                        $"{e.RangeDefenseDodge} " +
                        $"{e.MagicalDefense} " +
                        $"{e.Monster.Element} " +
                        $"{e.Monster.FireResistance} " +
                        $"{e.Monster.WaterResistance} " +
                        $"{e.Monster.LightResistance} " +
                        $"{e.Monster.DarkResistance} " +
                        $"{e.Hp} " +
                        $"{e.MaxHp} " +
                        $"{e.Mp} " +
                        $"{e.MaxMp} " +
                        $"{(e.IsTeamMember ? "1" : "0")} " +
                        $"{xp} " +
                        $"{(e.CanPickUp ? 1 : 0)} " +
                        $"{e.Name.Replace(' ', '^')} " +
                        $"{(e.IsSummonable ? 1 : 0)}";
            }

            return "";
        }

        public static string GenerateStatInfo(this Mate mate)
        {
            return "st " +
                   "2 " +
                   $"{mate.MateTransportId} " +
                   $"{mate.Level} " +
                   "0 " +
                   $"{(int) (mate.Hp / (float) mate.MaxHp * 100)} " +
                   $"{(int) (mate.Mp / (float) mate.MaxMp * 100)} " +
                   $"{mate.Hp} " +
                   $"{mate.Mp} " +
                   $"{mate.Buff.GetAllItems().Aggregate(string.Empty, (current, buff) => current + $" {buff.Card.CardId}.{buff.Level}")}";
        }


        public static string GenerateTp(this Mate e)
        {
            return $"tp 2 {e.MateTransportId} {e.PositionX} {e.PositionY} 0";
        }

        public static string GenerateRc(this Mate e, int characterHealth)
        {
            return $"rc 2 {e.MateTransportId} {characterHealth} 0";
        }

        public static byte GetMateType(this Mate e, bool simple = false)
        {
            return e.Monster.AttackClass;
        }
        public static void RemovePetBuffs(this Mate e)
        {
            if (e.Owner == null || e.MateType == MateType.Partner)
            {
                return;
            }

            foreach (Buff mateBuff in e.Owner.Buff.Where(b =>
                MateHelper.Instance.MateBuffs.Values.Any(v => v == b.Card.CardId)))
            {
                e.Owner.RemoveBuff(mateBuff.Card.CardId, true);
            }

            e.Owner.Session.SendPacket(e.Owner.GeneratePetskill());
        }

        public static void RemovePartnerSpBuffs(this Mate m)
        {
            if (m.Owner == null) return;
            if (m.MateType != MateType.Partner) return;

            foreach (var partnerSpBuff in m.Owner.BattleEntity.Buffs.Where(b => b.Card.CardId > 2999 && b.Card.CardId < 3042))
            {
                m.Owner.RemoveBuff(partnerSpBuff.Card.CardId, true);
            }
        }

        public static void AddPetBuff(this Mate e)
        {
            if (e.Owner == null)
            {
                return;
            }

            if (e.MateType != MateType.Pet)
            {
                return;
            }

            if (MateHelper.Instance.MateBuffs.TryGetValue(e.NpcMonsterVNum, out var cardId) &&
                e.Owner.Buff.All(b => b.Card.CardId != cardId))
            {
                e.Owner.AddBuff(new Buff((short)cardId, e.Owner.Level, true), e.Owner.BattleEntity);
            }
        
            foreach (NpcMonsterSkill skill in e.Monster.Skills.Where(sk => MateHelper.Instance.PetSkills.Contains(sk.SkillVNum)))
            {
                e.Owner.Session.SendPacket(e.Owner.GeneratePetskill(skill.SkillVNum));
            }
        }

        public static void AddPartnerSpBuff(this Mate p)
        {
            if (p.Owner == null)
            {
                return;
            }

            if (p.MateType != MateType.Partner)
            {
                return;
            }

            int cardId = 0;
            if (p.IsUsingSp)
            {
                if (MateHelper.Instance.PartnerSpBuffs.TryGetValue(p.Sp.Instance.ItemVNum, out cardId) &&
                    p.Owner.Buff.All(b => b.Card.CardId != cardId) && p.Sp.GetSkillsCount() == 1 || p.Sp.GetSkillsCount() == 2 || p.Sp.GetSkillsCount() == 3)
                {
                    if(cardId > 0)
                        p.Owner.AddBuff(new Buff((short)(cardId - 1 + p.Sp.GetSkillsLevels() / 3), p.Level, isPermaBuff: true), p.BattleEntity);
                }
            }
        }
    }
}