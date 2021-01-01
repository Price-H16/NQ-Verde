using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChickenAPI.Events
{
    public abstract class GenericEventHandlerBase<TNotification> : IEventHandler
        where TNotification : IEventNotification
    {
        #region Properties

        public Type Type => typeof(TNotification);

        #endregion

        #region Methods

        public Task Handle(IEventNotification notification, CancellationToken cancellation)
        {
            if (notification is TNotification typedNotification) return Handle(typedNotification, cancellation);

            return Task.CompletedTask;
        }

        protected abstract Task Handle(TNotification e, CancellationToken cancellation);

        #endregion
    }
}