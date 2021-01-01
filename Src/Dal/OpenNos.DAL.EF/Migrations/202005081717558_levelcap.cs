namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class levelcap : DbMigration
    {
        #region Methods

        public override void Down()
        {
            DropColumn("dbo.Character", "UnlockedHLevel");
        }

        public override void Up()
        {
            AddColumn("dbo.Character", "UnlockedHLevel", c => c.Byte(nullable: false));
        }

        #endregion
    }
}