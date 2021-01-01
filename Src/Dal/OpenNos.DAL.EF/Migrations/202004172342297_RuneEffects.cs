namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class RuneEffects : DbMigration
    {
        #region Methods

        public override void Down()
        {
            DropTable("dbo.RuneEffect");
        }

        public override void Up()
        {
            CreateTable(
                "dbo.RuneEffect",
                c => new
                {
                    RuneEffectId = c.Int(nullable: false, identity: true),
                    EquipmentSerialId = c.Guid(nullable: false),
                    Type = c.Byte(nullable: false),
                    SubType = c.Byte(nullable: false),
                    FirstData = c.Int(nullable: false),
                    SecondData = c.Int(nullable: false),
                    ThirdData = c.Int(nullable: false),
                    IsPower = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.RuneEffectId);
        }

        #endregion
    }
}