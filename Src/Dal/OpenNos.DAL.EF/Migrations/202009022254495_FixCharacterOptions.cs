namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class FixCharacterOptions : DbMigration
    {
        #region Methods

        public override void Down()
        {
            DropColumn("dbo.Character", "UiBlocked");
            DropColumn("dbo.Character", "HideHat");
        }

        public override void Up()
        {
            AddColumn("dbo.Character", "HideHat", c => c.Boolean(nullable: false));
            AddColumn("dbo.Character", "UiBlocked", c => c.Boolean(nullable: false));
        }

        #endregion
    }
}