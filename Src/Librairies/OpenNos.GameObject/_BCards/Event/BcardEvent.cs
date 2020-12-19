using OpenNos.GameObject._Event;
using OpenNos.GameObject.Battle;

namespace OpenNos.GameObject._BCards.Event
{
    public class BCardEvent : PlayerEvent
    {
        public BattleEntity Target { get; set; }
        
        public BattleEntity Sender { get; set; }

        public BCard Card { get; set; }
    }
}