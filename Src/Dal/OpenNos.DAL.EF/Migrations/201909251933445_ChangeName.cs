using System.Data.Entity.Migrations;

namespace OpenNos.DAL.EF.Migrations
{
    public partial class ChangeName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Character", "IsChangeName", c => c.Boolean(false));
        }

        public override void Down()
        {
            DropColumn("dbo.Character", "IsChangeName");
        }
    }
}