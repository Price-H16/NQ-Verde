using System.Data.Entity.Migrations;

namespace OpenNos.DAL.EF.Migrations
{
    public partial class Act7_Rune_Item : DbMigration
    {
        #region Methods

        public override void Down()
        {
            AlterColumn("dbo.ShellEffect", "EffectLevel", c => c.Byte(false));
            DropColumn("dbo.ShellEffect", "Upgrade");
            DropColumn("dbo.ShellEffect", "Type");
            DropColumn("dbo.ShellEffect", "IsRune");
            DropColumn("dbo.ItemInstance", "RuneAmount");
            DropColumn("dbo.ItemInstance", "IsBreaked");
        }

        public override void Up()
        {
            AddColumn("dbo.ItemInstance", "IsBreaked", c => c.Boolean(false));
            AddColumn("dbo.ItemInstance", "RuneAmount", c => c.Byte(false));
            AddColumn("dbo.ShellEffect", "IsRune", c => c.Boolean(false));
            AddColumn("dbo.ShellEffect", "Type", c => c.Short(false));
            AddColumn("dbo.ShellEffect", "Upgrade", c => c.Short(false));
            AlterColumn("dbo.ShellEffect", "EffectLevel", c => c.Byte());
        }

        #endregion
    }
}