using System.Data.Entity.Migrations;

namespace OpenNos.DAL.EF.Migrations
{
    public partial class GoldBank_Fix : DbMigration
    {
        #region Methods

        public override void Down()
        {
            AddColumn("dbo.Character", "GoldBank", c => c.Long(false));
            DropColumn("dbo.Account", "GoldBank");
        }

        public override void Up()
        {
            AddColumn("dbo.Account", "GoldBank", c => c.Long(false));
            DropColumn("dbo.Character", "GoldBank");
        }

        #endregion
    }
}