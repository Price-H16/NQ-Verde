using System.Threading.Tasks;
using ChickenAPI.Events;
using OpenNos.GameObject.Battle;

namespace OpenNos.GameObject._Event
{
    public class PlayerEvent : IEventNotification
    {
        public EventEntity Sender { get; set; }
    }
}