using System.Data.Entity.Migrations;

namespace OpenNos.DAL.EF.Migrations
{
    public partial class BoxItem : DbMigration
    {
        #region Methods

        public override void Down()
        {
            DropTable("dbo.BoxItem");
        }

        public override void Up()
        {
            CreateTable(
                    "dbo.BoxItem",
                    c => new
                    {
                        BoxItemId = c.Long(false, true),
                        OriginalItemVNum = c.Short(false),
                        OriginalItemDesign = c.Short(false),
                        ItemGeneratedAmount = c.Short(false),
                        ItemGeneratedVNum = c.Short(false),
                        ItemGeneratedDesign = c.Short(false),
                        ItemGeneratedRare = c.Byte(false),
                        ItemGeneratedUpgrade = c.Byte(false),
                        Probability = c.Byte(false)
                    })
                .PrimaryKey(t => t.BoxItemId);
        }

        #endregion
    }
}