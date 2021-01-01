using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._Guri;
using OpenNos.GameObject._Guri.Event;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.Guri.Handler
{
    public class G300 : IGuriHandler
    {
        #region Properties

        public long GuriEffectId => 300;

        #endregion

        #region Methods

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            if (e.Type == 300)
            {
                if (e.Argument == 8023 && short.TryParse(e.User.ToString(), out var slot))
                {
                    var box = Session.Character.Inventory.LoadBySlotAndType(slot, InventoryType.Equipment);
                    if (box != null)
                    {
                        box.Item.Use(Session, ref box, 1, new[] { e.Data.ToString() });
                    }
                }
            }
        }

        #endregion
    }
}