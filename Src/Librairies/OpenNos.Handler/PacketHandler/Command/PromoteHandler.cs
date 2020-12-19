using System.Linq;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class PromoteHandler : IPacketHandler
    {
        #region Instantiation

        public PromoteHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Promote(PromotePacket promotePacket)
        {
            if (promotePacket != null)
            {
                Session.AddLogsCmd(promotePacket);
                var name = promotePacket.CharacterName;
                try
                {
                    var account = DAOFactory.AccountDAO.LoadById(DAOFactory.CharacterDAO.LoadByName(name).AccountId);
                    if (account?.Authority >= AuthorityType.User)
                    {
                        if (account.Authority < Session.Account.Authority)
                        {
                            var newAuthority = AuthorityType.User;
                            switch (account.Authority)
                            {
                                case AuthorityType.User:
                                    newAuthority = AuthorityType.Supporter;
                                    break;

                                case AuthorityType.Supporter:
                                    newAuthority = AuthorityType.GM;
                                    break;

                                case AuthorityType.GM:
                                    newAuthority = AuthorityType.SGM;
                                    break;

                                case AuthorityType.SGM:
                                    newAuthority = AuthorityType.GA;
                                    break;

                                case AuthorityType.GA:
                                    newAuthority = AuthorityType.TM;
                                    break;

                                case AuthorityType.TM:
                                    newAuthority = AuthorityType.CM;
                                    break;

                                case AuthorityType.CM:
                                    newAuthority = AuthorityType.DEV;
                                    break;

                                default:
                                    newAuthority = account.Authority;
                                    break;
                            }

                            account.Authority = newAuthority;
                            DAOFactory.AccountDAO.InsertOrUpdate(ref account);
                            var session =
                                ServerManager.Instance.Sessions.FirstOrDefault(s => s.Character?.Name == name);

                            if (session != null)
                            {
                                session.Account.Authority = newAuthority;
                                session.Character.Authority = newAuthority;
                                ServerManager.Instance.ChangeMap(session.Character.CharacterId);
                                DAOFactory.AccountDAO.WriteGeneralLog(session.Account.AccountId, session.IpAddress,
                                    session.Character.CharacterId, GeneralLogType.Promotion,
                                    $"by: {Session.Character.Name}");
                            }
                            else
                            {
                                DAOFactory.AccountDAO.WriteGeneralLog(account.AccountId, "127.0.0.1", null,
                                    GeneralLogType.Promotion, $"by: {Session.Character.Name}");
                            }

                            Session.SendPacket(
                                Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
                        }
                        else
                        {
                            Session.SendPacket(
                                Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_DO_THAT"), 10));
                        }
                    }
                    else
                    {
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_FOUND"), 10));
                    }
                }
                catch
                {
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_FOUND"), 10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(PromotePacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}