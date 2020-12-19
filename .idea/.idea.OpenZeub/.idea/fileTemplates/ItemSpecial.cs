using System;
using System.Linq;
using System.Threading.Tasks;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._ItemUsage;
using OpenNos.GameObject._ItemUsage.Event;
using OpenNos.GameObject.Helpers;

namespace Plugins.BasicImplementations.ItemUsage.Handler.Special
{
   public class $NAME : IUseItemRequestHandlerAsync
    {
        public ItemType Type => ItemType.Special;
        public long EffectId => ;

        public async Task HandleAsync(ClientSession session, InventoryUseItemEvent e)
        {

        }
    }
}