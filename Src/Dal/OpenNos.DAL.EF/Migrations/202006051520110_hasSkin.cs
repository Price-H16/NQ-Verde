namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class hasSkin : DbMigration
    {
        #region Methods

        public override void Down()
        {
            DropColumn("dbo.ItemInstance", "HasSkin");
        }

        public override void Up()
        {
            AddColumn("dbo.ItemInstance", "HasSkin", c => c.Boolean());
        }

        #endregion
    }
}