using System.Data.Entity.Migrations;

namespace OpenNos.DAL.EF.Migrations
{
    public partial class MaxPartnerCount : DbMigration
    {
        #region Methods

        public override void Down()
        {
            DropColumn("dbo.Character", "MaxPartnerCount");
        }

        public override void Up()
        {
            AddColumn("dbo.Character", "MaxPartnerCount", c => c.Byte(false));
        }

        #endregion
    }
}