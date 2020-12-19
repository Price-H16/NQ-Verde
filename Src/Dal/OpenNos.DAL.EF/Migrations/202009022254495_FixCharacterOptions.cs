namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixCharacterOptions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Character", "HideHat", c => c.Boolean(nullable: false));
            AddColumn("dbo.Character", "UiBlocked", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Character", "UiBlocked");
            DropColumn("dbo.Character", "HideHat");
        }
    }
}
