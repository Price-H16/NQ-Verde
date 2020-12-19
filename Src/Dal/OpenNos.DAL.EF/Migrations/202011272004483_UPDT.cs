﻿namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UPDT : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Character", "BattleTowerExp", c => c.Int(nullable: false));
            AddColumn("dbo.Character", "BattleTowerStage", c => c.Byte(nullable: false));
            AddColumn("dbo.Item", "IsWarehouseable", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Item", "IsWarehouseable");
            DropColumn("dbo.Character", "BattleTowerStage");
            DropColumn("dbo.Character", "BattleTowerExp");
        }
    }
}
