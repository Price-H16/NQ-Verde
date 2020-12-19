using System.Threading.Tasks;
using OpenNos.GameObject.Battle;

namespace OpenNos.GameObject._BCards
{
    public interface IBCardEffectHandlerContainer
    {
        Task RegisterAsync(IBCardEffectAsyncHandler handler);

        Task UnregisterAsync(IBCardEffectAsyncHandler handler);

        void Execute(BattleEntity target, BattleEntity sender, BCard bcard);

        Task ExecuteAsync(BattleEntity target, BattleEntity sender, BCard bcard);
    }
}