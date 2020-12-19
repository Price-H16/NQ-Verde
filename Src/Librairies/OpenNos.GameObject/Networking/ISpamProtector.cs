namespace OpenNos.GameObject.Networking
{
    public interface ISpamProtector
    {
        bool CanConnect(string ipAddress);
    }
}
