using System.Data.Entity.Migrations;

namespace OpenNos.DAL.EF.Migrations
{
    public partial class ClassType_In_Portal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Portal", "RequiredClass", c => c.Byte());
        }

        public override void Down()
        {
            DropColumn("dbo.Portal", "RequiredClass");
        }
    }
}