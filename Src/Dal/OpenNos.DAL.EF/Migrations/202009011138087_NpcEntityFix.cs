namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class NpcEntityFix : DbMigration
    {
        #region Methods

        public override void Down()
        {
            DropColumn("dbo.NpcMonster", "EvolvePet");
        }

        public override void Up()
        {
            AddColumn("dbo.NpcMonster", "EvolvePet", c => c.Short(nullable: false));
        }

        #endregion
    }
}