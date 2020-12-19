namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Chat : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.BotAuthority");
            DropTable("dbo.ChatLog");
        }
        
        public override void Down()
        {
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
            
            CreateTable(
                "dbo.BotAuthority",
                c => new
                    {
                        DiscordId = c.Long(nullable: false),
                        Authority = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DiscordId);
            
        }
    }
}
