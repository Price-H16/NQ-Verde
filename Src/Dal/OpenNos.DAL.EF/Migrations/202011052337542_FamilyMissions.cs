﻿namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FamilyMissions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FamilySkillMission",
                c => new
                    {
                        FamilySkillMissionId = c.Long(nullable: false, identity: true),
                        FamilyId = c.Long(nullable: false),
                        ItemVNum = c.Short(nullable: false),
                        CurrentValue = c.Short(nullable: false),
                        TotalValue = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.FamilySkillMissionId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FamilySkillMission");
        }
    }
}
