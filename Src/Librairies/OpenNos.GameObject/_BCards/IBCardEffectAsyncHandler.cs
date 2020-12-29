using System.Threading.Tasks;
using OpenNos.Domain;
using OpenNos.GameObject.Battle;

namespace OpenNos.GameObject._BCards
{
    public interface IBCardEffectAsyncHandler
    {
        BCardType.CardType HandledType { get; }

        Task ExecuteAsync(BattleEntity target, BattleEntity sender, BCard bcard);
    }
}