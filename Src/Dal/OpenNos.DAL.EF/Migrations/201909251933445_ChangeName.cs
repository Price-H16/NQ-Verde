using System.Data.Entity.Migrations;

namespace OpenNos.DAL.EF.Migrations
{
    public partial class ChangeName : DbMigration
    {
        #region Methods

        public override void Down()
        {
            DropColumn("dbo.Character", "IsChangeName");
        }

        public override void Up()
        {
            AddColumn("dbo.Character", "IsChangeName", c => c.Boolean(false));
        }

        #endregion
    }
}