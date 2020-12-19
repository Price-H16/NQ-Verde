﻿using OpenNos.Domain;
using System.Threading.Tasks;

namespace ChickenAPI.Core.Events
{
    public interface IPlayerNotifier
    {
        /// <summary>
        /// Notify and format the expected string
        /// </summary>
        /// <param name="notifiable"></param>
        /// <returns></returns>
        Task NotifyAllAsync(NotifiableEventType notifiable);

        /// <summary>
        /// Notify and format the expected string with the given objects
        /// </summary>
        /// <param name="notifiable"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        Task NotifyAllAsync(NotifiableEventType notifiable, params object[] objs);
    }
}