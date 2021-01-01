namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class RbbStat : DbMigration
    {
        #region Methods

        public override void Down()
        {
            DropColumn("dbo.Character", "RBBLose");
            DropColumn("dbo.Character", "RBBWin");
        }

        public override void Up()
        {
            AddColumn("dbo.Character", "RBBWin", c => c.Long(nullable: false));
            AddColumn("dbo.Character", "RBBLose", c => c.Long(nullable: false));
        }

        #endregion
    }
}