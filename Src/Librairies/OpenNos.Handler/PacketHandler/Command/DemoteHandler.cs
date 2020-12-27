using System.Linq;
using ChickenAPI.Enums;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class DemoteHandler : IPacketHandler
    {
        #region Instantiation

        public DemoteHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Demote(DemotePacket demotePacket)
        {
            if (demotePacket != null)
            {
                Session.AddLogsCmd(demotePacket);
                var name = demotePacket.CharacterName;
                try
                {
                    var account = DAOFactory.AccountDAO.LoadById(DAOFactory.CharacterDAO.LoadByName(name).AccountId);
                    if (account?.Authority > AuthorityType.User)
                    {
                        if (Session.Account.Authority >= account?.Authority)
                        {
                            var newAuthority = AuthorityType.User;
                            switch (account.Authority)
                            {
                                case AuthorityType.DEV:
                                    newAuthority = AuthorityType.CM;
                                    break;

                                case AuthorityType.CM:
                                    newAuthority = AuthorityType.TM;
                                    break;

                                case AuthorityType.TM:
                                    newAuthority = AuthorityType.GA;
                                    break;

                                case AuthorityType.GA:
                                    newAuthority = AuthorityType.SGM;
                                    break;

                                case AuthorityType.SGM:
                                    newAuthority = AuthorityType.GM;
                                    break;

                                case AuthorityType.GM:
                                    newAuthority = AuthorityType.Supporter;
                                    break;

                                case AuthorityType.Supporter:
                                    newAuthority = AuthorityType.User;
                                    break;

                                default:
                                    newAuthority = AuthorityType.User;
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
                                if (session.Character.InvisibleGm)
                                {
                                    session.Character.Invisible = false;
                                    session.Character.InvisibleGm = false;
                                    Session.Character.Mates.Where(m => m.IsTeamMember).ToList().ForEach(m =>
                                        Session.CurrentMapInstance?.Broadcast(m.GenerateIn(),
                                            ReceiverType.AllExceptMe));
                                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateIn(),
                                        ReceiverType.AllExceptMe);
                                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateGidx(),
                                        ReceiverType.AllExceptMe);
                                }

                                ServerManager.Instance.ChangeMap(session.Character.CharacterId);
                                DAOFactory.AccountDAO.WriteGeneralLog(session.Account.AccountId, session.IpAddress,
                                    session.Character.CharacterId, GeneralLogType.Demotion,
                                    $"by: {Session.Character.Name}");
                            }
                            else
                            {
                                DAOFactory.AccountDAO.WriteGeneralLog(account.AccountId, "127.0.0.1", null,
                                    GeneralLogType.Demotion, $"by: {Session.Character.Name}");
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
                Session.SendPacket(Session.Character.GenerateSay(DemotePacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}