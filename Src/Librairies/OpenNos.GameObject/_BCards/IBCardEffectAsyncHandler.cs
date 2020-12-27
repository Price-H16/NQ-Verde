using System.Threading.Tasks;
using ChickenAPI.Enums.Game.BCard;
using OpenNos.Domain;
using OpenNos.GameObject.Battle;

namespace OpenNos.GameObject._BCards
{
    public interface IBCardEffectAsyncHandler
    {
        BCardType HandledType { get; }

        Task ExecuteAsync(BattleEntity target, BattleEntity sender, BCard bcard);
    }
}