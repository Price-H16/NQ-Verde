namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Lock : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Character", "VerifiedLock", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Character", "VerifiedLock");
        }
    }
}
