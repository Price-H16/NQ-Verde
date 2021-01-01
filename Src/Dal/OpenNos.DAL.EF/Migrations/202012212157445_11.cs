namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class _11 : DbMigration
    {
        #region Methods

        public override void Down()
        {
            CreateTable(
                "dbo.FamilyQuests",
                c => new
                {
                    FamilyQuestsId = c.Long(nullable: false, identity: true),
                    FamilyId = c.Long(nullable: false),
                    QuestType = c.Byte(nullable: false),
                    QuestId = c.Short(nullable: false),
                    Do = c.Boolean(nullable: false),
                    Date = c.String(),
                    Count = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.FamilyQuestsId);

            CreateTable(
                "dbo.ChatLog",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    AccountId = c.Int(nullable: false),
                    CharacterId = c.Int(nullable: false),
                    CharacterName = c.String(),
                    DateTime = c.DateTime(nullable: false),
                    Message = c.String(),
                    TargetCharacterId = c.Int(),
                    TargetCharacterName = c.String(),
                    Type = c.Byte(nullable: false),
                })
                .PrimaryKey(t => t.Id);
        }

        public override void Up()
        {
        }

        #endregion
    }
}