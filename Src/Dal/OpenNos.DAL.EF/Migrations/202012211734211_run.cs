namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class run : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.RuneEffect");
            AddColumn("dbo.ItemInstance", "RuneAmount", c => c.Byte(nullable: false));
            AddPrimaryKey("dbo.RuneEffect", "RuneEffectId");
            DropColumn("dbo.ItemInstance", "RuneUpgrade");
            DropColumn("dbo.ItemInstance", "RuneBroke");
            DropColumn("dbo.ItemInstance", "RuneCount");
            DropColumn("dbo.ShellEffect", "RuneUpgrade");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ShellEffect", "RuneUpgrade", c => c.Byte(nullable: false));
            AddColumn("dbo.ItemInstance", "RuneCount", c => c.Byte(nullable: false));
            AddColumn("dbo.ItemInstance", "RuneBroke", c => c.Boolean(nullable: false));
            AddColumn("dbo.ItemInstance", "RuneUpgrade", c => c.Byte(nullable: false));
            DropPrimaryKey("dbo.RuneEffect");
            DropColumn("dbo.ItemInstance", "RuneAmount");
            AddPrimaryKey("dbo.RuneEffect", "RuneEffectId");
        }
    }
}
