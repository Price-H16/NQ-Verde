using System.Data.Entity.Migrations;

namespace OpenNos.DAL.EF.Migrations
{
    public partial class QuestTimeSpaceId : DbMigration
    {
        #region Methods

        public override void Down()
        {
            DropColumn("dbo.ScriptedInstance", "QuestTimeSpaceId");
        }

        public override void Up()
        {
            AddColumn("dbo.ScriptedInstance", "QuestTimeSpaceId", c => c.Int(false));
        }

        #endregion
    }
}