namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class RR : DbMigration
    {
        #region Methods

        public override void Down()
        {
            AlterColumn("dbo.RuneEffect", "Type", c => c.Byte(nullable: false));
            AlterColumn("dbo.Card", "BuffType", c => c.Byte(nullable: false));
            DropColumn("dbo.Character", "ItemShopShip");
        }

        public override void Up()
        {
            AddColumn("dbo.Character", "ItemShopShip", c => c.Int(nullable: false));
            AlterColumn("dbo.Card", "BuffType", c => c.Int(nullable: false));
            AlterColumn("dbo.RuneEffect", "Type", c => c.Int(nullable: false));
        }

        #endregion
    }
}