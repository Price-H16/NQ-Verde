namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FamilyQuests : DbMigration
    {
        public override void Up()
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FamilyQuests");
        }
    }
}
