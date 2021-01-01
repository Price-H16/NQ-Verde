namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class ArenaScore : DbMigration
    {
        #region Methods

        public override void Down()
        {
            DropColumn("dbo.Character", "ArenaDeath");
            DropColumn("dbo.Character", "ArenaKill");
        }

        public override void Up()
        {
            AddColumn("dbo.Character", "ArenaKill", c => c.Int(nullable: false));
            AddColumn("dbo.Character", "ArenaDeath", c => c.Int(nullable: false));
        }

        #endregion
    }
}