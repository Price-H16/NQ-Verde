using System;
using System.Linq;
using ChickenAPI.Enums;
using NosTale.Configuration;
using NosTale.Configuration.Utilities;
using NosTale.Extension.GameExtension.Packet;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.Master.Library.Client;

namespace OpenNos.Handler.BasicPacket.Login
{
    public class LoginPacketHandler : IPacketHandler
    {
        #region Members

        private readonly ClientSession _session;

        #endregion

        #region Instantiation
        
        private string GetFailPacket(AuthResponse response) => $"failc {(byte)response}";

        public LoginPacketHandler(ClientSession session)
        {
            _session = session;
        }

        #endregion

        #region Methods

        private string BuildServersPacket(string username, byte regionType, int sessionId, bool ignoreUserName)
        {
            string channelpacket = CommunicationServiceClient.Instance.RetrieveRegisteredWorldServers(username, regionType, sessionId, ignoreUserName);

            if (channelpacket == null || !channelpacket.Contains(':'))
            {
                Logger.Debug(
                    "Could not retrieve Worldserver groups. Please make sure they've already been registered.");
                _session.SendPacket(GetFailPacket(AuthResponse.Maintenance));
            }

            return channelpacket;
        }



        public void VerifyLogin(LoginPacket loginPacket)
        {
            if (loginPacket == null || loginPacket.Name == null || loginPacket.Password == null)
            {
                return;
            }

            var a = DependencyContainer.Instance.GetInstance<JsonGameConfiguration>().Server;

            UserDTO user = new UserDTO
            {
                Name = loginPacket.Name,
                Password = a.UseOldCrypto == true
                    ? CryptographyBase.Sha512(LoginCryptography.GetPassword(loginPacket.Password)).ToUpper()
                    : loginPacket.Password
            };
            if (user == null || user.Name == null || user.Password == null)
            {
                return;
            }
            AccountDTO loadedAccount = DAOFactory.AccountDAO.LoadByName(user.Name);
            if (loadedAccount != null && loadedAccount.Name != user.Name)
            {
                _session.SendPacket(GetFailPacket(AuthResponse.WrongCaps));
                return;
            }
            if (loadedAccount?.Password.ToUpper().Equals(user.Password) == true)
            {
                string ipAddress = _session.IpAddress;
                DAOFactory.AccountDAO.WriteGeneralLog(loadedAccount.AccountId, ipAddress, null,
                    GeneralLogType.Connection, "LoginServer");

                if (DAOFactory.PenaltyLogDAO.LoadByIp(ipAddress).Count() > 0)
                {
                    _session.SendPacket(GetFailPacket(AuthResponse.CantConnect));
                    return;
                }

                //check if the account is connected
                if (!CommunicationServiceClient.Instance.IsAccountConnected(loadedAccount.AccountId))
                {
                    AuthorityType type = loadedAccount.Authority;
                    PenaltyLogDTO penalty = DAOFactory.PenaltyLogDAO.LoadByAccount(loadedAccount.AccountId)
                        .FirstOrDefault(s => s.DateEnd > DateTime.Now && s.Penalty == PenaltyType.Banned);
                    if (penalty != null)
                    {
                        _session.SendPacket(GetFailPacket(AuthResponse.Banned));
                    }
                    else
                    {
                        switch (type)
                        {

                            case AuthorityType.Banned:
                                {
                                    _session.SendPacket(GetFailPacket(AuthResponse.Banned));
                                }
                                break;

                            default:
                                {
                                    if (loadedAccount.Authority < AuthorityType.Supporter)
                                    {
                                        MaintenanceLogDTO maintenanceLog = DAOFactory.MaintenanceLogDAO.LoadFirst();
                                        if (maintenanceLog != null && maintenanceLog.DateStart < DateTime.Now)
                                        {
                                            _session.SendPacket(GetFailPacket(AuthResponse.Maintenance));
                                            return;
                                        }
                                    }

                                    int newSessionId = SessionFactory.Instance.GenerateSessionId();
                                    Logger.Debug(string.Format(Language.Instance.GetMessageFromKey("CONNECTION"), user.Name,
                                        newSessionId));
                                    try
                                    {
                                        ipAddress = ipAddress.Substring(6, ipAddress.LastIndexOf(':') - 6);
                                        CommunicationServiceClient.Instance.RegisterAccountLogin(loadedAccount.AccountId,
                                            newSessionId, ipAddress);
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error("General Error SessionId: " + newSessionId, ex);
                                    }

                                    string[] clientData = loginPacket.ClientData.Split('.');


                                    // crypto check
                                    byte regionType = 0;
                                    if (clientData.Length < 2)
                                    {
                                        clientData = loginPacket.ClientDataOld.Split('.');
                                    }
                                    else
                                    {
                                        regionType = byte.Parse(clientData[0].Split('\v')[0]);
                                    }

                                    if (clientData.Length < 2)
                                    {
                                        clientData = loginPacket.ClientDataOld.Split('.');
                                    }

                                    bool ignoreUserName = short.TryParse(clientData[3], out short clientVersion)
                                                          && (clientVersion < 3075
                                                           || a.UseOldCrypto == true);
                                    _session.SendPacket(BuildServersPacket(user.Name, regionType, newSessionId, ignoreUserName));
                                }
                                break;
                        }
                    }
                }
                else
                {
                    _session.SendPacket(GetFailPacket(AuthResponse.AlreadyConnected));
                }
            }
            else
            {
                _session.SendPacket(GetFailPacket(AuthResponse.AccountOrPasswordWrong));
            }
        }

        #endregion
    }
}
