using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._BCards;
using OpenNos.GameObject.Battle;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.BCards.Handler
{
    public class TestHealingBurningAndCasting : IBCardEffectAsyncHandler
    {
        #region Properties

        public BCardType.CardType HandledType { get; } = BCardType.CardType.HealingBurningAndCasting;

        #endregion

        #region Methods

        public async Task ExecuteAsync(BattleEntity target, BattleEntity sender, BCard bcard)
        {
            var senderLevel = sender.Level;
            var firstData = bcard.FirstData;
            var IsLevelDivided = bcard.IsLevelDivided;
            var IsLevelScaled = bcard.IsLevelScaled;
            var session = target;
            var ThirdData = bcard.ThirdData;
            /* if (session.HasBuff(BCardType.CardType.RecoveryAndDamagePercent, 01))
                            {
                                return;
                            }*/

            // WTF ? Why Cryless ?

            var amount = bcard.IsLevelDivided ? senderLevel / (firstData += 1) :
                IsLevelDivided && IsLevelScaled ? senderLevel / (firstData += 1) :
                IsLevelScaled ? senderLevel * (firstData += 1) : firstData;

            void HealingBurningAndCastingAction()
            {
                if (session.Hp < 1 || session.MapInstance == null)
                {
                    return;
                }

                switch (bcard.SubType)
                {
                    case (byte)AdditionalTypes.HealingBurningAndCasting.RestoreHP:

                        if (session.Hp + amount > session.HpMax)
                        {
                            amount = session.HpMax - session.Hp;
                        }

                        if (amount > 0)
                        {
                            if (session.HasBuff(BCardType.CardType.DarkCloneSummon,
                                (byte)AdditionalTypes.DarkCloneSummon.ConvertRecoveryToDamage))
                            {
                                amount = session.GetDamage(amount, sender, true, true);

                                session.MapInstance.Broadcast(session.GenerateDm(amount));
                            }
                            else
                            {
                                session.Hp += amount;

                                session.MapInstance.Broadcast(session.GenerateRc(amount));
                            }
                        }

                        break;

                    case (byte)AdditionalTypes.HealingBurningAndCasting.RestoreMP:

                        if (session.Mp + amount > session.MpMax)
                        {
                            amount = session.MpMax - session.Mp;
                        }

                        session.Mp += amount;

                        break;

                    case (byte)AdditionalTypes.HealingBurningAndCasting.DecreaseHP:

                        session.Hp = session.Hp - amount <= 0 ? 1 : session.Hp - amount;
                        session.MapInstance?.Broadcast(session.GenerateDm(amount));

                        break;

                    case (byte)AdditionalTypes.HealingBurningAndCasting.DecreaseMP:

                        session.Mp = session.Mp - amount <= 0 ? 1 : session.Mp - amount;

                        break;
                }

                session?.Character?.Session?.SendPacket(session.Character?.GenerateStat());
            }

            HealingBurningAndCastingAction();

            var interval = ThirdData > 0 ? ThirdData * 2 : bcard.CastType * 2;

            if (bcard.CardId != null && interval > 0)
            {
                IDisposable bcardDisposable = null;
                bcardDisposable = Observable.Interval(TimeSpan.FromSeconds(interval))
                    .Subscribe(s =>
                    {
                        if (session.BCardDisposables[bcard.BCardId] != bcardDisposable)
                        {
                            bcardDisposable.Dispose();
                            return;
                        }

                        if (session != null)
                        {
                            HealingBurningAndCastingAction();
                        }
                    });
                session.BCardDisposables[bcard.BCardId] = bcardDisposable;
            }
        }

        #endregion
    }
}