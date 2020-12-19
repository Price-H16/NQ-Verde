using System.Data.Entity.Migrations;

namespace OpenNos.DAL.EF.Migrations
{
    public partial class OpenNos : DbMigration
    {
        #region Methods

        public override void Down()
        {
            DropForeignKey("dbo.CharacterQuest", "QuestId", "dbo.Quest");
            DropForeignKey("dbo.PenaltyLog", "AccountId", "dbo.Account");
            DropForeignKey("dbo.Character", "AccountId", "dbo.Account");
            DropForeignKey("dbo.StaticBuff", "CharacterId", "dbo.Character");
            DropForeignKey("dbo.StaticBonus", "CharacterId", "dbo.Character");
            DropForeignKey("dbo.Respawn", "CharacterId", "dbo.Character");
            DropForeignKey("dbo.QuicklistEntry", "CharacterId", "dbo.Character");
            DropForeignKey("dbo.MinilandObject", "CharacterId", "dbo.Character");
            DropForeignKey("dbo.MinigameLog", "CharacterId", "dbo.Character");
            DropForeignKey("dbo.Mate", "CharacterId", "dbo.Character");
            DropForeignKey("dbo.Mail", "ReceiverId", "dbo.Character");
            DropForeignKey("dbo.Mail", "SenderId", "dbo.Character");
            DropForeignKey("dbo.ItemInstance", "CharacterId", "dbo.Character");
            DropForeignKey("dbo.GeneralLog", "CharacterId", "dbo.Character");
            DropForeignKey("dbo.GeneralLog", "AccountId", "dbo.Account");
            DropForeignKey("dbo.FamilyCharacter", "FamilyId", "dbo.Family");
            DropForeignKey("dbo.FamilyLog", "FamilyId", "dbo.Family");
            DropForeignKey("dbo.FamilyCharacter", "CharacterId", "dbo.Character");
            DropForeignKey("dbo.CharacterSkill", "CharacterId", "dbo.Character");
            DropForeignKey("dbo.CharacterRelation", "RelatedCharacterId", "dbo.Character");
            DropForeignKey("dbo.CharacterRelation", "CharacterId", "dbo.Character");
            DropForeignKey("dbo.BazaarItem", "ItemInstanceId", "dbo.ItemInstance");
            DropForeignKey("dbo.MinilandObject", "ItemInstanceId", "dbo.ItemInstance");
            DropForeignKey("dbo.ShopItem", "ItemVNum", "dbo.Item");
            DropForeignKey("dbo.RollGeneratedItem", "OriginalItemVNum", "dbo.Item");
            DropForeignKey("dbo.RollGeneratedItem", "ItemGeneratedVNum", "dbo.Item");
            DropForeignKey("dbo.RecipeItem", "ItemVNum", "dbo.Item");
            DropForeignKey("dbo.Recipe", "ItemVNum", "dbo.Item");
            DropForeignKey("dbo.Mail", "AttachmentVNum", "dbo.Item");
            DropForeignKey("dbo.ItemInstance", "ItemVNum", "dbo.Item");
            DropForeignKey("dbo.Drop", "ItemVNum", "dbo.Item");
            DropForeignKey("dbo.BCard", "SkillVNum", "dbo.Skill");
            DropForeignKey("dbo.BCard", "NpcMonsterVNum", "dbo.NpcMonster");
            DropForeignKey("dbo.NpcMonsterSkill", "NpcMonsterVNum", "dbo.NpcMonster");
            DropForeignKey("dbo.Mate", "NpcMonsterVNum", "dbo.NpcMonster");
            DropForeignKey("dbo.MapNpc", "NpcVNum", "dbo.NpcMonster");
            DropForeignKey("dbo.MapMonster", "MonsterVNum", "dbo.NpcMonster");
            DropForeignKey("dbo.Drop", "MonsterVNum", "dbo.NpcMonster");
            DropForeignKey("dbo.MapType", "ReturnMapTypeId", "dbo.RespawnMapType");
            DropForeignKey("dbo.MapType", "RespawnMapTypeId", "dbo.RespawnMapType");
            DropForeignKey("dbo.MapTypeMap", "MapTypeId", "dbo.MapType");
            DropForeignKey("dbo.MapTypeMap", "MapId", "dbo.Map");
            DropForeignKey("dbo.Teleporter", "MapId", "dbo.Map");
            DropForeignKey("dbo.ScriptedInstance", "MapId", "dbo.Map");
            DropForeignKey("dbo.Respawn", "RespawnMapTypeId", "dbo.RespawnMapType");
            DropForeignKey("dbo.RespawnMapType", "DefaultMapId", "dbo.Map");
            DropForeignKey("dbo.Respawn", "MapId", "dbo.Map");
            DropForeignKey("dbo.Portal", "SourceMapId", "dbo.Map");
            DropForeignKey("dbo.Portal", "DestinationMapId", "dbo.Map");
            DropForeignKey("dbo.MapNpc", "MapId", "dbo.Map");
            DropForeignKey("dbo.Teleporter", "MapNpcId", "dbo.MapNpc");
            DropForeignKey("dbo.Shop", "MapNpcId", "dbo.MapNpc");
            DropForeignKey("dbo.ShopSkill", "ShopId", "dbo.Shop");
            DropForeignKey("dbo.ShopSkill", "SkillVNum", "dbo.Skill");
            DropForeignKey("dbo.NpcMonsterSkill", "SkillVNum", "dbo.Skill");
            DropForeignKey("dbo.Combo", "SkillVNum", "dbo.Skill");
            DropForeignKey("dbo.CharacterSkill", "SkillVNum", "dbo.Skill");
            DropForeignKey("dbo.ShopItem", "ShopId", "dbo.Shop");
            DropForeignKey("dbo.RecipeList", "RecipeId", "dbo.Recipe");
            DropForeignKey("dbo.RecipeItem", "RecipeId", "dbo.Recipe");
            DropForeignKey("dbo.RecipeList", "MapNpcId", "dbo.MapNpc");
            DropForeignKey("dbo.RecipeList", "ItemVNum", "dbo.Item");
            DropForeignKey("dbo.MapMonster", "MapId", "dbo.Map");
            DropForeignKey("dbo.Character", "MapId", "dbo.Map");
            DropForeignKey("dbo.Drop", "MapTypeId", "dbo.MapType");
            DropForeignKey("dbo.BCard", "ItemVNum", "dbo.Item");
            DropForeignKey("dbo.BCard", "CardId", "dbo.Card");
            DropForeignKey("dbo.StaticBuff", "CardId", "dbo.Card");
            DropForeignKey("dbo.ItemInstance", "BoundCharacterId", "dbo.Character");
            DropForeignKey("dbo.BazaarItem", "SellerId", "dbo.Character");
            DropIndex("dbo.CharacterQuest", new[] {"QuestId"});
            DropIndex("dbo.PenaltyLog", new[] {"AccountId"});
            DropIndex("dbo.StaticBonus", new[] {"CharacterId"});
            DropIndex("dbo.QuicklistEntry", new[] {"CharacterId"});
            DropIndex("dbo.MinigameLog", new[] {"CharacterId"});
            DropIndex("dbo.GeneralLog", new[] {"CharacterId"});
            DropIndex("dbo.GeneralLog", new[] {"AccountId"});
            DropIndex("dbo.FamilyLog", new[] {"FamilyId"});
            DropIndex("dbo.FamilyCharacter", new[] {"FamilyId"});
            DropIndex("dbo.FamilyCharacter", new[] {"CharacterId"});
            DropIndex("dbo.CharacterRelation", new[] {"RelatedCharacterId"});
            DropIndex("dbo.CharacterRelation", new[] {"CharacterId"});
            DropIndex("dbo.MinilandObject", new[] {"ItemInstanceId"});
            DropIndex("dbo.MinilandObject", new[] {"CharacterId"});
            DropIndex("dbo.RollGeneratedItem", new[] {"OriginalItemVNum"});
            DropIndex("dbo.RollGeneratedItem", new[] {"ItemGeneratedVNum"});
            DropIndex("dbo.Mail", new[] {"SenderId"});
            DropIndex("dbo.Mail", new[] {"ReceiverId"});
            DropIndex("dbo.Mail", new[] {"AttachmentVNum"});
            DropIndex("dbo.Mate", new[] {"NpcMonsterVNum"});
            DropIndex("dbo.Mate", new[] {"CharacterId"});
            DropIndex("dbo.ScriptedInstance", new[] {"MapId"});
            DropIndex("dbo.RespawnMapType", new[] {"DefaultMapId"});
            DropIndex("dbo.Respawn", new[] {"RespawnMapTypeId"});
            DropIndex("dbo.Respawn", new[] {"MapId"});
            DropIndex("dbo.Respawn", new[] {"CharacterId"});
            DropIndex("dbo.Portal", new[] {"SourceMapId"});
            DropIndex("dbo.Portal", new[] {"DestinationMapId"});
            DropIndex("dbo.Teleporter", new[] {"MapNpcId"});
            DropIndex("dbo.Teleporter", new[] {"MapId"});
            DropIndex("dbo.NpcMonsterSkill", new[] {"SkillVNum"});
            DropIndex("dbo.NpcMonsterSkill", new[] {"NpcMonsterVNum"});
            DropIndex("dbo.Combo", new[] {"SkillVNum"});
            DropIndex("dbo.CharacterSkill", new[] {"SkillVNum"});
            DropIndex("dbo.CharacterSkill", new[] {"CharacterId"});
            DropIndex("dbo.ShopSkill", new[] {"SkillVNum"});
            DropIndex("dbo.ShopSkill", new[] {"ShopId"});
            DropIndex("dbo.ShopItem", new[] {"ShopId"});
            DropIndex("dbo.ShopItem", new[] {"ItemVNum"});
            DropIndex("dbo.Shop", new[] {"MapNpcId"});
            DropIndex("dbo.RecipeItem", new[] {"RecipeId"});
            DropIndex("dbo.RecipeItem", new[] {"ItemVNum"});
            DropIndex("dbo.Recipe", new[] {"ItemVNum"});
            DropIndex("dbo.RecipeList", new[] {"RecipeId"});
            DropIndex("dbo.RecipeList", new[] {"MapNpcId"});
            DropIndex("dbo.RecipeList", new[] {"ItemVNum"});
            DropIndex("dbo.MapNpc", new[] {"NpcVNum"});
            DropIndex("dbo.MapNpc", new[] {"MapId"});
            DropIndex("dbo.MapMonster", new[] {"MonsterVNum"});
            DropIndex("dbo.MapMonster", new[] {"MapId"});
            DropIndex("dbo.MapTypeMap", new[] {"MapTypeId"});
            DropIndex("dbo.MapTypeMap", new[] {"MapId"});
            DropIndex("dbo.MapType", new[] {"ReturnMapTypeId"});
            DropIndex("dbo.MapType", new[] {"RespawnMapTypeId"});
            DropIndex("dbo.Drop", new[] {"MonsterVNum"});
            DropIndex("dbo.Drop", new[] {"MapTypeId"});
            DropIndex("dbo.Drop", new[] {"ItemVNum"});
            DropIndex("dbo.StaticBuff", new[] {"CharacterId"});
            DropIndex("dbo.StaticBuff", new[] {"CardId"});
            DropIndex("dbo.BCard", new[] {"SkillVNum"});
            DropIndex("dbo.BCard", new[] {"NpcMonsterVNum"});
            DropIndex("dbo.BCard", new[] {"ItemVNum"});
            DropIndex("dbo.BCard", new[] {"CardId"});
            DropIndex("dbo.ItemInstance", new[] {"ItemVNum"});
            DropIndex("dbo.ItemInstance", "IX_SlotAndType");
            DropIndex("dbo.ItemInstance", new[] {"BoundCharacterId"});
            DropIndex("dbo.BazaarItem", new[] {"SellerId"});
            DropIndex("dbo.BazaarItem", new[] {"ItemInstanceId"});
            DropIndex("dbo.Character", new[] {"MapId"});
            DropIndex("dbo.Character", new[] {"AccountId"});
            DropTable("dbo.ShellEffect");
            DropTable("dbo.QuestReward");
            DropTable("dbo.QuestObjective");
            DropTable("dbo.QuestLog");
            DropTable("dbo.PartnerSkill");
            DropTable("dbo.MaintenanceLog");
            DropTable("dbo.Quest");
            DropTable("dbo.CharacterQuest");
            DropTable("dbo.CellonOption");
            DropTable("dbo.PenaltyLog");
            DropTable("dbo.StaticBonus");
            DropTable("dbo.QuicklistEntry");
            DropTable("dbo.MinigameLog");
            DropTable("dbo.GeneralLog");
            DropTable("dbo.FamilyLog");
            DropTable("dbo.Family");
            DropTable("dbo.FamilyCharacter");
            DropTable("dbo.CharacterRelation");
            DropTable("dbo.MinilandObject");
            DropTable("dbo.RollGeneratedItem");
            DropTable("dbo.Mail");
            DropTable("dbo.Mate");
            DropTable("dbo.ScriptedInstance");
            DropTable("dbo.RespawnMapType");
            DropTable("dbo.Respawn");
            DropTable("dbo.Portal");
            DropTable("dbo.Teleporter");
            DropTable("dbo.NpcMonsterSkill");
            DropTable("dbo.Combo");
            DropTable("dbo.CharacterSkill");
            DropTable("dbo.Skill");
            DropTable("dbo.ShopSkill");
            DropTable("dbo.ShopItem");
            DropTable("dbo.Shop");
            DropTable("dbo.RecipeItem");
            DropTable("dbo.Recipe");
            DropTable("dbo.RecipeList");
            DropTable("dbo.MapNpc");
            DropTable("dbo.MapMonster");
            DropTable("dbo.Map");
            DropTable("dbo.MapTypeMap");
            DropTable("dbo.MapType");
            DropTable("dbo.Drop");
            DropTable("dbo.NpcMonster");
            DropTable("dbo.StaticBuff");
            DropTable("dbo.Card");
            DropTable("dbo.BCard");
            DropTable("dbo.Item");
            DropTable("dbo.ItemInstance");
            DropTable("dbo.BazaarItem");
            DropTable("dbo.Character");
            DropTable("dbo.Account");
        }

        public override void Up()
        {
            CreateTable(
                    "dbo.Account",
                    c => new
                    {
                        AccountId = c.Long(false, true),
                        Authority = c.Short(false),
                        Email = c.String(maxLength: 255),
                        Name = c.String(maxLength: 255),
                        Password = c.String(maxLength: 255, unicode: false),
                        ReferrerId = c.Long(false),
                        RegistrationIP = c.String(maxLength: 45),
                        VerificationToken = c.String(maxLength: 32),
                        DailyRewardSent = c.Boolean(false)
                    })
                .PrimaryKey(t => t.AccountId);

            CreateTable(
                    "dbo.Character",
                    c => new
                    {
                        CharacterId = c.Long(false, true),
                        AccountId = c.Long(false),
                        Act4Dead = c.Int(false),
                        Act4Kill = c.Int(false),
                        Act4Points = c.Int(false),
                        ArenaWinner = c.Int(false),
                        Biography = c.String(maxLength: 255),
                        BuffBlocked = c.Boolean(false),
                        Class = c.Byte(false),
                        Compliment = c.Short(false),
                        Dignity = c.Single(false),
                        EmoticonsBlocked = c.Boolean(false),
                        ExchangeBlocked = c.Boolean(false),
                        Faction = c.Byte(false),
                        FamilyRequestBlocked = c.Boolean(false),
                        FriendRequestBlocked = c.Boolean(false),
                        Gender = c.Byte(false),
                        Gold = c.Long(false),
                        GoldBank = c.Long(false),
                        GroupRequestBlocked = c.Boolean(false),
                        HairColor = c.Byte(false),
                        HairStyle = c.Byte(false),
                        HeroChatBlocked = c.Boolean(false),
                        HeroLevel = c.Byte(false),
                        HeroXp = c.Long(false),
                        Hp = c.Int(false),
                        HpBlocked = c.Boolean(false),
                        IsPetAutoRelive = c.Boolean(false),
                        IsPartnerAutoRelive = c.Boolean(false),
                        IsSeal = c.Boolean(false),
                        JobLevel = c.Byte(false),
                        JobLevelXp = c.Long(false),
                        LastFamilyLeave = c.Long(false),
                        Level = c.Byte(false),
                        LevelXp = c.Long(false),
                        MapId = c.Short(false),
                        MapX = c.Short(false),
                        MapY = c.Short(false),
                        MasterPoints = c.Int(false),
                        MasterTicket = c.Int(false),
                        MaxMateCount = c.Byte(false),
                        MinilandInviteBlocked = c.Boolean(false),
                        MinilandMessage = c.String(maxLength: 255),
                        MinilandPoint = c.Short(false),
                        MinilandState = c.Byte(false),
                        MouseAimLock = c.Boolean(false),
                        Mp = c.Int(false),
                        Name = c.String(maxLength: 255, unicode: false),
                        QuickGetUp = c.Boolean(false),
                        RagePoint = c.Long(false),
                        Reputation = c.Long(false),
                        Slot = c.Byte(false),
                        SpAdditionPoint = c.Int(false),
                        SpPoint = c.Int(false),
                        State = c.Byte(false),
                        TalentLose = c.Int(false),
                        TalentSurrender = c.Int(false),
                        TalentWin = c.Int(false),
                        WhisperBlocked = c.Boolean(false)
                    })
                .PrimaryKey(t => t.CharacterId)
                .ForeignKey("dbo.Map", t => t.MapId)
                .ForeignKey("dbo.Account", t => t.AccountId)
                .Index(t => t.AccountId)
                .Index(t => t.MapId);

            CreateTable(
                    "dbo.BazaarItem",
                    c => new
                    {
                        BazaarItemId = c.Long(false, true),
                        Amount = c.Short(false),
                        DateStart = c.DateTime(false),
                        Duration = c.Short(false),
                        IsPackage = c.Boolean(false),
                        ItemInstanceId = c.Guid(false),
                        MedalUsed = c.Boolean(false),
                        Price = c.Long(false),
                        SellerId = c.Long(false)
                    })
                .PrimaryKey(t => t.BazaarItemId)
                .ForeignKey("dbo.Character", t => t.SellerId)
                .ForeignKey("dbo.ItemInstance", t => t.ItemInstanceId)
                .Index(t => t.ItemInstanceId)
                .Index(t => t.SellerId);

            CreateTable(
                    "dbo.ItemInstance",
                    c => new
                    {
                        Id = c.Guid(false),
                        Amount = c.Int(false),
                        BazaarItemId = c.Long(),
                        BoundCharacterId = c.Long(),
                        CharacterId = c.Long(false),
                        Design = c.Short(false),
                        DurabilityPoint = c.Int(false),
                        ItemDeleteTime = c.DateTime(),
                        ItemVNum = c.Short(false),
                        Rare = c.Short(false),
                        Slot = c.Short(false),
                        Type = c.Byte(false),
                        Upgrade = c.Byte(false),
                        HoldingVNum = c.Short(),
                        ShellRarity = c.Short(),
                        SlDamage = c.Short(),
                        SlDefence = c.Short(),
                        SlElement = c.Short(),
                        SlHP = c.Short(),
                        SpDamage = c.Byte(),
                        SpDark = c.Byte(),
                        SpDefence = c.Byte(),
                        SpElement = c.Byte(),
                        SpFire = c.Byte(),
                        SpHP = c.Byte(),
                        SpLevel = c.Byte(),
                        SpLight = c.Byte(),
                        SpStoneUpgrade = c.Byte(),
                        SpWater = c.Byte(),
                        Ammo = c.Byte(),
                        Cellon = c.Byte(),
                        CloseDefence = c.Short(),
                        Concentrate = c.Short(),
                        CriticalDodge = c.Short(),
                        CriticalLuckRate = c.Byte(),
                        CriticalRate = c.Short(),
                        DamageMaximum = c.Short(),
                        DamageMinimum = c.Short(),
                        DarkElement = c.Byte(),
                        DarkResistance = c.Short(),
                        DefenceDodge = c.Short(),
                        DistanceDefence = c.Short(),
                        DistanceDefenceDodge = c.Short(),
                        ElementRate = c.Short(),
                        EquipmentSerialId = c.Guid(),
                        FireElement = c.Byte(),
                        FireResistance = c.Short(),
                        HitRate = c.Short(),
                        HP = c.Short(),
                        IsEmpty = c.Boolean(),
                        IsFixed = c.Boolean(),
                        IsPartnerEquipment = c.Boolean(),
                        LightElement = c.Byte(),
                        LightResistance = c.Short(),
                        MagicDefence = c.Short(),
                        MaxElementRate = c.Short(),
                        MP = c.Short(),
                        WaterElement = c.Byte(),
                        WaterResistance = c.Short(),
                        XP = c.Long()
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Character", t => t.BoundCharacterId)
                .ForeignKey("dbo.Item", t => t.ItemVNum)
                .ForeignKey("dbo.Character", t => t.CharacterId)
                .Index(t => t.BoundCharacterId)
                .Index(t => new {t.CharacterId, t.Slot, t.Type}, "IX_SlotAndType")
                .Index(t => t.ItemVNum);

            CreateTable(
                    "dbo.Item",
                    c => new
                    {
                        VNum = c.Short(false),
                        BasicUpgrade = c.Byte(false),
                        CellonLvl = c.Byte(false),
                        Class = c.Byte(false),
                        CloseDefence = c.Short(false),
                        Color = c.Byte(false),
                        Concentrate = c.Short(false),
                        CriticalLuckRate = c.Byte(false),
                        CriticalRate = c.Short(false),
                        DamageMaximum = c.Short(false),
                        DamageMinimum = c.Short(false),
                        DarkElement = c.Byte(false),
                        DarkResistance = c.Short(false),
                        DefenceDodge = c.Short(false),
                        DistanceDefence = c.Short(false),
                        DistanceDefenceDodge = c.Short(false),
                        Effect = c.Short(false),
                        EffectValue = c.Int(false),
                        Element = c.Byte(false),
                        ElementRate = c.Short(false),
                        EquipmentSlot = c.Byte(false),
                        FireElement = c.Byte(false),
                        FireResistance = c.Short(false),
                        Height = c.Byte(false),
                        HitRate = c.Short(false),
                        Hp = c.Short(false),
                        HpRegeneration = c.Short(false),
                        IsBlocked = c.Boolean(false),
                        IsColored = c.Boolean(false),
                        IsConsumable = c.Boolean(false),
                        IsDroppable = c.Boolean(false),
                        IsHeroic = c.Boolean(false),
                        IsHolder = c.Boolean(false),
                        IsMinilandObject = c.Boolean(false),
                        IsSoldable = c.Boolean(false),
                        IsTradable = c.Boolean(false),
                        ItemSubType = c.Byte(false),
                        ItemType = c.Byte(false),
                        ItemValidTime = c.Long(false),
                        LevelJobMinimum = c.Byte(false),
                        LevelMinimum = c.Byte(false),
                        LightElement = c.Byte(false),
                        LightResistance = c.Short(false),
                        MagicDefence = c.Short(false),
                        MaxCellon = c.Byte(false),
                        MaxCellonLvl = c.Byte(false),
                        MaxElementRate = c.Short(false),
                        MaximumAmmo = c.Byte(false),
                        MinilandObjectPoint = c.Int(false),
                        MoreHp = c.Short(false),
                        MoreMp = c.Short(false),
                        Morph = c.Short(false),
                        Mp = c.Short(false),
                        MpRegeneration = c.Short(false),
                        Name = c.String(maxLength: 255),
                        Price = c.Long(false),
                        SellToNpcPrice = c.Long(false),
                        PvpDefence = c.Short(false),
                        PvpStrength = c.Byte(false),
                        ReduceOposantResistance = c.Short(false),
                        ReputationMinimum = c.Byte(false),
                        ReputPrice = c.Long(false),
                        SecondaryElement = c.Byte(false),
                        Sex = c.Byte(false),
                        Speed = c.Byte(false),
                        SpType = c.Byte(false),
                        Type = c.Byte(false),
                        WaitDelay = c.Short(false),
                        WaterElement = c.Byte(false),
                        WaterResistance = c.Short(false),
                        Width = c.Byte(false)
                    })
                .PrimaryKey(t => t.VNum);

            CreateTable(
                    "dbo.BCard",
                    c => new
                    {
                        BCardId = c.Int(false, true),
                        CardId = c.Short(),
                        CastType = c.Byte(false),
                        FirstData = c.Int(false),
                        IsLevelDivided = c.Boolean(false),
                        IsLevelScaled = c.Boolean(false),
                        ItemVNum = c.Short(),
                        NpcMonsterVNum = c.Short(),
                        SecondData = c.Int(false),
                        SkillVNum = c.Short(),
                        SubType = c.Byte(false),
                        ThirdData = c.Int(false),
                        Type = c.Byte(false)
                    })
                .PrimaryKey(t => t.BCardId)
                .ForeignKey("dbo.Card", t => t.CardId)
                .ForeignKey("dbo.Item", t => t.ItemVNum)
                .ForeignKey("dbo.NpcMonster", t => t.NpcMonsterVNum)
                .ForeignKey("dbo.Skill", t => t.SkillVNum)
                .Index(t => t.CardId)
                .Index(t => t.ItemVNum)
                .Index(t => t.NpcMonsterVNum)
                .Index(t => t.SkillVNum);

            CreateTable(
                    "dbo.Card",
                    c => new
                    {
                        CardId = c.Short(false),
                        BuffType = c.Byte(false),
                        Delay = c.Int(false),
                        Duration = c.Int(false),
                        EffectId = c.Int(false),
                        Level = c.Byte(false),
                        Name = c.String(maxLength: 255),
                        Propability = c.Byte(false),
                        TimeoutBuff = c.Short(false),
                        TimeoutBuffChance = c.Byte(false)
                    })
                .PrimaryKey(t => t.CardId);

            CreateTable(
                    "dbo.StaticBuff",
                    c => new
                    {
                        StaticBuffId = c.Long(false, true),
                        CardId = c.Short(false),
                        CharacterId = c.Long(false),
                        RemainingTime = c.Int(false)
                    })
                .PrimaryKey(t => t.StaticBuffId)
                .ForeignKey("dbo.Card", t => t.CardId)
                .ForeignKey("dbo.Character", t => t.CharacterId)
                .Index(t => t.CardId)
                .Index(t => t.CharacterId);

            CreateTable(
                    "dbo.NpcMonster",
                    c => new
                    {
                        NpcMonsterVNum = c.Short(false),
                        AmountRequired = c.Short(false),
                        AttackClass = c.Byte(false),
                        AttackUpgrade = c.Byte(false),
                        BasicArea = c.Byte(false),
                        BasicCooldown = c.Short(false),
                        BasicRange = c.Byte(false),
                        BasicSkill = c.Short(false),
                        Catch = c.Boolean(false),
                        CloseDefence = c.Short(false),
                        Concentrate = c.Short(false),
                        CriticalChance = c.Byte(false),
                        CriticalRate = c.Short(false),
                        DamageMaximum = c.Short(false),
                        DamageMinimum = c.Short(false),
                        DarkResistance = c.Short(false),
                        DefenceDodge = c.Short(false),
                        DefenceUpgrade = c.Byte(false),
                        DistanceDefence = c.Short(false),
                        DistanceDefenceDodge = c.Short(false),
                        Element = c.Byte(false),
                        ElementRate = c.Short(false),
                        FireResistance = c.Short(false),
                        HeroLevel = c.Byte(false),
                        HeroXP = c.Int(false),
                        IsHostile = c.Boolean(false),
                        JobXP = c.Int(false),
                        Level = c.Byte(false),
                        LightResistance = c.Short(false),
                        MagicDefence = c.Short(false),
                        MaxHP = c.Int(false),
                        MaxMP = c.Int(false),
                        MonsterType = c.Byte(false),
                        Name = c.String(maxLength: 255),
                        NoAggresiveIcon = c.Boolean(false),
                        NoticeRange = c.Byte(false),
                        OriginalNpcMonsterVNum = c.Short(false),
                        Race = c.Byte(false),
                        RaceType = c.Byte(false),
                        RespawnTime = c.Int(false),
                        Speed = c.Byte(false),
                        VNumRequired = c.Short(false),
                        WaterResistance = c.Short(false),
                        XP = c.Int(false)
                    })
                .PrimaryKey(t => t.NpcMonsterVNum);

            CreateTable(
                    "dbo.Drop",
                    c => new
                    {
                        DropId = c.Short(false, true),
                        Amount = c.Int(false),
                        DropChance = c.Int(false),
                        ItemVNum = c.Short(false),
                        MapTypeId = c.Short(),
                        MonsterVNum = c.Short()
                    })
                .PrimaryKey(t => t.DropId)
                .ForeignKey("dbo.MapType", t => t.MapTypeId)
                .ForeignKey("dbo.NpcMonster", t => t.MonsterVNum)
                .ForeignKey("dbo.Item", t => t.ItemVNum)
                .Index(t => t.ItemVNum)
                .Index(t => t.MapTypeId)
                .Index(t => t.MonsterVNum);

            CreateTable(
                    "dbo.MapType",
                    c => new
                    {
                        MapTypeId = c.Short(false, true),
                        MapTypeName = c.String(),
                        PotionDelay = c.Short(false),
                        RespawnMapTypeId = c.Long(),
                        ReturnMapTypeId = c.Long()
                    })
                .PrimaryKey(t => t.MapTypeId)
                .ForeignKey("dbo.RespawnMapType", t => t.RespawnMapTypeId)
                .ForeignKey("dbo.RespawnMapType", t => t.ReturnMapTypeId)
                .Index(t => t.RespawnMapTypeId)
                .Index(t => t.ReturnMapTypeId);

            CreateTable(
                    "dbo.MapTypeMap",
                    c => new
                    {
                        MapId = c.Short(false),
                        MapTypeId = c.Short(false)
                    })
                .PrimaryKey(t => new {t.MapId, t.MapTypeId})
                .ForeignKey("dbo.Map", t => t.MapId)
                .ForeignKey("dbo.MapType", t => t.MapTypeId)
                .Index(t => t.MapId)
                .Index(t => t.MapTypeId);

            CreateTable(
                    "dbo.Map",
                    c => new
                    {
                        MapId = c.Short(false),
                        Data = c.Binary(),
                        GridMapId = c.Short(false),
                        Music = c.Int(false),
                        Name = c.String(maxLength: 255),
                        ShopAllowed = c.Boolean(false),
                        XpRate = c.Byte(false)
                    })
                .PrimaryKey(t => t.MapId);

            CreateTable(
                    "dbo.MapMonster",
                    c => new
                    {
                        MapMonsterId = c.Int(false),
                        IsDisabled = c.Boolean(false),
                        IsMoving = c.Boolean(false),
                        MapId = c.Short(false),
                        MapX = c.Short(false),
                        MapY = c.Short(false),
                        MonsterVNum = c.Short(false),
                        Name = c.String(),
                        Position = c.Byte(false)
                    })
                .PrimaryKey(t => t.MapMonsterId)
                .ForeignKey("dbo.Map", t => t.MapId)
                .ForeignKey("dbo.NpcMonster", t => t.MonsterVNum)
                .Index(t => t.MapId)
                .Index(t => t.MonsterVNum);

            CreateTable(
                    "dbo.MapNpc",
                    c => new
                    {
                        MapNpcId = c.Int(false),
                        Dialog = c.Short(false),
                        Effect = c.Short(false),
                        EffectDelay = c.Short(false),
                        IsDisabled = c.Boolean(false),
                        IsMoving = c.Boolean(false),
                        IsSitting = c.Boolean(false),
                        MapId = c.Short(false),
                        MapX = c.Short(false),
                        MapY = c.Short(false),
                        Name = c.String(),
                        NpcVNum = c.Short(false),
                        Position = c.Byte(false)
                    })
                .PrimaryKey(t => t.MapNpcId)
                .ForeignKey("dbo.Map", t => t.MapId)
                .ForeignKey("dbo.NpcMonster", t => t.NpcVNum)
                .Index(t => t.MapId)
                .Index(t => t.NpcVNum);

            CreateTable(
                    "dbo.RecipeList",
                    c => new
                    {
                        RecipeListId = c.Int(false, true),
                        ItemVNum = c.Short(),
                        MapNpcId = c.Int(),
                        RecipeId = c.Short(false)
                    })
                .PrimaryKey(t => t.RecipeListId)
                .ForeignKey("dbo.Item", t => t.ItemVNum)
                .ForeignKey("dbo.MapNpc", t => t.MapNpcId)
                .ForeignKey("dbo.Recipe", t => t.RecipeId)
                .Index(t => t.ItemVNum)
                .Index(t => t.MapNpcId)
                .Index(t => t.RecipeId);

            CreateTable(
                    "dbo.Recipe",
                    c => new
                    {
                        RecipeId = c.Short(false, true),
                        Amount = c.Short(false),
                        ItemVNum = c.Short(false)
                    })
                .PrimaryKey(t => t.RecipeId)
                .ForeignKey("dbo.Item", t => t.ItemVNum)
                .Index(t => t.ItemVNum);

            CreateTable(
                    "dbo.RecipeItem",
                    c => new
                    {
                        RecipeItemId = c.Short(false, true),
                        Amount = c.Short(false),
                        ItemVNum = c.Short(false),
                        RecipeId = c.Short(false)
                    })
                .PrimaryKey(t => t.RecipeItemId)
                .ForeignKey("dbo.Recipe", t => t.RecipeId)
                .ForeignKey("dbo.Item", t => t.ItemVNum)
                .Index(t => t.ItemVNum)
                .Index(t => t.RecipeId);

            CreateTable(
                    "dbo.Shop",
                    c => new
                    {
                        ShopId = c.Int(false, true),
                        MapNpcId = c.Int(false),
                        MenuType = c.Byte(false),
                        Name = c.String(maxLength: 255),
                        ShopType = c.Byte(false)
                    })
                .PrimaryKey(t => t.ShopId)
                .ForeignKey("dbo.MapNpc", t => t.MapNpcId)
                .Index(t => t.MapNpcId);

            CreateTable(
                    "dbo.ShopItem",
                    c => new
                    {
                        ShopItemId = c.Int(false, true),
                        Color = c.Byte(false),
                        ItemVNum = c.Short(false),
                        Rare = c.Short(false),
                        ShopId = c.Int(false),
                        Slot = c.Byte(false),
                        Type = c.Byte(false),
                        Upgrade = c.Byte(false)
                    })
                .PrimaryKey(t => t.ShopItemId)
                .ForeignKey("dbo.Shop", t => t.ShopId)
                .ForeignKey("dbo.Item", t => t.ItemVNum)
                .Index(t => t.ItemVNum)
                .Index(t => t.ShopId);

            CreateTable(
                    "dbo.ShopSkill",
                    c => new
                    {
                        ShopSkillId = c.Int(false, true),
                        ShopId = c.Int(false),
                        SkillVNum = c.Short(false),
                        Slot = c.Byte(false),
                        Type = c.Byte(false)
                    })
                .PrimaryKey(t => t.ShopSkillId)
                .ForeignKey("dbo.Skill", t => t.SkillVNum)
                .ForeignKey("dbo.Shop", t => t.ShopId)
                .Index(t => t.ShopId)
                .Index(t => t.SkillVNum);

            CreateTable(
                    "dbo.Skill",
                    c => new
                    {
                        SkillVNum = c.Short(false),
                        AttackAnimation = c.Short(false),
                        CastAnimation = c.Short(false),
                        CastEffect = c.Short(false),
                        CastId = c.Short(false),
                        CastTime = c.Short(false),
                        Class = c.Byte(false),
                        Cooldown = c.Short(false),
                        CPCost = c.Byte(false),
                        Duration = c.Short(false),
                        Effect = c.Short(false),
                        Element = c.Byte(false),
                        HitType = c.Byte(false),
                        ItemVNum = c.Short(false),
                        Level = c.Byte(false),
                        LevelMinimum = c.Byte(false),
                        MinimumAdventurerLevel = c.Byte(false),
                        MinimumArcherLevel = c.Byte(false),
                        MinimumMagicianLevel = c.Byte(false),
                        MinimumSwordmanLevel = c.Byte(false),
                        MpCost = c.Short(false),
                        Name = c.String(maxLength: 255),
                        Price = c.Int(false),
                        Range = c.Byte(false),
                        SkillType = c.Byte(false),
                        TargetRange = c.Byte(false),
                        TargetType = c.Byte(false),
                        Type = c.Byte(false),
                        UpgradeSkill = c.Short(false),
                        UpgradeType = c.Short(false)
                    })
                .PrimaryKey(t => t.SkillVNum);

            CreateTable(
                    "dbo.CharacterSkill",
                    c => new
                    {
                        Id = c.Guid(false),
                        CharacterId = c.Long(false),
                        SkillVNum = c.Short(false)
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Skill", t => t.SkillVNum)
                .ForeignKey("dbo.Character", t => t.CharacterId)
                .Index(t => t.CharacterId)
                .Index(t => t.SkillVNum);

            CreateTable(
                    "dbo.Combo",
                    c => new
                    {
                        ComboId = c.Int(false, true),
                        Animation = c.Short(false),
                        Effect = c.Short(false),
                        Hit = c.Short(false),
                        SkillVNum = c.Short(false)
                    })
                .PrimaryKey(t => t.ComboId)
                .ForeignKey("dbo.Skill", t => t.SkillVNum)
                .Index(t => t.SkillVNum);

            CreateTable(
                    "dbo.NpcMonsterSkill",
                    c => new
                    {
                        NpcMonsterSkillId = c.Long(false, true),
                        NpcMonsterVNum = c.Short(false),
                        Rate = c.Short(false),
                        SkillVNum = c.Short(false)
                    })
                .PrimaryKey(t => t.NpcMonsterSkillId)
                .ForeignKey("dbo.Skill", t => t.SkillVNum)
                .ForeignKey("dbo.NpcMonster", t => t.NpcMonsterVNum)
                .Index(t => t.NpcMonsterVNum)
                .Index(t => t.SkillVNum);

            CreateTable(
                    "dbo.Teleporter",
                    c => new
                    {
                        TeleporterId = c.Short(false, true),
                        Index = c.Short(false),
                        Type = c.Byte(false),
                        MapId = c.Short(false),
                        MapNpcId = c.Int(false),
                        MapX = c.Short(false),
                        MapY = c.Short(false)
                    })
                .PrimaryKey(t => t.TeleporterId)
                .ForeignKey("dbo.MapNpc", t => t.MapNpcId)
                .ForeignKey("dbo.Map", t => t.MapId)
                .Index(t => t.MapId)
                .Index(t => t.MapNpcId);

            CreateTable(
                    "dbo.Portal",
                    c => new
                    {
                        PortalId = c.Int(false, true),
                        DestinationMapId = c.Short(false),
                        DestinationX = c.Short(false),
                        DestinationY = c.Short(false),
                        IsDisabled = c.Boolean(false),
                        SourceMapId = c.Short(false),
                        SourceX = c.Short(false),
                        SourceY = c.Short(false),
                        Type = c.Short(false)
                    })
                .PrimaryKey(t => t.PortalId)
                .ForeignKey("dbo.Map", t => t.DestinationMapId)
                .ForeignKey("dbo.Map", t => t.SourceMapId)
                .Index(t => t.DestinationMapId)
                .Index(t => t.SourceMapId);

            CreateTable(
                    "dbo.Respawn",
                    c => new
                    {
                        RespawnId = c.Long(false, true),
                        CharacterId = c.Long(false),
                        MapId = c.Short(false),
                        RespawnMapTypeId = c.Long(false),
                        X = c.Short(false),
                        Y = c.Short(false)
                    })
                .PrimaryKey(t => t.RespawnId)
                .ForeignKey("dbo.Map", t => t.MapId)
                .ForeignKey("dbo.RespawnMapType", t => t.RespawnMapTypeId)
                .ForeignKey("dbo.Character", t => t.CharacterId)
                .Index(t => t.CharacterId)
                .Index(t => t.MapId)
                .Index(t => t.RespawnMapTypeId);

            CreateTable(
                    "dbo.RespawnMapType",
                    c => new
                    {
                        RespawnMapTypeId = c.Long(false),
                        DefaultMapId = c.Short(false),
                        DefaultX = c.Short(false),
                        DefaultY = c.Short(false),
                        Name = c.String(maxLength: 255)
                    })
                .PrimaryKey(t => t.RespawnMapTypeId)
                .ForeignKey("dbo.Map", t => t.DefaultMapId)
                .Index(t => t.DefaultMapId);

            CreateTable(
                    "dbo.ScriptedInstance",
                    c => new
                    {
                        ScriptedInstanceId = c.Short(false, true),
                        MapId = c.Short(false),
                        PositionX = c.Short(false),
                        PositionY = c.Short(false),
                        Script = c.String(),
                        Type = c.Byte(false),
                        Label = c.String()
                    })
                .PrimaryKey(t => t.ScriptedInstanceId)
                .ForeignKey("dbo.Map", t => t.MapId)
                .Index(t => t.MapId);

            CreateTable(
                    "dbo.Mate",
                    c => new
                    {
                        MateId = c.Long(false, true),
                        Attack = c.Byte(false),
                        CanPickUp = c.Boolean(false),
                        CharacterId = c.Long(false),
                        Defence = c.Byte(false),
                        Direction = c.Byte(false),
                        Experience = c.Long(false),
                        Hp = c.Double(false),
                        IsSummonable = c.Boolean(false),
                        IsTeamMember = c.Boolean(false),
                        Level = c.Byte(false),
                        Loyalty = c.Short(false),
                        MapX = c.Short(false),
                        MapY = c.Short(false),
                        MateType = c.Byte(false),
                        Mp = c.Double(false),
                        Name = c.String(maxLength: 255),
                        NpcMonsterVNum = c.Short(false),
                        Skin = c.Short(false)
                    })
                .PrimaryKey(t => t.MateId)
                .ForeignKey("dbo.NpcMonster", t => t.NpcMonsterVNum)
                .ForeignKey("dbo.Character", t => t.CharacterId)
                .Index(t => t.CharacterId)
                .Index(t => t.NpcMonsterVNum);

            CreateTable(
                    "dbo.Mail",
                    c => new
                    {
                        MailId = c.Long(false, true),
                        AttachmentAmount = c.Short(false),
                        AttachmentLevel = c.Byte(false),
                        AttachmentRarity = c.Byte(false),
                        AttachmentUpgrade = c.Byte(false),
                        AttachmentDesign = c.Short(false),
                        AttachmentVNum = c.Short(),
                        Date = c.DateTime(false),
                        EqPacket = c.String(maxLength: 255),
                        IsOpened = c.Boolean(false),
                        IsSenderCopy = c.Boolean(false),
                        Message = c.String(maxLength: 255),
                        ReceiverId = c.Long(false),
                        SenderClass = c.Byte(false),
                        SenderGender = c.Byte(false),
                        SenderHairColor = c.Byte(false),
                        SenderHairStyle = c.Byte(false),
                        SenderId = c.Long(false),
                        SenderMorphId = c.Short(false),
                        Title = c.String(maxLength: 255)
                    })
                .PrimaryKey(t => t.MailId)
                .ForeignKey("dbo.Item", t => t.AttachmentVNum)
                .ForeignKey("dbo.Character", t => t.SenderId)
                .ForeignKey("dbo.Character", t => t.ReceiverId)
                .Index(t => t.AttachmentVNum)
                .Index(t => t.ReceiverId)
                .Index(t => t.SenderId);

            CreateTable(
                    "dbo.RollGeneratedItem",
                    c => new
                    {
                        RollGeneratedItemId = c.Short(false, true),
                        IsRareRandom = c.Boolean(false),
                        ItemGeneratedAmount = c.Short(false),
                        ItemGeneratedVNum = c.Short(false),
                        ItemGeneratedDesign = c.Short(false),
                        MaximumOriginalItemRare = c.Byte(false),
                        MinimumOriginalItemRare = c.Byte(false),
                        OriginalItemDesign = c.Short(false),
                        OriginalItemVNum = c.Short(false),
                        Probability = c.Short(false)
                    })
                .PrimaryKey(t => t.RollGeneratedItemId)
                .ForeignKey("dbo.Item", t => t.ItemGeneratedVNum)
                .ForeignKey("dbo.Item", t => t.OriginalItemVNum)
                .Index(t => t.ItemGeneratedVNum)
                .Index(t => t.OriginalItemVNum);

            CreateTable(
                    "dbo.MinilandObject",
                    c => new
                    {
                        MinilandObjectId = c.Long(false, true),
                        CharacterId = c.Long(false),
                        ItemInstanceId = c.Guid(),
                        Level1BoxAmount = c.Byte(false),
                        Level2BoxAmount = c.Byte(false),
                        Level3BoxAmount = c.Byte(false),
                        Level4BoxAmount = c.Byte(false),
                        Level5BoxAmount = c.Byte(false),
                        MapX = c.Short(false),
                        MapY = c.Short(false)
                    })
                .PrimaryKey(t => t.MinilandObjectId)
                .ForeignKey("dbo.ItemInstance", t => t.ItemInstanceId)
                .ForeignKey("dbo.Character", t => t.CharacterId)
                .Index(t => t.CharacterId)
                .Index(t => t.ItemInstanceId);

            CreateTable(
                    "dbo.CharacterRelation",
                    c => new
                    {
                        CharacterRelationId = c.Long(false, true),
                        CharacterId = c.Long(false),
                        RelatedCharacterId = c.Long(false),
                        RelationType = c.Short(false)
                    })
                .PrimaryKey(t => t.CharacterRelationId)
                .ForeignKey("dbo.Character", t => t.CharacterId)
                .ForeignKey("dbo.Character", t => t.RelatedCharacterId)
                .Index(t => t.CharacterId)
                .Index(t => t.RelatedCharacterId);

            CreateTable(
                    "dbo.FamilyCharacter",
                    c => new
                    {
                        FamilyCharacterId = c.Long(false, true),
                        Authority = c.Byte(false),
                        CharacterId = c.Long(false),
                        DailyMessage = c.String(maxLength: 255),
                        Experience = c.Int(false),
                        FamilyId = c.Long(false),
                        Rank = c.Byte(false)
                    })
                .PrimaryKey(t => t.FamilyCharacterId)
                .ForeignKey("dbo.Character", t => t.CharacterId)
                .ForeignKey("dbo.Family", t => t.FamilyId)
                .Index(t => t.CharacterId)
                .Index(t => t.FamilyId);

            CreateTable(
                    "dbo.Family",
                    c => new
                    {
                        FamilyId = c.Long(false, true),
                        FamilyExperience = c.Int(false),
                        FamilyFaction = c.Byte(false),
                        FamilyHeadGender = c.Byte(false),
                        FamilyLevel = c.Byte(false),
                        FamilyMessage = c.String(maxLength: 255),
                        LastFactionChange = c.Long(false),
                        ManagerAuthorityType = c.Byte(false),
                        ManagerCanGetHistory = c.Boolean(false),
                        ManagerCanInvite = c.Boolean(false),
                        ManagerCanNotice = c.Boolean(false),
                        ManagerCanShout = c.Boolean(false),
                        MaxSize = c.Short(false),
                        MemberAuthorityType = c.Byte(false),
                        MemberCanGetHistory = c.Boolean(false),
                        Name = c.String(maxLength: 255),
                        WarehouseSize = c.Byte(false)
                    })
                .PrimaryKey(t => t.FamilyId);

            CreateTable(
                    "dbo.FamilyLog",
                    c => new
                    {
                        FamilyLogId = c.Long(false, true),
                        FamilyId = c.Long(false),
                        FamilyLogData = c.String(maxLength: 255),
                        FamilyLogType = c.Byte(false),
                        Timestamp = c.DateTime(false)
                    })
                .PrimaryKey(t => t.FamilyLogId)
                .ForeignKey("dbo.Family", t => t.FamilyId)
                .Index(t => t.FamilyId);

            CreateTable(
                    "dbo.GeneralLog",
                    c => new
                    {
                        LogId = c.Long(false, true),
                        AccountId = c.Long(),
                        CharacterId = c.Long(),
                        IpAddress = c.String(maxLength: 255),
                        LogData = c.String(maxLength: 255),
                        LogType = c.String(),
                        Timestamp = c.DateTime(false)
                    })
                .PrimaryKey(t => t.LogId)
                .ForeignKey("dbo.Account", t => t.AccountId)
                .ForeignKey("dbo.Character", t => t.CharacterId)
                .Index(t => t.AccountId)
                .Index(t => t.CharacterId);

            CreateTable(
                    "dbo.MinigameLog",
                    c => new
                    {
                        MinigameLogId = c.Long(false, true),
                        StartTime = c.Long(false),
                        EndTime = c.Long(false),
                        Score = c.Int(false),
                        Minigame = c.Byte(false),
                        CharacterId = c.Long(false)
                    })
                .PrimaryKey(t => t.MinigameLogId)
                .ForeignKey("dbo.Character", t => t.CharacterId)
                .Index(t => t.CharacterId);

            CreateTable(
                    "dbo.QuicklistEntry",
                    c => new
                    {
                        Id = c.Guid(false),
                        CharacterId = c.Long(false),
                        Morph = c.Short(false),
                        Pos = c.Short(false),
                        Q1 = c.Short(false),
                        Q2 = c.Short(false),
                        Slot = c.Short(false),
                        Type = c.Short(false)
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Character", t => t.CharacterId)
                .Index(t => t.CharacterId);

            CreateTable(
                    "dbo.StaticBonus",
                    c => new
                    {
                        StaticBonusId = c.Long(false, true),
                        CharacterId = c.Long(false),
                        DateEnd = c.DateTime(false),
                        StaticBonusType = c.Byte(false)
                    })
                .PrimaryKey(t => t.StaticBonusId)
                .ForeignKey("dbo.Character", t => t.CharacterId)
                .Index(t => t.CharacterId);

            CreateTable(
                    "dbo.PenaltyLog",
                    c => new
                    {
                        PenaltyLogId = c.Int(false, true),
                        AccountId = c.Long(false),
                        IP = c.String(),
                        AdminName = c.String(),
                        DateEnd = c.DateTime(false),
                        DateStart = c.DateTime(false),
                        Penalty = c.Byte(false),
                        Reason = c.String(maxLength: 255)
                    })
                .PrimaryKey(t => t.PenaltyLogId)
                .ForeignKey("dbo.Account", t => t.AccountId)
                .Index(t => t.AccountId);

            CreateTable(
                    "dbo.CellonOption",
                    c => new
                    {
                        CellonOptionId = c.Long(false, true),
                        EquipmentSerialId = c.Guid(false),
                        Level = c.Byte(false),
                        Type = c.Byte(false),
                        Value = c.Int(false)
                    })
                .PrimaryKey(t => t.CellonOptionId);

            CreateTable(
                    "dbo.CharacterQuest",
                    c => new
                    {
                        Id = c.Guid(false),
                        CharacterId = c.Long(false),
                        QuestId = c.Long(false),
                        FirstObjective = c.Int(false),
                        SecondObjective = c.Int(false),
                        ThirdObjective = c.Int(false),
                        FourthObjective = c.Int(false),
                        FifthObjective = c.Int(false),
                        IsMainQuest = c.Boolean(false)
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Quest", t => t.QuestId, true)
                .Index(t => t.QuestId);

            CreateTable(
                    "dbo.Quest",
                    c => new
                    {
                        QuestId = c.Long(false),
                        QuestType = c.Int(false),
                        LevelMin = c.Byte(false),
                        LevelMax = c.Byte(false),
                        StartDialogId = c.Int(),
                        EndDialogId = c.Int(),
                        DialogNpcVNum = c.Int(),
                        DialogNpcId = c.Int(),
                        TargetMap = c.Short(),
                        TargetX = c.Short(),
                        TargetY = c.Short(),
                        InfoId = c.Int(false),
                        NextQuestId = c.Long(),
                        IsDaily = c.Boolean(false)
                    })
                .PrimaryKey(t => t.QuestId);

            CreateTable(
                    "dbo.MaintenanceLog",
                    c => new
                    {
                        LogId = c.Long(false, true),
                        DateEnd = c.DateTime(false),
                        DateStart = c.DateTime(false),
                        Reason = c.String(maxLength: 255)
                    })
                .PrimaryKey(t => t.LogId);

            CreateTable(
                    "dbo.PartnerSkill",
                    c => new
                    {
                        PartnerSkillId = c.Long(false, true),
                        EquipmentSerialId = c.Guid(false),
                        SkillVNum = c.Short(false),
                        Level = c.Byte(false)
                    })
                .PrimaryKey(t => t.PartnerSkillId);

            CreateTable(
                    "dbo.QuestLog",
                    c => new
                    {
                        Id = c.Long(false, true),
                        CharacterId = c.Long(false),
                        QuestId = c.Long(false),
                        IpAddress = c.String(),
                        LastDaily = c.DateTime()
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                    "dbo.QuestObjective",
                    c => new
                    {
                        QuestObjectiveId = c.Int(false, true),
                        QuestId = c.Int(false),
                        Data = c.Int(),
                        Objective = c.Int(),
                        SpecialData = c.Int(),
                        DropRate = c.Int(),
                        ObjectiveIndex = c.Byte(false)
                    })
                .PrimaryKey(t => t.QuestObjectiveId);

            CreateTable(
                    "dbo.QuestReward",
                    c => new
                    {
                        QuestRewardId = c.Long(false, true),
                        RewardType = c.Byte(false),
                        Data = c.Int(false),
                        Design = c.Byte(false),
                        Rarity = c.Byte(false),
                        Upgrade = c.Byte(false),
                        Amount = c.Int(false),
                        QuestId = c.Long(false)
                    })
                .PrimaryKey(t => t.QuestRewardId);

            CreateTable(
                    "dbo.ShellEffect",
                    c => new
                    {
                        ShellEffectId = c.Long(false, true),
                        Effect = c.Byte(false),
                        EffectLevel = c.Byte(false),
                        EquipmentSerialId = c.Guid(false),
                        Value = c.Short(false)
                    })
                .PrimaryKey(t => t.ShellEffectId);
        }

        #endregion
    }
}