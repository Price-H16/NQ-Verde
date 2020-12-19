namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ArenaScore : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Character", "ArenaKill", c => c.Int(nullable: false));
            AddColumn("dbo.Character", "ArenaDeath", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Character", "ArenaDeath");
            DropColumn("dbo.Character", "ArenaKill");
        }
    }
}
