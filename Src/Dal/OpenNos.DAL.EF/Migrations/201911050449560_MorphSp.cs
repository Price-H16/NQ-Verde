using System.Data.Entity.Migrations;

namespace OpenNos.DAL.EF.Migrations
{
    public partial class MorphSp : DbMigration
    {
        #region Methods

        public override void Down()
        {
            DropColumn("dbo.Item", "MorphSp");
        }

        public override void Up()
        {
            AddColumn("dbo.Item", "MorphSp", c => c.Short(false));
        }

        #endregion
    }
}