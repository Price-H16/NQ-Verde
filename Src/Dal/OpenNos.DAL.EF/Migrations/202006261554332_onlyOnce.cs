namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class onlyOnce : DbMigration
    {
        #region Methods

        public override void Down()
        {
            DropColumn("dbo.Quest", "CanBeDoneOnlyOnce");
        }

        public override void Up()
        {
            AddColumn("dbo.Quest", "CanBeDoneOnlyOnce", c => c.Boolean(nullable: false));
        }

        #endregion
    }
}