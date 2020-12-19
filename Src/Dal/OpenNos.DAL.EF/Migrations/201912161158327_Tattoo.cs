using System.Data.Entity.Migrations;

namespace OpenNos.DAL.EF.Migrations
{
    public partial class Tattoo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CharacterSkill", "IsTattoo", c => c.Boolean(false));
            AddColumn("dbo.CharacterSkill", "TattooLevel", c => c.Byte(false));
        }

        public override void Down()
        {
            DropColumn("dbo.CharacterSkill", "TattooLevel");
            DropColumn("dbo.CharacterSkill", "IsTattoo");
        }
    }
}