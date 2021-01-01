namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class LastF : DbMigration
    {
        #region Methods

        public override void Down()
        {
            DropColumn("dbo.Character", "ItemShopShip");
        }

        public override void Up()
        {
            AddColumn("dbo.Character", "ItemShopShip", c => c.Int(nullable: false));
        }

        #endregion
    }
}