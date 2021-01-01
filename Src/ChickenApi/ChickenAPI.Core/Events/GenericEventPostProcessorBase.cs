// WingsEmu
//
// Developed by NosWings Team

using ChickenAPI.Core.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChickenAPI.Core.Events
{
    public abstract class GenericEventPostProcessorBase<TNotification> : IEventPostProcessor where TNotification : IEventNotification
    {
        #region Members

        protected readonly ILogger Log;

        #endregion

        #region Instantiation

        protected GenericEventPostProcessorBase(ILogger log) => Log = log;

        #endregion

        #region Properties

        public Type Type => typeof(TNotification);

        #endregion

        #region Methods

        public Task Handle(IEventNotification notification, CancellationToken cancellation)
        {
            if (notification is TNotification typedNotification)
            {
                return Handle(typedNotification, cancellation);
            }

            return Task.CompletedTask;
        }

        protected abstract Task Handle(TNotification e, CancellationToken cancellation);

        #endregion
    }
}