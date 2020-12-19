//using System.Linq;
//using System.Threading.Tasks;
//using OpenNos.Core;
//using OpenNos.Domain;
//using OpenNos.GameObject;
//using OpenNos.GameObject._NpcDialog;
//using OpenNos.GameObject._NpcDialog.Event;
//using OpenNos.GameObject.Networking;

//namespace Plugins.BasicImplementations.NpcDialog.Handler
//{
//    public class RBB : INpcDialogAsyncHandler
//    {
//        public long HandledId => 4000;

//        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
//        {
//            var npc = packet.Npc;
//            if (npc != null)
//            {
//                if (ServerManager.Instance.CanRegisterRainbowBattle)
//                {
//                    if (Session.Character.Group != null)
//                    {
//                        if (Session.Character.Group.Raid == null)
//                        {
//                            if (Session.Character.Group.GroupType == GroupType.BigTeam)
//                            {
//                                if (Session.Character.Group.IsLeader(Session))
//                                {
//                                    if (ServerManager.Instance.RainbowBattleMembersRegistered.Count() > 0)
//                                    {
//                                        if (ServerManager.Instance.RainbowBattleMembersRegistered?.Where(s => s.Session == Session).Count() > 0)
//                                        {
//                                            return;
//                                        }
//                                    }
//                                    Parallel.ForEach(
//                                    ServerManager.Instance.RainbowBattleMembers.Where(s => s.GroupId == Session.Character.Group.GroupId), s =>
//                                    {
//                                     ServerManager.Instance.RainbowBattleMembersRegistered.Add(s);
//                                    }
//                                    );
//                                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("BA_REGISTERED"), 10));

//                                }
//                            }
//                        }
//                    }
//                }
//                else
//                {
//                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("BA_NOT_OPEN"), 10));
//                }
//            }
        
//        }
//    }
//}