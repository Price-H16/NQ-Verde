namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Update : DbMigration
    {
        #region Methods

        public override void Down()
        {
            DropColumn("dbo.CharacterSkill", "IsPartnerSkill");
        }

        public override void Up()
        {
            AddColumn("dbo.CharacterSkill", "IsPartnerSkill", c => c.Boolean(nullable: false));
        }

        #endregion
    }
}