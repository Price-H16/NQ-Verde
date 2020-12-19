namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LockCoe : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Character", "LockCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Character", "LockCode");
        }
    }
}
