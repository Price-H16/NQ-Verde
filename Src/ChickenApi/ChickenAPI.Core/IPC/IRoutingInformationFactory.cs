﻿// WingsEmu
//
// Developed by NosWings Team

using System.Threading.Tasks;

namespace ChickenAPI.Core.IPC
{
    public interface IRoutingInformationFactory
    {
        #region Methods

        Task<IRoutingInformation> Create(string topic, string responseTopic);

        #endregion
    }
}