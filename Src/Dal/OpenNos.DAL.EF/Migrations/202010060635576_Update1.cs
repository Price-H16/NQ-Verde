namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update1 : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.BotAuthority");
        }
        
        public override void Down()
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
    }
}
