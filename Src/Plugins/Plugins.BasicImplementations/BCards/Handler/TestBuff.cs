using System.Threading.Tasks;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._BCards;
using OpenNos.GameObject.Battle;

namespace Plugins.BasicImplementations.BCards.Handler
{
    public class TestBuff : IBCardEffectAsyncHandler
    {
        public BCardType.CardType HandledType { get; } = BCardType.CardType.Buff;

        public async Task ExecuteAsync(BattleEntity target, BattleEntity sender, BCard bcard)
        {
            var cardId = (short) bcard.SecondData;

            // Memorial should only be applied on 1st Mass Teleport activation

            var buff = new Buff(cardId, sender.Level)
            {
                SkillVNum = bcard.SkillVNum
            };
            
            target.AddBuff(buff, sender);
        }
    }
}