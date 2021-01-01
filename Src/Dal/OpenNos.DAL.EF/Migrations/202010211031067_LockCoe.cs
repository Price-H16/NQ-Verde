namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class LockCoe : DbMigration
    {
        #region Methods

        public override void Down()
        {
            DropColumn("dbo.Character", "LockCode");
        }

        public override void Up()
        {
            AddColumn("dbo.Character", "LockCode", c => c.String());
        }

        #endregion
    }
}