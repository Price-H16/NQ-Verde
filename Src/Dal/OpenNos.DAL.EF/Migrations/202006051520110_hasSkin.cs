namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hasSkin : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemInstance", "HasSkin", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemInstance", "HasSkin");
        }
    }
}
