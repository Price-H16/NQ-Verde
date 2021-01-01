// WingsEmu
//
// Developed by NosWings Team

namespace ChickenAPI.Core.IPC
{
    public interface IRoutingInformation
    {
        #region Properties

        string IncomingTopic { get; }

        string OutgoingTopic { get; }

        #endregion
    }
}