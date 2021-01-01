﻿using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._BCards;
using OpenNos.GameObject.Battle;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.BCards
{
    public class BCardHandlerContainer : IBCardEffectHandlerContainer
    {
        #region Members

        private readonly Dictionary<BCardType.CardType, IBCardEffectAsyncHandler> _handlers;

        #endregion

        #region Instantiation

        public BCardHandlerContainer() => _handlers = new Dictionary<BCardType.CardType, IBCardEffectAsyncHandler>();

        #endregion

        #region Methods

        public void Execute(BattleEntity target, BattleEntity sender, BCard bcard)
        {
            ExecuteAsync(target, sender, bcard).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task ExecuteAsync(BattleEntity target, BattleEntity sender, BCard bcard)
        {
            if (target == null)
            {
                return;
            }

            if (!_handlers.TryGetValue((BCardType.CardType)bcard.Type, out var handler))
            {
                Logger.Log.Debug($"[HANDLER_NOT_FOUND] BCARD_ID : {bcard.CardId} : {bcard.Type}");
                return;
            }

            await handler.ExecuteAsync(target, sender, bcard);
        }

        public async Task RegisterAsync(IBCardEffectAsyncHandler handler)
        {
            _handlers.Add(handler.HandledType, handler);
            Logger.Log.Debug($"[BCARD][REGISTER_HANDLER] BCARDTYPE : {handler.HandledType} REGISTERED !");
        }

        public async Task UnregisterAsync(IBCardEffectAsyncHandler handler) => _handlers.Remove(handler.HandledType);

        #endregion
    }
}