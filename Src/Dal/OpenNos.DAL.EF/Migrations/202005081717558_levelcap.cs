namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class levelcap : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Character", "UnlockedHLevel", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Character", "UnlockedHLevel");
        }
    }
}
