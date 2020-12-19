namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Runes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RuneEffects",
                c => new
                {
                    RuneEffectId = c.Long(nullable: false, identity: true),
                    EquipmentSerialId = c.Guid(nullable: false),
                    Type = c.Byte(nullable: false),
                    SubType = c.Byte(nullable: false),
                    FirstData = c.Int(nullable: false),
                    SecondData = c.Int(nullable: false),
                    ThirdData = c.Int(nullable: false),
                    IsPower = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.RuneEffectId);
            AddColumn("dbo.ItemInstance", "RuneUpgrade", c => c.Byte(nullable: false));
            AddColumn("dbo.ItemInstance", "RuneBroke", c => c.Boolean(nullable: false));
            AddColumn("dbo.ItemInstance", "RuneCount", c => c.Byte(nullable: false));
            AddColumn("dbo.RuneEffects", "ThirdDada", c => c.Int(nullable: false));
            AddColumn("dbo.ShellEffect", "RuneUpgrade", c => c.Byte(nullable: false));
            DropColumn("dbo.ItemInstance", "RuneAmount");
            DropColumn("dbo.RuneEffects", "ThirdData");
            DropColumn("dbo.RuneEffects", "IsPower");
        }

        public override void Down()
        {
            DropTable("dbo.RuneEffects");
            AddColumn("dbo.RuneEffects", "IsPower", c => c.Boolean(nullable: false));
            AddColumn("dbo.RuneEffects", "ThirdData", c => c.Int(nullable: false));
            AddColumn("dbo.ItemInstance", "RuneAmount", c => c.Byte(nullable: false));
            DropColumn("dbo.ShellEffect", "RuneUpgrade");
            DropColumn("dbo.RuneEffects", "ThirdDada");
            DropColumn("dbo.ItemInstance", "RuneCount");
            DropColumn("dbo.ItemInstance", "RuneBroke");
            DropColumn("dbo.ItemInstance", "RuneUpgrade");
            AddPrimaryKey("dbo.RuneEffects", "RuneEffectId");
        }
    }
}
