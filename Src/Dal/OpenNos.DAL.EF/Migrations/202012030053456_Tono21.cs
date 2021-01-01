namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Tono21 : DbMigration
    {
        #region Methods

        public override void Down()
        {
            DropColumn("dbo.Character", "GoldBank");
        }

        public override void Up()
        {
            AddColumn("dbo.Character", "GoldBank", c => c.Long(nullable: false));
        }

        #endregion
    }
}