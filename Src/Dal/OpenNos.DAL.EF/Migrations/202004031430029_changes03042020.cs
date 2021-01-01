namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class changes03042020 : DbMigration
    {
        #region Methods

        public override void Down()
        {
            DropColumn("dbo.Character", "MobKillCounter");
        }

        public override void Up()
        {
            AddColumn("dbo.Character", "MobKillCounter", c => c.Int(nullable: false));
        }

        #endregion
    }
}