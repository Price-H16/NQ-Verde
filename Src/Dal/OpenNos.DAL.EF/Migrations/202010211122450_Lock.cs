namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Lock : DbMigration
    {
        #region Methods

        public override void Down()
        {
            DropColumn("dbo.Character", "VerifiedLock");
        }

        public override void Up()
        {
            AddColumn("dbo.Character", "VerifiedLock", c => c.Boolean(nullable: false));
        }

        #endregion
    }
}