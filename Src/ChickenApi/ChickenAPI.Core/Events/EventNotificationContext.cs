using OpenNos.Domain;

namespace OpenNos.GameObject._gameEvent
{
    public struct EventNotificationContext
    {
        /// <summary>
        /// Servers from where the event is happening
        /// </summary>
        //public WorldServerDTO Server { get; set; }

        #region Properties

        public NotifiableEventType NotifiableEventType { get; set; }

        #endregion
    }
}