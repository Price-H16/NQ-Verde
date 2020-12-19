namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class onlyOnce : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Quest", "CanBeDoneOnlyOnce", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Quest", "CanBeDoneOnlyOnce");
        }
    }
}
