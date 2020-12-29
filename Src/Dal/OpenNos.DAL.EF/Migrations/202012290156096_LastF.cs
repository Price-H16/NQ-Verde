namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LastF : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Character", "ItemShopShip", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Character", "ItemShopShip");
        }
    }
}
