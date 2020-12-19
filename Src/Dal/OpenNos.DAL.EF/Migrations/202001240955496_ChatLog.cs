using System.Data.Entity.Migrations;

namespace OpenNos.DAL.EF.Migrations
{
    public partial class ChatLog : DbMigration
    {
        #region Methods

        public override void Down()
        {
            DropTable("dbo.ChatLog");
        }

        public override void Up()
        {
            CreateTable(
                    "dbo.ChatLog",
                    c => new
                    {
                        Id = c.Int(false, true),
                        AccountId = c.Int(false),
                        CharacterId = c.Int(false),
                        CharacterName = c.String(),
                        DateTime = c.DateTime(false),
                        Message = c.String(),
                        TargetCharacterId = c.Int(),
                        TargetCharacterName = c.String(),
                        Type = c.Byte(false)
                    })
                .PrimaryKey(t => t.Id);
        }

        #endregion
    }
}