using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System.Linq;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D4 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 4;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            var mate = Session.Character.Mates.Find(s => s.MateTransportId == packet.NpcId);
            switch (packet.Type)
            {
                case 2:
                    if (mate != null)
                    {
                        if (Session.Character.Miniland == Session.Character.MapInstance)
                        {
                            if (Session.Character.Level >= mate.Level)
                            {
                                var teammate = Session.Character.Mates.Where(s => s.IsTeamMember).FirstOrDefault(s => s.MateType == mate.MateType);
                                if (teammate != null)
                                {
                                    teammate.RemoveTeamMember();
                                }
                                mate.AddTeamMember();
                            }
                            else
                            {
                                Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("PET_HIGHER_LEVEL"), 0));
                            }
                        }
                    }
                    break;

                case 3:
                    if (mate != null && Session.Character.Miniland == Session.Character.MapInstance)
                    {
                        mate.RemoveTeamMember();
                    }
                    break;

                case 4:
                    if (mate != null)
                    {
                        if (Session.Character.Miniland == Session.Character.MapInstance)
                        {
                            mate.RemoveTeamMember(false);
                            mate.MapX = mate.PositionX;
                            mate.MapY = mate.PositionY;
                        }
                        else
                        {
                            Session.SendPacket($"qna #n_run^4^5^3^{mate.MateTransportId} {Language.Instance.GetMessageFromKey("ASK_KICK_PET")}");
                        }
                        break;
                    }
                    break;

                case 5:
                    if (mate != null)
                    {
                        Session.SendPacket(UserInterfaceHelper.GenerateDelay(3000, 10, $"#n_run^4^6^3^{mate.MateTransportId}"));
                    }
                    break;

                case 6:
                    if (mate != null && Session.Character.Miniland != Session.Character.MapInstance)
                    {
                        mate.BackToMiniland();
                    }
                    break;

                case 7:
                    if (Session.Character.MapInstance == null || Session.Character.MapInstance.MapInstanceType == MapInstanceType.ArenaInstance || Session.Character.MapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance)
                    {
                        Session.SendPacket(Session.Character.GenerateSay("Yeah nice try, but you can't.", 11));
                        return;
                    }

                    if (mate != null)
                    {
                        if (Session.Character.Mates.Any(s => s.MateType == mate.MateType && s.IsTeamMember))
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("ALREADY_PET_IN_TEAM"), 11));
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("ALREADY_PET_IN_TEAM"), 0));
                        }
                        else
                        {
                            mate.RemoveTeamMember();
                            Session.SendPacket(UserInterfaceHelper.GenerateDelay(3000, 10, $"#n_run^4^9^3^{mate.MateTransportId}"));
                        }
                    }
                    break;

                case 9:
                    if (mate != null && mate.IsSummonable && Session.Character.MapInstance.MapInstanceType != MapInstanceType.TalentArenaMapInstance)
                    {
                        if (Session.Character.Level >= mate.Level)
                        {
                            mate.PositionX = (short)(Session.Character.PositionX + (mate.MateType == MateType.Partner ? -1 : 1));
                            mate.PositionY = (short)(Session.Character.PositionY + 1);
                            mate.AddTeamMember();
                            foreach (var s in Session.CurrentMapInstance.Sessions.Where(s => s.Character != null))
                            {
                                if (ServerManager.Instance.ChannelId != 51 || Session.Character.Faction == s.Character.Faction)
                                {
                                    s.SendPacket(mate.GenerateIn(false, ServerManager.Instance.ChannelId == 51));
                                }
                                else
                                {
                                    s.SendPacket(mate.GenerateIn(true, ServerManager.Instance.ChannelId == 51, s.Account.Authority));
                                }
                            }
                        }
                        else
                        {
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("PET_HIGHER_LEVEL"), 0));
                        }
                    }
                    break;
            }
            Session.SendPacket(Session.Character.GeneratePinit());
            Session.SendPackets(Session.Character.GeneratePst());
        }

        #endregion
    }
}