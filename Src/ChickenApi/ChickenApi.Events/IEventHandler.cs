using System.Threading;
using System.Threading.Tasks;

namespace ChickenAPI.Events
{
    /// <summary>
    ///     Defines a handler for any type of notification
    /// </summary>
    public interface IEventHandler
    {
        Task Handle(IEventNotification notification, CancellationToken cancellation);
    }
}