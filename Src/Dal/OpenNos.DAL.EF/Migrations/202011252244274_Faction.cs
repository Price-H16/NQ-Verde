namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Faction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Character", "ArenaDie", c => c.Long(nullable: false));
            AddColumn("dbo.Character", "ArenaTc", c => c.Long(nullable: false));
            AddColumn("dbo.Character", "LastFactionChange", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Character", "LastFactionChange");
            DropColumn("dbo.Character", "ArenaTc");
            DropColumn("dbo.Character", "ArenaDie");
        }
    }
}
