using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject
{
    public class ItemInstance : ItemInstanceDTO
    {
        #region Members

        private List<CellonOptionDTO> _cellonOptions;
        private Item _item;
        private List<RuneEffectDTO> _runeEffects;
        private List<ShellEffectDTO> _shellEffects;
        private long _transportId;

        #endregion

        #region Instantiation

        public ItemInstance()
        {
        }

        public ItemInstance(short vNum, short amount)
        {
            ItemVNum = vNum;
            Amount = amount;
            Type = Item.Type;
        }

        public ItemInstance(ItemInstanceDTO input)
        {
            Ammo = input.Ammo;
            Amount = input.Amount;
            BoundCharacterId = input.BoundCharacterId;
            Cellon = input.Cellon;
            CharacterId = input.CharacterId;
            CloseDefence = input.CloseDefence;
            Concentrate = input.Concentrate;
            CriticalDodge = input.CriticalDodge;
            CriticalLuckRate = input.CriticalLuckRate;
            CriticalRate = input.CriticalRate;
            DamageMaximum = input.DamageMaximum;
            DamageMinimum = input.DamageMinimum;
            DarkElement = input.DarkElement;
            DarkResistance = input.DarkResistance;
            DefenceDodge = input.DefenceDodge;
            Design = input.Design;
            DistanceDefence = input.DistanceDefence;
            DistanceDefenceDodge = input.DistanceDefenceDodge;
            DurabilityPoint = input.DurabilityPoint;
            ElementRate = input.ElementRate;
            EquipmentSerialId = input.EquipmentSerialId;
            FireElement = input.FireElement;
            FireResistance = input.FireResistance;
            HitRate = input.HitRate;
            HasSkin = input.HasSkin;
            HoldingVNum = input.HoldingVNum;
            HP = input.HP;
            Id = input.Id;
            IsEmpty = input.IsEmpty;
            IsFixed = input.IsFixed;
            IsPartnerEquipment = input.IsPartnerEquipment;
            ItemDeleteTime = input.ItemDeleteTime;
            ItemVNum = input.ItemVNum;
            LightElement = input.LightElement;
            LightResistance = input.LightResistance;
            MagicDefence = input.MagicDefence;
            MaxElementRate = input.MaxElementRate;
            MP = input.MP;
            Rare = input.Rare;
            ShellRarity = input.ShellRarity;
            SlDamage = input.SlDamage;
            SlDefence = input.SlDefence;
            SlElement = input.SlElement;
            SlHP = input.SlHP;
            Slot = input.Slot;
            SpDamage = input.SpDamage;
            SpDark = input.SpDark;
            SpDefence = input.SpDefence;
            SpElement = input.SpElement;
            SpFire = input.SpFire;
            SpHP = input.SpHP;
            SpLevel = input.SpLevel;
            SpLight = input.SpLight;
            SpStoneUpgrade = input.SpStoneUpgrade;
            SpWater = input.SpWater;
            Type = input.Type;
            Upgrade = input.Upgrade;
            WaterElement = input.WaterElement;
            WaterResistance = input.WaterResistance;
            XP = input.XP;
            RuneAmount = input.RuneAmount;
            IsBreaked = input.IsBreaked;
        }

        #endregion

        #region Properties

        public List<CellonOptionDTO> CellonOptions => _cellonOptions ?? (_cellonOptions = DAOFactory.CellonOptionDAO
                                                          .GetOptionsByWearableInstanceId(
                                                              EquipmentSerialId == Guid.Empty
                                                                  ? EquipmentSerialId = Guid.NewGuid()
                                                                  : EquipmentSerialId).ToList());

        public bool IsBound => BoundCharacterId.HasValue && Item.ItemType != ItemType.Armor &&
                               Item.ItemType != ItemType.Weapon;

        public Item Item => _item ?? (_item = IsPartnerEquipment && HoldingVNum != 0
                                ? ServerManager.GetItem(HoldingVNum)
                                : ServerManager.GetItem(ItemVNum));

        public List<RuneEffectDTO> RuneEffects => _runeEffects ?? (_runeEffects = DAOFactory.RuneEffectDAO
                                                       .LoadByEquipmentSerialId(
                                                           EquipmentSerialId == Guid.Empty
                                                               ? EquipmentSerialId = Guid.NewGuid()
                                                               : EquipmentSerialId).ToList());

        public List<ShellEffectDTO> ShellEffects => _shellEffects ?? (_shellEffects = DAOFactory.ShellEffectDAO
                                                        .LoadByEquipmentSerialId(EquipmentSerialId == Guid.Empty
                                                            ? EquipmentSerialId = Guid.NewGuid()
                                                            : EquipmentSerialId).ToList());

        public long TransportId
        {
            get
            {
                if (_transportId == 0)
                // create transportId thru factory
                {
                    _transportId = TransportFactory.Instance.GenerateTransportId();
                }

                return _transportId;
            }
        }

        #endregion

        #region Methods

        public ItemInstance DeepCopy() => (ItemInstance)MemberwiseClone();

        public string GenerateEInfo()
        {
            var equipmentslot = Item.EquipmentSlot;
            var itemType = Item.ItemType;
            var itemClass = Item.Class;
            var subtype = Item.ItemSubType;
            var test = ItemDeleteTime ?? DateTime.Now;
            var time = ItemDeleteTime != null ? (long)test.Subtract(DateTime.Now).TotalSeconds : 0;
            var seconds = IsBound ? time : Item.ItemValidTime;
            if (seconds < 0)
            {
                seconds = 0;
            }

            var rune =
                $"{RuneAmount} {(IsBreaked ? "1" : "0")} {RuneEffects.Count()} " +
                $"{RuneEffects.Where(x => x.Type != BCardType.CardType.A7Powers1 && x.Type != BCardType.CardType.A7Powers2).Aggregate("", (result, effect) => result += $"{(byte)effect.Type}.{(byte)effect.SubType}.{effect.FirstData * 4}.{effect.SecondData * 4}.{effect.ThirdData} ")} " +
                $"{RuneEffects.Where(x => x.Type == BCardType.CardType.A7Powers1 || x.Type == BCardType.CardType.A7Powers2).Aggregate("", (result, effect) => result += $"{(byte)effect.Type}.{(byte)effect.SubType}.{effect.FirstData * 4}.{effect.SecondData * 4}.{effect.ThirdData} ")}";
            switch (itemType)
            {
                case ItemType.Weapon:
                    switch (equipmentslot)
                    {
                        case EquipmentType.MainWeapon:
                            return
                                $"e_info {(itemClass == 4 ? 1 : itemClass == 8 ? 5 : 0)} {ItemVNum} {Rare} {Upgrade} {(IsFixed ? 1 : 0)} {Item.LevelMinimum} {Item.DamageMinimum + DamageMinimum} {Item.DamageMaximum + DamageMaximum} {Item.HitRate + HitRate} {Item.CriticalLuckRate + CriticalLuckRate} {Item.CriticalRate + CriticalRate} {Ammo} {Item.MaximumAmmo} {Item.SellToNpcPrice} {(IsPartnerEquipment ? $"{HoldingVNum}" : "-1")} {(ShellRarity == null ? "0" : $"{ShellRarity}")} {BoundCharacterId ?? 0} {ShellEffects.Count} {ShellEffects.Aggregate("", (result, effect) => result += $"{(byte)effect.EffectLevel}.{effect.Effect}.{(byte)effect.Value} ")} {rune}";

                        case EquipmentType.SecondaryWeapon:
                            return
                                $"e_info {(itemClass <= 2 ? 1 : 0)} {ItemVNum} {Rare} {Upgrade} {(IsFixed ? 1 : 0)} {Item.LevelMinimum} {Item.DamageMinimum + DamageMinimum} {Item.DamageMaximum + DamageMaximum} {Item.HitRate + HitRate} {Item.CriticalLuckRate + CriticalLuckRate} {Item.CriticalRate + CriticalRate} {Ammo} {Item.MaximumAmmo} {Item.SellToNpcPrice} {(IsPartnerEquipment ? $"{HoldingVNum}" : "-1")} {(ShellRarity == null ? "0" : $"{ShellRarity}")} {BoundCharacterId ?? 0} {ShellEffects.Count} {ShellEffects.Aggregate("", (result, effect) => result += $"{(byte)effect.EffectLevel}.{effect.Effect}.{(byte)effect.Value} ")}";
                    }

                    break;

                case ItemType.Armor:
                    return
                        $"e_info 2 {ItemVNum} {Rare} {Upgrade} {(IsFixed ? 1 : 0)} {Item.LevelMinimum} {Item.CloseDefence + CloseDefence} {Item.DistanceDefence + DistanceDefence} {Item.MagicDefence + MagicDefence} {Item.DefenceDodge + DefenceDodge} {Item.SellToNpcPrice} {(IsPartnerEquipment ? $"{HoldingVNum}" : "-1")} {(ShellRarity == null ? "0" : $"{ShellRarity}")} {BoundCharacterId ?? 0} {ShellEffects.Count} {ShellEffects.Aggregate("", (result, effect) => result += $"{((byte)effect.EffectLevel > 12 ? (byte)effect.EffectLevel - 12 : (byte)effect.EffectLevel)}.{(effect.Effect > 50 ? effect.Effect - 50 : effect.Effect)}.{(byte)effect.Value} ")}";

                case ItemType.Fashion:
                    switch (equipmentslot)
                    {
                        case EquipmentType.CostumeHat:
                            return
                                $"e_info 3 {ItemVNum} {Item.LevelMinimum} {Item.CloseDefence + CloseDefence} {Item.DistanceDefence + DistanceDefence} {Item.MagicDefence + MagicDefence} {Item.DefenceDodge + DefenceDodge} {Item.FireResistance + FireResistance} {Item.WaterResistance + WaterResistance} {Item.LightResistance + LightResistance} {Item.DarkResistance + DarkResistance} {Item.SellToNpcPrice} {(Item.ItemValidTime == 0 ? -1 : 0)} 2 {(Item.ItemValidTime == 0 ? -1 : seconds / 3600)}";

                        case EquipmentType.CostumeSuit:
                            return
                                $"e_info 2 {ItemVNum} {Rare} {Upgrade} {(IsFixed ? 1 : 0)} {Item.LevelMinimum} {Item.CloseDefence + CloseDefence} {Item.DistanceDefence + DistanceDefence} {Item.MagicDefence + MagicDefence} {Item.DefenceDodge + DefenceDodge} {Item.SellToNpcPrice} {(Item.ItemValidTime == 0 ? -1 : 0)} 1 {(Item.ItemValidTime == 0 ? -1 : seconds / 3600)}"; // 1 = IsCosmetic -1 = no shells

                        default:
                            return
                                $"e_info 3 {ItemVNum} {Item.LevelMinimum} {Item.CloseDefence + CloseDefence} {Item.DistanceDefence + DistanceDefence} {Item.MagicDefence + MagicDefence} {Item.DefenceDodge + DefenceDodge} {Item.FireResistance + FireResistance} {Item.WaterResistance + WaterResistance} {Item.LightResistance + LightResistance} {Item.DarkResistance + DarkResistance} {Item.SellToNpcPrice} {Upgrade} 0 -1"; // after Item.Price theres TimesConnected {(Item.ItemValidTime == 0 ? -1 : Item.ItemValidTime / (3600))}
                    }

                case ItemType.Jewelery:
                    switch (equipmentslot)
                    {
                        case EquipmentType.Amulet:
                            if (DurabilityPoint > 0)
                            {
                                return
                                        $"e_info 4 {ItemVNum} {Item.LevelMinimum} {DurabilityPoint} 100 0 {Item.SellToNpcPrice}";
                            }

                            return $"e_info 4 {ItemVNum} {Item.LevelMinimum} {seconds * 10} 0 0 {Item.SellToNpcPrice}";

                        case EquipmentType.Fairy:
                            return
                                $"e_info 4 {ItemVNum} {Item.Element} {ElementRate + Item.ElementRate} 0 0 0 0 0"; // last IsNosmall

                        default:
                            var cellon = "";
                            foreach (var option in CellonOptions)
                            {
                                cellon += $" {(byte)option.Type} {option.Level} {option.Value}";
                            }

                            return
                                $"e_info 4 {ItemVNum} {Item.LevelMinimum} {Item.MaxCellonLvl} {Item.MaxCellon} {CellonOptions.Count} {Item.SellToNpcPrice}{cellon}";
                    }
                case ItemType.Specialist:
                    return $"e_info 8 {ItemVNum}";

                case ItemType.Box:
                    switch (subtype)
                    {
                        case 0:
                        case 1:
                            return HoldingVNum == 0
                                ? $"e_info 7 {ItemVNum} 0"
                                : $"e_info 7 {ItemVNum} 1 {HoldingVNum} {SpLevel} {XP} 100 {SpDamage} {SpDefence}";

                        case 2:
                            var spitem = ServerManager.GetItem(HoldingVNum);
                            return HoldingVNum == 0
                                ? $"e_info 7 {ItemVNum} 0"
                                : $"e_info 7 {ItemVNum} 1 {HoldingVNum} {SpLevel} {XP} {CharacterHelper.SPXPData[SpLevel == 0 ? 0 : SpLevel - 1]} {Upgrade} {CharacterHelper.SlPoint(SlDamage, 0)} {CharacterHelper.SlPoint(SlDefence, 1)} {CharacterHelper.SlPoint(SlElement, 2)} {CharacterHelper.SlPoint(SlHP, 3)} {CharacterHelper.SPPoint(SpLevel, Upgrade) - SlDamage - SlHP - SlElement - SlDefence} {SpStoneUpgrade} {spitem.FireResistance} {spitem.WaterResistance} {spitem.LightResistance} {spitem.DarkResistance} {SpDamage} {SpDefence} {SpElement} {SpHP} {SpFire} {SpWater} {SpLight} {SpDark}";

                        case 4:
                            return HoldingVNum == 0
                                ? $"e_info 11 {ItemVNum} 0"
                                : $"e_info 11 {ItemVNum} 1 {HoldingVNum}";

                        case 5:
                            var fairyitem = ServerManager.GetItem(HoldingVNum);
                            return HoldingVNum == 0
                                ? $"e_info 12 {ItemVNum} 0"
                                : $"e_info 12 {ItemVNum} 1 {HoldingVNum} {ElementRate + fairyitem.ElementRate}";

                        case 6:
                            var packet = string.Empty;
                            foreach (var skill in DAOFactory.PartnerSkillDAO.LoadByEquipmentSerialId(EquipmentSerialId))
                            {
                                packet += $"{skill.SkillVNum} {skill.Level} ";
                            }

                            var data = packet.Split();
                            var output = data.Length == 1 ? "0 0 0 0 0 0" :
                                data.Length == 3 ? $"{packet}0 0 0 0" :
                                data.Length == 5 ? $"{packet}0 0" : $"{packet}";

                            return HoldingVNum == 0
                                ? $"e_info 13 {ItemVNum} 0"
                                : $"e_info 13 {ItemVNum} 1 {HoldingVNum} 1 {output}";

                        default:
                            return $"e_info 8 {ItemVNum} {Design} {Rare}";
                    }

                case ItemType.Shell:
                    return
                        $"e_info 9 {ItemVNum} {Upgrade} {Rare} {Item.SellToNpcPrice} {ShellEffects.Count}{ShellEffects.Aggregate("", (current, option) => current + $" {((byte)option.EffectLevel > 12 ? (byte)option.EffectLevel - 12 : (byte)option.EffectLevel)}.{(option.Effect > 50 ? option.Effect - 50 : option.Effect)}.{option.Value}")}";
            }

            return "";
        }

        public string GenerateFStash() => $"f_stash {GenerateStashPacket()}";

        public void GenerateHeroicShell(RarifyProtection protection, bool forced = false)
        {
            byte shellType;
            if (protection != RarifyProtection.RandomHeroicAmulet)
            {
                return;
            }

            if (Item.ItemType == ItemType.Jewelery)
            {
                return;
            }

            if (!Item.IsHeroic || Rare <= 0)
            {
                return;
            }

            if (Rare < 8)
            {
                shellType = (byte)(Item.ItemType == ItemType.Armor ? 11 : 10);
                if (shellType != 11 && shellType != 10)
                {
                    return;
                }
            }
            else
            {
                var possibleTypes = new List<byte> { 4, 5, 6, 7 };
                var probability = ServerManager.RandomNumber();
                shellType = (byte)(Item.ItemType == ItemType.Armor
                    ? probability > 50 ? 5 : 7
                    : probability > 50
                        ? 4
                        : 6);
                if (!possibleTypes.Contains(shellType))
                {
                    return;
                }
            }

            
            DAOFactory.ShellEffectDAO.DeleteByEquipmentSerialId(EquipmentSerialId);
            var shellLevel = Item.LevelMinimum == 25 ? 101 : 106;
            AddShellsFixed(ShellGeneratorHelper.Instance.GenerateShell(shellType, Rare == 8 ? 7 : Rare, shellLevel));
        }
        public void AddShellsFixed(List<ShellEffectDTO> list)
        {
            DAOFactory.ShellEffectDAO.DeleteByEquipmentSerialId(this.EquipmentSerialId);
            ShellEffects.Clear();
            ShellEffects.AddRange(list);
        }
        public void GenerateHeroicShellFutur(ClientSession session, RarifyProtection protection)
        {
            if (protection != RarifyProtection.RandomHeroicAmulet)
            {
                return;
            }

            if (Item.ItemType == ItemType.Jewelery)
            {
                return;
            }

            if (!Item.IsHeroic || Rare <= 0)
            {
                return;
            }

            ShellRarity = null;
            ShellEffects.Clear();
            DAOFactory.ShellEffectDAO.DeleteByEquipmentSerialId(EquipmentSerialId);
            SetShellEffects(true);
            ShellRarity = Rare;
            BoundCharacterId = session.Character.CharacterId;
        }

        public string GenerateInventoryAdd()
        {
            switch (Type)
            {
                case InventoryType.Equipment:
                    return
                        $"ivn 0 {Slot}.{ItemVNum}.{Rare}.{(Item.IsColored ? Design : Upgrade)}.{SpStoneUpgrade}.{RuneAmount}";

                case InventoryType.Main:
                    return $"ivn 1 {Slot}.{ItemVNum}.{Amount}.0";

                case InventoryType.Etc:
                    return $"ivn 2 {Slot}.{ItemVNum}.{Amount}.0";

                case InventoryType.Miniland:
                    return $"ivn 3 {Slot}.{ItemVNum}.{Amount}";

                case InventoryType.Specialist:
                    return $"ivn 6 {Slot}.{ItemVNum}.{Rare}.{Upgrade}.{SpStoneUpgrade}";

                case InventoryType.Costume:
                    return $"ivn 7 {Slot}.{ItemVNum}.{Rare}.{Upgrade}.0";
            }

            return "";
        }

        public string GeneratePslInfo()
        {
            var partnerSp = new PartnerSp(this);

            return
                $"pslinfo {Item.VNum} {Item.Element} {Item.ElementRate} {Item.LevelJobMinimum} {Item.Speed} {Item.FireResistance} {Item.WaterResistance} {Item.LightResistance} {Item.DarkResistance}{partnerSp.GenerateSkills()}";
        }

        public string GeneratePStash() => $"pstash {GenerateStashPacket()}";

        public string GenerateReqInfo()
        {
            byte type = 0;
            if (BoundCharacterId != null && BoundCharacterId != CharacterId)
            {
                type = 2;
            }

            return $"r_info {ItemVNum} {type} {0}";
        }

        public string GenerateSlInfo(ClientSession session = null)
        {
            var freepoint = CharacterHelper.SPPoint(SpLevel, Upgrade) - SlDamage - SlHP - SlElement - SlDefence;
            var slElementShell = 0;
            var slHpShell = 0;
            var slDefenceShell = 0;
            var slHitShell = 0;

            if (session != null)
            {
                var mainWeapon =
                    session.Character?.Inventory.LoadBySlotAndType((byte)EquipmentType.MainWeapon, InventoryType.Wear);
                var secondaryWeapon =
                    session.Character?.Inventory.LoadBySlotAndType((byte)EquipmentType.SecondaryWeapon,
                        InventoryType.Wear);

                var effects = new List<ShellEffectDTO>();
                if (mainWeapon?.ShellEffects != null)
                {
                    effects.AddRange(mainWeapon.ShellEffects);
                }

                if (secondaryWeapon?.ShellEffects != null)
                {
                    effects.AddRange(secondaryWeapon.ShellEffects);
                }

                int GetShellWeaponEffectValue(ShellWeaponEffectType effectType)
                {
                    return effects?.Where(s => s.Effect == (byte)effectType)?.OrderByDescending(s => s.Value)
                               ?.FirstOrDefault()?.Value ?? 0;
                }

                slElementShell = GetShellWeaponEffectValue(ShellWeaponEffectType.SLElement) +
                                 GetShellWeaponEffectValue(ShellWeaponEffectType.SLGlobal) +
                                 session.Character.GetTitleEffectValue(BCardType.CardType.IncreaseSlPoint,
                                     (byte) AdditionalTypes.IncreaseSlPoint.IncreaseEllement);
                slHpShell = GetShellWeaponEffectValue(ShellWeaponEffectType.SLHP) +
                            GetShellWeaponEffectValue(ShellWeaponEffectType.SLGlobal) +
                            session.Character.GetTitleEffectValue(BCardType.CardType.IncreaseSlPoint,
                                (byte) AdditionalTypes.IncreaseSlPoint.IncreaseHPMP);
                slDefenceShell = GetShellWeaponEffectValue(ShellWeaponEffectType.SLDefence) +
                                 GetShellWeaponEffectValue(ShellWeaponEffectType.SLGlobal) +
                                 session.Character.GetTitleEffectValue(BCardType.CardType.IncreaseSlPoint,
                                     (byte) AdditionalTypes.IncreaseSlPoint.IncreaseDefence);
                slHitShell = GetShellWeaponEffectValue(ShellWeaponEffectType.SLDamage) +
                             GetShellWeaponEffectValue(ShellWeaponEffectType.SLGlobal) +
                             session.Character.GetTitleEffectValue(BCardType.CardType.IncreaseSlPoint,
                                 (byte) AdditionalTypes.IncreaseSlPoint.IncreaseDamage);
            }

            var slElement = CharacterHelper.SlPoint(SlElement, 2);
            var slHp = CharacterHelper.SlPoint(SlHP, 3);
            var slDefence = CharacterHelper.SlPoint(SlDefence, 1);
            var slHit = CharacterHelper.SlPoint(SlDamage, 0);

            var skills = new StringBuilder();

            var skillsSp = new List<CharacterSkill>();

            foreach (var ski in ServerManager.GetAllSkill()
                .Where(ski =>
                    ski.UpgradeType == Item.Morph && ski.SkillType == (byte)SkillType.CharacterSKill &&
                    ski.LevelMinimum <= SpLevel).OrderBy(s => s.SkillVNum))
            {
                skillsSp.Add(new CharacterSkill
                {
                    SkillVNum = ski.SkillVNum,
                    CharacterId = CharacterId
                });
            }

            byte spdestroyed = 0;

            if (Rare == -2)
            {
                spdestroyed = 1;
            }

            if (skillsSp.Count == 0)
            {
                skills.Append("-1");
            }
            else
            {
                var firstSkillVNum = skillsSp[0].SkillVNum;

                for (var i = 1; i < 11; i++)
                {
                    if (skillsSp.Count >= i + 1 && skillsSp[i].SkillVNum <= firstSkillVNum + 10)
                    {
                        if (skills.Length > 0)
                        {
                            skills.Append(".");
                        }

                        skills.Append($"{skillsSp[i].SkillVNum}");
                    }
                }
            }

            // 10 9 8 '0 0 0 0'<- bonusdamage bonusarmor bonuselement bonushpmp its after upgrade
            // and 3 first values are not important

            return
                $"slinfo {(Type == InventoryType.Wear || Type == InventoryType.Specialist || Type == InventoryType.Equipment ? "0" : "2")} {ItemVNum} {Item.Morph} {SpLevel} {Item.LevelJobMinimum} {Item.ReputationMinimum} 0 {Item.Speed} 0 0 0 0 0 {Item.SpType} {Item.FireResistance} {Item.WaterResistance} {Item.LightResistance} {Item.DarkResistance} {XP} {CharacterHelper.SPXPData[SpLevel == 0 ? 0 : SpLevel - 1]} {skills} {TransportId} {freepoint} {slHit} {slDefence} {slElement} {slHp} {Upgrade} 0 0 {spdestroyed} {slHitShell} {slDefenceShell} {slElementShell} {slHpShell} {SpStoneUpgrade} {SpDamage} {SpDefence} {SpElement} {SpHP} {SpFire} {SpWater} {SpLight} {SpDark}";
        }

        public string GenerateStash() => $"stash {GenerateStashPacket()}";

        public string GenerateStashPacket()
        {
            var packet = $"{Slot}.{ItemVNum}.{(byte)Item.Type}";
            switch (Item.Type)
            {
                case InventoryType.Equipment:
                    return packet + $".{Amount}.{Rare}.{Upgrade}";

                case InventoryType.Specialist:
                    return packet + $".{Upgrade}.{SpStoneUpgrade}.0";

                default:
                    return packet + $".{Amount}.0.0";
            }
        }

        public ItemInstance HardItemCopy() => new ItemInstance
        {
            Ammo = Ammo,
            Amount = Amount,
            BoundCharacterId = BoundCharacterId,
            Cellon = Cellon,
            CharacterId = CharacterId,
            CloseDefence = CloseDefence,
            Concentrate = Concentrate,
            CriticalDodge = CriticalDodge,
            CriticalLuckRate = CriticalLuckRate,
            CriticalRate = CriticalRate,
            DamageMaximum = DamageMaximum,
            DamageMinimum = DamageMinimum,
            DarkElement = DarkElement,
            DarkResistance = DarkResistance,
            DefenceDodge = DefenceDodge,
            Design = Design,
            DistanceDefence = DistanceDefence,
            DistanceDefenceDodge = DistanceDefenceDodge,
            DurabilityPoint = DurabilityPoint,
            ElementRate = ElementRate,
            EquipmentSerialId = EquipmentSerialId,
            FireElement = FireElement,
            FireResistance = FireResistance,
            HitRate = HitRate,
            HoldingVNum = HoldingVNum,
            HP = HP,
            Id = Id,
            IsEmpty = IsEmpty,
            IsFixed = IsFixed,
            IsPartnerEquipment = IsPartnerEquipment,
            ItemDeleteTime = ItemDeleteTime,
            ItemVNum = ItemVNum,
            LightElement = LightElement,
            LightResistance = LightResistance,
            MagicDefence = MagicDefence,
            MaxElementRate = MaxElementRate,
            MP = MP,
            Rare = Rare,
            ShellRarity = ShellRarity,
            SlDamage = SlDamage,
            SlDefence = SlDefence,
            SlElement = SlElement,
            SlHP = SlHP,
            Slot = Slot,
            SpDamage = SpDamage,
            SpDark = SpDark,
            SpDefence = SpDefence,
            SpElement = SpElement,
            SpFire = SpFire,
            SpHP = SpHP,
            SpLevel = SpLevel,
            SpLight = SpLight,
            SpStoneUpgrade = SpStoneUpgrade,
            SpWater = SpWater,
            Type = Type,
            Upgrade = Upgrade,
            WaterElement = WaterElement,
            WaterResistance = WaterResistance,
            XP = XP
        };

        public void SetRarityPoint()
        {
            switch (Item.EquipmentSlot)
            {
                case EquipmentType.MainWeapon:
                case EquipmentType.SecondaryWeapon:
                    {
                        var point = CharacterHelper.RarityPoint(Rare,
                            Item.IsHeroic ? (short)(95 + Item.LevelMinimum) : Item.LevelMinimum, false);
                        Concentrate = 0;
                        HitRate = 0;
                        DamageMinimum = 0;
                        DamageMaximum = 0;
                        if (Rare >= 0)
                        {
                            for (var i = 0; i < point; i++)
                            {
                                var rndn = ServerManager.RandomNumber(0, 3);
                                if (rndn == 0)
                                {
                                    Concentrate++;
                                    HitRate++;
                                }
                                else
                                {
                                    DamageMinimum++;
                                    DamageMaximum++;
                                }
                            }
                        }
                        else
                        {
                            for (var i = 0; i > Rare * 10; i--)
                            {
                                DamageMinimum--;
                                DamageMaximum--;
                            }
                        }
                    }
                    break;

                case EquipmentType.Armor:
                    {
                        var point = CharacterHelper.RarityPoint(Rare,
                            Item.IsHeroic ? (short)(95 + Item.LevelMinimum) : Item.LevelMinimum, true);
                        DefenceDodge = 0;
                        DistanceDefenceDodge = 0;
                        DistanceDefence = 0;
                        MagicDefence = 0;
                        CloseDefence = 0;
                        double NewDistanceDefence = 0;
                        double NewMagicDefence = 0;
                        double NewCloseDefence = 0;
                        if (Rare >= 0)
                        {
                            for (var i = 0; i < point; i++)
                            {
                                var rndn = ServerManager.RandomNumber(0, 5);
                                if (rndn == 0)
                                {
                                    DefenceDodge++;
                                    DistanceDefenceDodge++;
                                }
                                else
                                {
                                    NewDistanceDefence = NewDistanceDefence + 0.9;
                                    NewMagicDefence = NewMagicDefence + 0.35;
                                    NewCloseDefence = NewCloseDefence + 0.95;
                                }
                            }

                            DistanceDefence = (short)NewDistanceDefence;
                            MagicDefence = (short)NewMagicDefence;
                            CloseDefence = (short)NewCloseDefence;
                        }
                        else
                        {
                            for (var i = 0; i > Rare * 10; i--)
                            {
                                DistanceDefence--;
                                MagicDefence--;
                                CloseDefence--;
                            }
                        }
                    }
                    break;
            }
        }

        public void SetShellEffects(bool champion = false)
        {
            byte CNormCount = 0;
            byte BNormCount = 0;
            byte ANormCount = 0;
            byte SNormCount = 0;
            byte CBonusMax = 0;
            byte BBonusMax = 0;
            byte ABonusMax = 0;
            byte SBonusMax = 0;
            byte CPVPMax = 0;
            byte BPVPMax = 0;
            byte APVPMax = 0;
            byte SPVPMax = 0;

            byte ShellType = 0;
            var IsWeapon = true;
            switch (ItemVNum)
            {
                case 589:
                case 590:
                case 591:
                case 592:
                case 593:
                case 594:
                case 595:
                case 596:
                case 597:
                case 598:
                    ShellType = 0;
                    break;

                case 565:
                case 566:
                case 567:
                    ShellType = 1;
                    break;

                case 568:
                case 569:
                case 570:
                    ShellType = 2;
                    break;

                case 571:
                case 572:
                case 573:
                    ShellType = 3;
                    break;

                case 574:
                case 575:
                case 576:
                    ShellType = 4;
                    break;

                case 599:
                case 656:
                case 657:
                case 658:
                case 659:
                case 660:
                case 661:
                case 662:
                case 663:
                case 664:
                    ShellType = 0;
                    IsWeapon = false;
                    break;

                case 577:
                case 578:
                case 579:
                    ShellType = 1;
                    IsWeapon = false;
                    break;

                case 580:
                case 581:
                case 582:
                    ShellType = 2;
                    IsWeapon = false;
                    break;

                case 583:
                case 584:
                case 585:
                    ShellType = 3;
                    IsWeapon = false;
                    break;

                case 586:
                case 587:
                case 588:
                    ShellType = 4;
                    IsWeapon = false;
                    break;

                default:
                    ShellType = 3;
                    IsWeapon = true;
                    if (champion)
                    {
                        IsWeapon = Item.ItemType == ItemType.Weapon;
                        var rnd = ServerManager.RandomNumber(0, 5);
                        ShellType = rnd switch
                        {
                            0 => 0,
                            1 => 1,
                            2 => 2,
                            3 => 3,
                            4 => 4,
                            _ => 1
                        };
                    }

                    break;
            }

            switch (Rare)
            {
                case 1:
                    switch (ShellType)
                    {
                        case 0:
                            CNormCount = 1;
                            break;

                        case 1:
                            CNormCount = 1;
                            break;

                        case 2:
                            CNormCount = 1;
                            CBonusMax = 1;
                            break;

                        case 3:
                            CNormCount = 1;
                            CPVPMax = 1;
                            break;

                        case 4:
                            CNormCount = 1;
                            CBonusMax = 1;
                            CPVPMax = 1;
                            break;
                    }

                    break;

                case 2:
                    switch (ShellType)
                    {
                        case 0:
                            CNormCount = 2;
                            break;

                        case 1:
                            CNormCount = 2;
                            break;

                        case 2:
                            CNormCount = 1;
                            CBonusMax = 1;
                            break;

                        case 3:
                            CNormCount = 1;
                            CPVPMax = 2;
                            break;

                        case 4:
                            CNormCount = 2;
                            CBonusMax = 1;
                            CPVPMax = 1;
                            break;
                    }

                    break;

                case 3:
                    switch (ShellType)
                    {
                        case 0:
                            CNormCount = 2;
                            BNormCount = 1;
                            break;

                        case 1:
                            CNormCount = 2;
                            BNormCount = 1;
                            break;

                        case 2:
                            CNormCount = 1;
                            BNormCount = 1;
                            BBonusMax = 1;
                            break;

                        case 3:
                            CNormCount = 1;
                            BNormCount = 1;
                            BPVPMax = 1;
                            break;

                        case 4:
                            CNormCount = 2;
                            BNormCount = 2;
                            BBonusMax = 1;
                            break;
                    }

                    break;

                case 4:
                    switch (ShellType)
                    {
                        case 0:
                            CNormCount = 2;
                            BNormCount = 2;
                            break;

                        case 1:
                            CNormCount = 2;
                            BNormCount = 2;
                            break;

                        case 2:
                            CNormCount = 1;
                            BNormCount = 1;
                            BBonusMax = 1;
                            break;

                        case 3:
                            CNormCount = 1;
                            BNormCount = 1;
                            BPVPMax = 2;
                            break;

                        case 4:
                            CNormCount = 2;
                            BNormCount = 2;
                            BBonusMax = 1;
                            BPVPMax = 1;
                            break;
                    }

                    break;

                case 5:
                    switch (ShellType)
                    {
                        case 0:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 1;
                            break;

                        case 1:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 1;
                            break;

                        case 2:
                            CNormCount = 1;
                            BNormCount = 1;
                            ANormCount = 1;
                            ABonusMax = 1;
                            break;

                        case 3:
                            CNormCount = 1;
                            BNormCount = 1;
                            ANormCount = 1;
                            BPVPMax = 2;
                            APVPMax = 1;
                            break;

                        case 4:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 1;
                            ABonusMax = 1;
                            BPVPMax = 1;
                            break;
                    }

                    break;

                case 6:
                    switch (ShellType)
                    {
                        case 0:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 2;
                            break;

                        case 1:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 2;
                            break;

                        case 2:
                            CNormCount = 1;
                            BNormCount = 1;
                            ANormCount = 2;
                            ABonusMax = 1;
                            break;

                        case 3:
                            CNormCount = 1;
                            BNormCount = 1;
                            ANormCount = 2;
                            BPVPMax = 2;
                            APVPMax = 2;
                            break;

                        case 4:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 2;
                            ABonusMax = 1;
                            BPVPMax = 1;
                            APVPMax = 1;
                            break;
                    }

                    break;

                case 7:
                    switch (ShellType)
                    {
                        case 0:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 2;
                            break;

                        case 1:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 2;
                            SNormCount = 1;
                            break;

                        case 2:
                            CNormCount = 1;
                            BNormCount = 1;
                            ANormCount = 2;
                            SNormCount = 1;
                            SBonusMax = 1;
                            break;

                        case 3:
                            CNormCount = 1;
                            BNormCount = 1;
                            ANormCount = 2;
                            SNormCount = 1;
                            BPVPMax = 2;
                            APVPMax = 2;
                            SPVPMax = 2;
                            break;

                        case 4:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 2;
                            SNormCount = 1;
                            SBonusMax = 1;
                            BPVPMax = 1;
                            APVPMax = 1;
                            SPVPMax = 1;
                            break;
                    }

                    break;

                case 8:
                    switch (ShellType)
                    {
                        case 0:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 2;
                            break;

                        case 1:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 2;
                            SNormCount = 2;
                            break;

                        case 2:
                            CNormCount = 1;
                            BNormCount = 1;
                            ANormCount = 2;
                            SNormCount = 2;
                            SBonusMax = 2;
                            break;

                        case 3:
                            CNormCount = 1;
                            BNormCount = 1;
                            ANormCount = 2;
                            SNormCount = 2;
                            BPVPMax = 2;
                            APVPMax = 2;
                            SPVPMax = 3;
                            break;

                        case 4:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 2;
                            SNormCount = 2;
                            SBonusMax = 2;
                            BPVPMax = 1;
                            APVPMax = 1;
                            SPVPMax = 2;
                            break;
                    }

                    break;
            }

            var effectsList = new List<ShellEffectDTO>();

            if (!IsWeapon && SPVPMax == 3)
            {
                SPVPMax = 2;
            }

            if (EquipmentSerialId == Guid.Empty)
            {
                EquipmentSerialId = Guid.NewGuid();
            }

            short CalculateEffect(short maximum)
            {
                if (maximum == 0)
                {
                    return 1;
                }

                double multiplier = 0;
                switch (Rare)
                {
                    case 0:
                    case 1:
                    case 2:
                        multiplier = 0.6;
                        break;

                    case 3:
                        multiplier = 0.65;
                        break;

                    case 4:
                        multiplier = 0.75;
                        break;

                    case 5:
                        multiplier = 0.85;
                        break;

                    case 6:
                        multiplier = 0.95;
                        break;

                    case 7:
                    case 8:
                        multiplier = 1;
                        break;
                }

                var min = (short)(maximum / 80D * (champion ? Item.LevelMinimum == 25 ? 45 : 60 : Upgrade - 20) *
                                   multiplier);
                var max =
                    (short)(maximum / 80D * (champion ? Item.LevelMinimum == 25 ? 60 : 85 : Upgrade) * multiplier);
                if (min == 0)
                {
                    min = 1;
                }

                if (max <= min)
                {
                    max = (short)(min + 1);
                }

                return (short)ServerManager.RandomNumber(min, max);
            }

            void AddEffect(ShellEffectLevelType levelType)
            {
                var i = 0;
                while (i < 10)
                {
                    i++;
                    switch (levelType)
                    {
                        case ShellEffectLevelType.CNormal:
                            {
                                byte[] effects =
                                {
                                (byte) ShellWeaponEffectType.DamageImproved,
                                (byte) ShellWeaponEffectType.DamageIncreasedtothePlant,
                                (byte) ShellWeaponEffectType.DamageIncreasedtotheAnimal,
                                (byte) ShellWeaponEffectType.DamageIncreasedtotheEnemy,
                                (byte) ShellWeaponEffectType.CriticalChance,
                                (byte) ShellWeaponEffectType.CriticalDamage,
                                (byte) ShellWeaponEffectType.AntiMagicDisorder,
                                (byte) ShellWeaponEffectType.ReducedMPConsume,
                                (byte) ShellWeaponEffectType.Blackout,
                                (byte) ShellWeaponEffectType.MinorBleeding
                            };
                                short[] maximum = { 80, 8, 8, 8, 10, 50, 0, 10, 4, 4 };

                                if (!IsWeapon)
                                {
                                    effects = new[]
                                    {
                                    (byte) ShellArmorEffectType.CloseDefence,
                                    (byte) ShellArmorEffectType.DistanceDefence,
                                    (byte) ShellArmorEffectType.MagicDefence,
                                    (byte) ShellArmorEffectType.ReducedCritChanceRecive,
                                    (byte) ShellArmorEffectType.ReducedStun,
                                    (byte) ShellArmorEffectType.ReducedMinorBleeding,
                                    (byte) ShellArmorEffectType.ReducedShock,
                                    (byte) ShellArmorEffectType.ReducedPoisonParalysis,
                                    (byte) ShellArmorEffectType.ReducedBlind,
                                    (byte) ShellArmorEffectType.RecoveryHPOnRest,
                                    (byte) ShellArmorEffectType.RecoveryMPOnRest
                                };
                                    maximum = new short[] { 55, 55, 55, 8, 30, 30, 45, 15, 30, 48, 48 };
                                }

                                var position = ServerManager.RandomNumber(0, effects.Length);
                                var effect = effects[position];
                                var value = CalculateEffect(maximum[position]);

                                if (effectsList.Any(s => s.Effect == effect))
                                {
                                    continue;
                                }

                                effectsList.Add(new ShellEffectDTO
                                {
                                    EffectLevel = ShellEffectLevelType.CNormal,
                                    Effect = effect,
                                    Value = value,
                                    EquipmentSerialId = EquipmentSerialId
                                });
                                return;
                            }
                        case ShellEffectLevelType.BNormal:
                            {
                                byte[] effects =
                                {
                                (byte) ShellWeaponEffectType.DamageImproved,
                                (byte) ShellWeaponEffectType.DamageIncreasedtothePlant,
                                (byte) ShellWeaponEffectType.DamageIncreasedtotheAnimal,
                                (byte) ShellWeaponEffectType.DamageIncreasedtotheEnemy,
                                (byte) ShellWeaponEffectType.DamageIncreasedtotheUnDead,
                                (byte) ShellWeaponEffectType.DamageincreasedtotheSmallMonster,
                                (byte) ShellWeaponEffectType.CriticalChance,
                                (byte) ShellWeaponEffectType.CriticalDamage,
                                (byte) ShellWeaponEffectType.ReducedMPConsume,
                                (byte) ShellWeaponEffectType.Freeze,
                                (byte) ShellWeaponEffectType.Bleeding,
                                (byte) ShellWeaponEffectType.IncreasedFireProperties,
                                (byte) ShellWeaponEffectType.IncreasedWaterProperties,
                                (byte) ShellWeaponEffectType.IncreasedLightProperties,
                                (byte) ShellWeaponEffectType.IncreasedDarkProperties,
                                (byte) ShellWeaponEffectType.SLDamage,
                                (byte) ShellWeaponEffectType.SLDefence,
                                (byte) ShellWeaponEffectType.SLElement,
                                (byte) ShellWeaponEffectType.SLHP,
                                (byte) ShellWeaponEffectType.HPRecoveryForKilling,
                                (byte) ShellWeaponEffectType.MPRecoveryForKilling
                            };
                                short[] maximum =
                                    {120, 16, 16, 16, 8, 8, 16, 75, 20, 4, 4, 61, 61, 61, 61, 10, 10, 10, 10, 150, 150};

                                if (!IsWeapon)
                                {
                                    effects = new[]
                                    {
                                    (byte) ShellArmorEffectType.CloseDefence,
                                    (byte) ShellArmorEffectType.DistanceDefence,
                                    (byte) ShellArmorEffectType.MagicDefence,
                                    (byte) ShellArmorEffectType.ReducedCritChanceRecive,
                                    (byte) ShellArmorEffectType.ReducedBlind,
                                    (byte) ShellArmorEffectType.RecoveryHPOnRest,
                                    (byte) ShellArmorEffectType.RecoveryMPOnRest,
                                    (byte) ShellArmorEffectType.ReducedBleedingAndMinorBleeding,
                                    (byte) ShellArmorEffectType.ReducedArmorDeBuff,
                                    (byte) ShellArmorEffectType.ReducedFreeze,
                                    (byte) ShellArmorEffectType.ReducedParalysis,
                                    (byte) ShellArmorEffectType.IncreasedFireResistence,
                                    (byte) ShellArmorEffectType.IncreasedWaterResistence,
                                    (byte) ShellArmorEffectType.IncreasedLightResistence,
                                    (byte) ShellArmorEffectType.IncreasedDarkResistence
                                };
                                    maximum = new short[] { 95, 95, 95, 13, 40, 85, 85, 27, 42, 38, 27, 8, 8, 8, 8 };
                                }

                                var position = ServerManager.RandomNumber(0, effects.Length);
                                var effect = effects[position];
                                var value = CalculateEffect(maximum[position]);

                                if (effectsList.Any(s => s.Effect == effect))
                                {
                                    continue;
                                }

                                effectsList.Add(new ShellEffectDTO
                                {
                                    EffectLevel = ShellEffectLevelType.BNormal,
                                    Effect = effect,
                                    Value = value,
                                    EquipmentSerialId = EquipmentSerialId
                                });
                                return;
                            }
                        case ShellEffectLevelType.ANormal:
                            {
                                byte[] effects =
                                {
                                (byte) ShellWeaponEffectType.DamageImproved,
                                (byte) ShellWeaponEffectType.DamageIncreasedtotheUnDead,
                                (byte) ShellWeaponEffectType.DamageincreasedtotheSmallMonster,
                                (byte) ShellWeaponEffectType.HeavyBleeding,
                                (byte) ShellWeaponEffectType.IncreasedFireProperties,
                                (byte) ShellWeaponEffectType.IncreasedWaterProperties,
                                (byte) ShellWeaponEffectType.IncreasedLightProperties,
                                (byte) ShellWeaponEffectType.IncreasedDarkProperties,
                                (byte) ShellWeaponEffectType.SLDamage,
                                (byte) ShellWeaponEffectType.SLDefence,
                                (byte) ShellWeaponEffectType.SLElement,
                                (byte) ShellWeaponEffectType.SLHP,
                                (byte) ShellWeaponEffectType.HPRecoveryForKilling,
                                (byte) ShellWeaponEffectType.MPRecoveryForKilling
                            };
                                short[] maximum = { 160, 16, 16, 1, 125, 125, 125, 125, 15, 15, 15, 15, 175, 175 };

                                if (!IsWeapon)
                                {
                                    effects = new[]
                                    {
                                    (byte) ShellArmorEffectType.CloseDefence,
                                    (byte) ShellArmorEffectType.DistanceDefence,
                                    (byte) ShellArmorEffectType.MagicDefence,
                                    (byte) ShellArmorEffectType.ReducedFreeze,
                                    (byte) ShellArmorEffectType.ReducedParalysis,
                                    (byte) ShellArmorEffectType.ReducedAllStun,
                                    (byte) ShellArmorEffectType.ReducedAllBleedingType,
                                    (byte) ShellArmorEffectType.RecoveryHP,
                                    (byte) ShellArmorEffectType.RecoveryMP,
                                    (byte) ShellArmorEffectType.IncreasedFireResistence,
                                    (byte) ShellArmorEffectType.IncreasedWaterResistence,
                                    (byte) ShellArmorEffectType.IncreasedLightResistence,
                                    (byte) ShellArmorEffectType.IncreasedDarkResistence
                                };
                                    maximum = new short[] { 160, 160, 160, 43, 35, 40, 40, 80, 80, 16, 16, 16, 16 };
                                }

                                var position = ServerManager.RandomNumber(0, effects.Length);
                                var effect = effects[position];
                                var value = CalculateEffect(maximum[position]);

                                if (effectsList.Any(s => s.Effect == effect))
                                {
                                    continue;
                                }

                                effectsList.Add(new ShellEffectDTO
                                {
                                    EffectLevel = ShellEffectLevelType.ANormal,
                                    Effect = effect,
                                    Value = value,
                                    EquipmentSerialId = EquipmentSerialId
                                });
                                return;
                            }
                        case ShellEffectLevelType.SNormal:
                            {
                                byte[] effects =
                                {
                                (byte) ShellWeaponEffectType.PercentageTotalDamage,
                                (byte) ShellWeaponEffectType.DamageincreasedtotheBigMonster,
                                (byte) ShellWeaponEffectType.IncreasedElementalProperties,
                                (byte) ShellWeaponEffectType.SLGlobal
                            };
                                short[] maximum = { 20, 25, 140, 9 };

                                if (!IsWeapon)
                                {
                                    effects = new[]
                                    {
                                    (byte) ShellArmorEffectType.PercentageTotalDefence,
                                    (byte) ShellArmorEffectType.ReducedAllNegativeEffect,
                                    (byte) ShellArmorEffectType.IncreasedAllResistence,
                                    (byte) ShellArmorEffectType.RecoveryHPInDefence
                                };
                                    maximum = new short[] { 20, 33, 22, 56 };
                                }

                                var position = ServerManager.RandomNumber(0, effects.Length);
                                var effect = effects[position];
                                var value = CalculateEffect(maximum[position]);

                                if (effectsList.Any(s => s.Effect == effect))
                                {
                                    continue;
                                }

                                effectsList.Add(new ShellEffectDTO
                                {
                                    EffectLevel = ShellEffectLevelType.SNormal,
                                    Effect = effect,
                                    Value = value,
                                    EquipmentSerialId = EquipmentSerialId
                                });
                                return;
                            }
                        case ShellEffectLevelType.CBonus:
                            {
                                byte[] effects =
                                {
                                (byte) ShellWeaponEffectType.GainMoreGold,
                                (byte) ShellWeaponEffectType.GainMoreXP,
                                (byte) ShellWeaponEffectType.GainMoreCXP
                            };
                                short[] maximum = { 7, 4, 4 };

                                if (!IsWeapon)
                                {
                                    effects = new[]
                                    {
                                    (byte) ShellArmorEffectType.ReducedPrideLoss,
                                    (byte) ShellArmorEffectType.ReducedProductionPointConsumed
                                };
                                    maximum = new short[] { 45, 43 };
                                }

                                var position = ServerManager.RandomNumber(0, effects.Length);
                                var effect = effects[position];
                                var value = CalculateEffect(maximum[position]);

                                if (effectsList.Any(s => s.Effect == effect))
                                {
                                    continue;
                                }

                                effectsList.Add(new ShellEffectDTO
                                {
                                    EffectLevel = ShellEffectLevelType.CBonus,
                                    Effect = effect,
                                    Value = value,
                                    EquipmentSerialId = EquipmentSerialId
                                });
                                return;
                            }
                        case ShellEffectLevelType.BBonus:
                            {
                                byte[] effects =
                                {
                                (byte) ShellWeaponEffectType.GainMoreGold,
                                (byte) ShellWeaponEffectType.GainMoreXP,
                                (byte) ShellWeaponEffectType.GainMoreCXP
                            };
                                short[] maximum = { 13, 6, 6 };

                                if (!IsWeapon)
                                {
                                    effects = new[]
                                    {
                                    (byte) ShellArmorEffectType.ReducedProductionPointConsumed,
                                    (byte) ShellArmorEffectType.IncreasedProductionPossibility,
                                    (byte) ShellArmorEffectType.IncreasedRecoveryItemSpeed
                                };
                                    maximum = new short[] { 56, 47, 21 };
                                }

                                var position = ServerManager.RandomNumber(0, effects.Length);
                                var effect = effects[position];
                                var value = CalculateEffect(maximum[position]);

                                if (effectsList.Any(s => s.Effect == effect))
                                {
                                    continue;
                                }

                                effectsList.Add(new ShellEffectDTO
                                {
                                    EffectLevel = ShellEffectLevelType.BBonus,
                                    Effect = effect,
                                    Value = value,
                                    EquipmentSerialId = EquipmentSerialId
                                });
                                return;
                            }
                        case ShellEffectLevelType.ABonus:
                            {
                                byte[] effects =
                                {
                                (byte) ShellWeaponEffectType.GainMoreGold,
                                (byte) ShellWeaponEffectType.GainMoreXP,
                                (byte) ShellWeaponEffectType.GainMoreCXP
                            };
                                short[] maximum = { 28, 12, 12 };

                                if (!IsWeapon)
                                {
                                    effects = new[]
                                    {
                                    (byte) ShellArmorEffectType.ReducedProductionPointConsumed,
                                    (byte) ShellArmorEffectType.IncreasedProductionPossibility,
                                    (byte) ShellArmorEffectType.IncreasedRecoveryItemSpeed
                                };
                                    maximum = new short[] { 60, 60, 46 };
                                }

                                var position = ServerManager.RandomNumber(0, effects.Length);
                                var effect = effects[position];
                                var value = CalculateEffect(maximum[position]);

                                if (effectsList.Any(s => s.Effect == effect))
                                {
                                    continue;
                                }

                                effectsList.Add(new ShellEffectDTO
                                {
                                    EffectLevel = ShellEffectLevelType.ABonus,
                                    Effect = effect,
                                    Value = value,
                                    EquipmentSerialId = EquipmentSerialId
                                });
                                return;
                            }
                        case ShellEffectLevelType.SBonus:
                            {
                                byte[] effects =
                                {
                                (byte) ShellWeaponEffectType.GainMoreGold,
                                (byte) ShellWeaponEffectType.GainMoreXP,
                                (byte) ShellWeaponEffectType.GainMoreCXP
                            };
                                short[] maximum = { 40, 18, 18 };

                                if (!IsWeapon)
                                {
                                    effects = new[]
                                    {
                                    (byte) ShellArmorEffectType.ReducedProductionPointConsumed,
                                    (byte) ShellArmorEffectType.IncreasedProductionPossibility,
                                    (byte) ShellArmorEffectType.IncreasedRecoveryItemSpeed
                                };
                                    maximum = new short[] { 60, 75, 55 };
                                }

                                var position = ServerManager.RandomNumber(0, effects.Length);
                                var effect = effects[position];
                                var value = CalculateEffect(maximum[position]);

                                if (effectsList.Any(s => s.Effect == effect))
                                {
                                    continue;
                                }

                                effectsList.Add(new ShellEffectDTO
                                {
                                    EffectLevel = ShellEffectLevelType.SBonus,
                                    Effect = effect,
                                    Value = value,
                                    EquipmentSerialId = EquipmentSerialId
                                });
                                return;
                            }
                        case ShellEffectLevelType.CPVP:
                            {
                                byte[] effects =
                                {
                                (byte) ShellWeaponEffectType.PercentageDamageInPVP,
                                (byte) ShellWeaponEffectType.ReducesPercentageEnemyDefenceInPVP,
                                (byte) ShellWeaponEffectType.PVPDamageAt15Percent,
                                (byte) ShellWeaponEffectType.ReducesEnemyMPInPVP
                            };
                                short[] maximum = { 8, 8, 54, 12 };

                                if (!IsWeapon)
                                {
                                    effects = new[]
                                    {
                                    (byte) ShellArmorEffectType.PercentageAllPVPDefence,
                                    (byte) ShellArmorEffectType.CloseDefenceDodgeInPVP,
                                    (byte) ShellArmorEffectType.DistanceDefenceDodgeInPVP,
                                    (byte) ShellArmorEffectType.IgnoreMagicDamage
                                };
                                    maximum = new short[] { 9, 4, 4, 4 };
                                }

                                var position = ServerManager.RandomNumber(0, effects.Length);
                                var effect = effects[position];
                                var value = CalculateEffect(maximum[position]);

                                if (effectsList.Any(s => s.Effect == effect))
                                {
                                    continue;
                                }

                                effectsList.Add(new ShellEffectDTO
                                {
                                    EffectLevel = ShellEffectLevelType.CPVP,
                                    Effect = effect,
                                    Value = value,
                                    EquipmentSerialId = EquipmentSerialId
                                });
                                return;
                            }
                        case ShellEffectLevelType.BPVP:
                            {
                                byte[] effects =
                                {
                                (byte) ShellWeaponEffectType.PercentageDamageInPVP,
                                (byte) ShellWeaponEffectType.ReducesPercentageEnemyDefenceInPVP,
                                (byte) ShellWeaponEffectType.ReducesEnemyMPInPVP,
                                (byte) ShellWeaponEffectType.ReducesEnemyFireResistanceInPVP,
                                (byte) ShellWeaponEffectType.ReducesEnemyWaterResistanceInPVP,
                                (byte) ShellWeaponEffectType.ReducesEnemyLightResistanceInPVP,
                                (byte) ShellWeaponEffectType.ReducesEnemyDarkResistanceInPVP
                            };
                                short[] maximum = { 12, 12, 20, 6, 6, 6, 6 };

                                if (!IsWeapon)
                                {
                                    effects = new[]
                                    {
                                    (byte) ShellArmorEffectType.PercentageAllPVPDefence,
                                    (byte) ShellArmorEffectType.CloseDefenceDodgeInPVP,
                                    (byte) ShellArmorEffectType.DistanceDefenceDodgeInPVP,
                                    (byte) ShellArmorEffectType.IgnoreMagicDamage
                                };
                                    maximum = new short[] { 11, 6, 6, 6 };
                                }

                                var position = ServerManager.RandomNumber(0, effects.Length);
                                var effect = effects[position];
                                var value = CalculateEffect(maximum[position]);

                                if (effectsList.Any(s => s.Effect == effect))
                                {
                                    continue;
                                }

                                effectsList.Add(new ShellEffectDTO
                                {
                                    EffectLevel = ShellEffectLevelType.BPVP,
                                    Effect = effect,
                                    Value = value,
                                    EquipmentSerialId = EquipmentSerialId
                                });
                                return;
                            }
                        case ShellEffectLevelType.APVP:
                            {
                                byte[] effects =
                                {
                                (byte) ShellWeaponEffectType.PercentageDamageInPVP,
                                (byte) ShellWeaponEffectType.ReducesPercentageEnemyDefenceInPVP,
                                (byte) ShellWeaponEffectType.ReducesEnemyMPInPVP,
                                (byte) ShellWeaponEffectType.ReducesEnemyFireResistanceInPVP,
                                (byte) ShellWeaponEffectType.ReducesEnemyWaterResistanceInPVP,
                                (byte) ShellWeaponEffectType.ReducesEnemyLightResistanceInPVP,
                                (byte) ShellWeaponEffectType.ReducesEnemyDarkResistanceInPVP
                            };
                                short[] maximum = { 17, 17, 42, 15, 15, 15, 15 };

                                if (!IsWeapon)
                                {
                                    effects = new[]
                                    {
                                    (byte) ShellArmorEffectType.PercentageAllPVPDefence,
                                    (byte) ShellArmorEffectType.CloseDefenceDodgeInPVP,
                                    (byte) ShellArmorEffectType.DistanceDefenceDodgeInPVP,
                                    (byte) ShellArmorEffectType.IgnoreMagicDamage,
                                    (byte) ShellArmorEffectType.ProtectMPInPVP
                                };
                                    maximum = new short[] { 20, 12, 12, 12, 0 };
                                }

                                var position = ServerManager.RandomNumber(0, effects.Length);
                                var effect = effects[position];
                                var value = CalculateEffect(maximum[position]);

                                if (effectsList.Any(s => s.Effect == effect))
                                {
                                    continue;
                                }

                                effectsList.Add(new ShellEffectDTO
                                {
                                    EffectLevel = ShellEffectLevelType.APVP,
                                    Effect = effect,
                                    Value = value,
                                    EquipmentSerialId = EquipmentSerialId
                                });
                                return;
                            }
                        case ShellEffectLevelType.SPVP:
                            {
                                byte[] effects =
                                {
                                (byte) ShellWeaponEffectType.PercentageDamageInPVP,
                                (byte) ShellWeaponEffectType.ReducesPercentageEnemyDefenceInPVP,
                                (byte) ShellWeaponEffectType.ReducesEnemyAllResistancesInPVP
                            };
                                short[] maximum = { 35, 35, 17 };

                                if (!IsWeapon)
                                {
                                    effects = new[]
                                    {
                                    (byte) ShellArmorEffectType.PercentageAllPVPDefence,
                                    (byte) ShellArmorEffectType.DodgeAllAttacksInPVP
                                };
                                    maximum = new short[] { 32, 16 };
                                }

                                var position = ServerManager.RandomNumber(0, effects.Length);
                                var effect = effects[position];
                                var value = CalculateEffect(maximum[position]);

                                if (effectsList.Any(s => s.Effect == effect))
                                {
                                    continue;
                                }

                                effectsList.Add(new ShellEffectDTO
                                {
                                    EffectLevel = ShellEffectLevelType.SPVP,
                                    Effect = effect,
                                    Value = value,
                                    EquipmentSerialId = EquipmentSerialId
                                });
                                return;
                            }
                    }
                }
            }

            for (var i = 0; i < CNormCount; i++)
            {
                AddEffect(ShellEffectLevelType.CNormal);
            }

            for (var i = 0; i < BNormCount; i++)
            {
                AddEffect(ShellEffectLevelType.BNormal);
            }

            for (var i = 0; i < ANormCount; i++)
            {
                AddEffect(ShellEffectLevelType.ANormal);
            }

            for (var i = 0; i < SNormCount; i++)
            {
                AddEffect(ShellEffectLevelType.SNormal);
            }

            for (var i = 0; i < CBonusMax; i++)
            {
                AddEffect(ShellEffectLevelType.CBonus);
            }

            for (var i = 0; i < BBonusMax; i++)
            {
                AddEffect(ShellEffectLevelType.BBonus);
            }

            for (var i = 0; i < ABonusMax; i++)
            {
                AddEffect(ShellEffectLevelType.ABonus);
            }

            for (var i = 0; i < SBonusMax; i++)
            {
                AddEffect(ShellEffectLevelType.SBonus);
            }

            for (var i = 0; i < SPVPMax; i++)
            {
                AddEffect(ShellEffectLevelType.SPVP);
            }

            for (var i = 0; i < APVPMax; i++)
            {
                AddEffect(ShellEffectLevelType.APVP);
            }

            for (var i = 0; i < BPVPMax; i++)
            {
                AddEffect(ShellEffectLevelType.BPVP);
            }

            for (var i = 0; i < CPVPMax; i++)
            {
                AddEffect(ShellEffectLevelType.CPVP);
            }

            ShellEffects.Clear();
            ShellEffects.AddRange(effectsList);

            DAOFactory.ShellEffectDAO.InsertOrUpdateFromList(ShellEffects, EquipmentSerialId);
        }

        #endregion
    }
}