namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DiscordBot : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BotAuthority",
                c => new
                    {
                        DiscordId = c.Long(nullable: false),
                        Authority = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DiscordId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BotAuthority");
        }
    }
}
