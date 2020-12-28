//using System.Linq;
//using OpenNos.Core;
//using OpenNos.Domain;
//using OpenNos.GameObject;
//using OpenNos.Master.Library.Client;

//namespace NosTale.Extension.GameExtension.Packet
//{
//    public static class LoginExt
//    {
//        #region Methods

//        public static string BuildServersPacket(this ClientSession session, string username, int sessionId, bool ignoreUserName)
//        {
//            var channelpacket = CommunicationServiceClient.Instance.RetrieveRegisteredWorldServers(username, sessionId, ignoreUserName);
//            if (channelpacket == null || !channelpacket.Contains(':'))
//            {
//                Logger.Debug("Could not retrieve Worldserver groups. Please make sure they've already been registered.");
//                session.SendPacket($"failc {(byte)LoginFailType.Maintenance}");
//            }

//            return channelpacket;
//        }

//        #endregion
//    }
//}