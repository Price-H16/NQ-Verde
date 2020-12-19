﻿namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tono1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "BankMoney", c => c.Long(nullable: false));
            DropColumn("dbo.Account", "GoldBank");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Account", "GoldBank", c => c.Long(nullable: false));
            DropColumn("dbo.Account", "BankMoney");
        }
    }
}
