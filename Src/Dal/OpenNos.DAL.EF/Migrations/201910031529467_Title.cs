using System.Data.Entity.Migrations;

namespace OpenNos.DAL.EF.Migrations
{
    public partial class Title : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.CharacterTitle",
                    c => new
                    {
                        CharacterTitleId = c.Long(false, true),
                        CharacterId = c.Long(false),
                        TitleVnum = c.Long(false),
                        Stat = c.Byte(false)
                    })
                .PrimaryKey(t => t.CharacterTitleId)
                .ForeignKey("dbo.Character", t => t.CharacterId)
                .Index(t => t.CharacterId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.CharacterTitle", "CharacterId", "dbo.Character");
            DropIndex("dbo.CharacterTitle", new[] {"CharacterId"});
            DropTable("dbo.CharacterTitle");
        }
    }
}