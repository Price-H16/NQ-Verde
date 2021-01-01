namespace NosQuest.Network
{
    public interface ISpamProtector
    {
        #region Methods

        bool CanConnect(string ipAddress);

        #endregion
    }
}