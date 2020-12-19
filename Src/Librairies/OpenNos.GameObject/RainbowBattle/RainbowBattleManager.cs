using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace OpenNos.GameObject.RainbowBattle
{
    public class RainbowBattleManager
    {
        #region Methods

        public static void AddFlag(ClientSession ses, RainbowBattleTeam rbb, RainbowNpcType type, int npcId)
        {
            if (rbb == null) return;

            rbb.Score += (byte)type;

            rbb.TotalFlag.Add(new Tuple<int, RainbowNpcType>(npcId, type));

            SendIcoFlagOnMinimap(ses, npcId, (byte)type, (byte)(rbb.TeamEntity == RainbowTeamBattleType.Blue ? 2 : 1));

            var RainbowTeam2 = rbb.SecondTeam;

            if (RainbowTeam2 == null) return;

            if (AlreadyHaveFlag(RainbowTeam2, type, npcId)) RemoveFlag(RainbowTeam2, type, npcId);

            SendFbs(ses.CurrentMapInstance);
            ses.CurrentMapInstance.Broadcast($"msg 0 {rbb.TeamEntity} Team captured the {type} Crystal and scored {(byte)type} points [Score: {rbb.Score} : {rbb.SecondTeam.Score}]");
        }

        public static bool AlreadyHaveFlag(RainbowBattleTeam RainbowBattleTeam, RainbowNpcType type, int NpcId)
        {
            if (RainbowBattleTeam == null) return false;

            var a = RainbowBattleTeam.TotalFlag.FindAll(s => s.Item1 == NpcId && s.Item2 == type).Count();

            return a == 0 ? false : true;
        }

        public static bool AreNotInMap(ClientSession ses)
        {
            if (ses == null) return false;

            if (ses?.CurrentMapInstance?.MapInstanceType != MapInstanceType.RainbowBattleInstance)
            {
                return true;
            }
            return false;
        }

        public static void EndEvent(ClientSession ses, MapInstance map)
        {
            if (ses == null) return;

            map.IsPVP = false;
            var rbb = ServerManager.Instance.RainbowBattleMembers.Find(s => s.Session.Contains(ses));

            if (rbb == null || rbb.SecondTeam == null) return;

            bool teamWinner = (rbb.Score > rbb.SecondTeam.Score ? true : false);

            if (rbb.Score == rbb.SecondTeam.Score)
            {
                SendGift(rbb.Session, 2);
                SendGift(rbb.SecondTeam.Session, 2);
            }
            else
            {
                SendGift(rbb.Session, teamWinner ? (byte)1 : (byte)0);
                SendGift(rbb.SecondTeam.Session, !teamWinner ? (byte)1 : (byte)0);
            }

            ServerManager.Instance.RainbowBattleMembers.Remove(rbb);
            ServerManager.Instance.RainbowBattleMembers.Remove(rbb.SecondTeam);
            //ServerManager.Instance.RainbowBattleMembers = null;
        }

        public static void GenerateScore(RainbowBattleTeam RainbowBattle)
        {
            if (RainbowBattle == null) return;

            var first = GetFlag(RainbowBattle, RainbowNpcType.Small);
            var Second = GetFlag(RainbowBattle, RainbowNpcType.Second);
            var Last = GetFlag(RainbowBattle, RainbowNpcType.Large);

            var total = first + (Second * 2) + (Last * 3);

            RainbowBattle.Score += total;

            foreach (var ses in RainbowBattle.Session)
            {
                SendFbs(ses.CurrentMapInstance);
            }
        }

        public static void GenerateScoreForAll()
        {
            foreach (var rbb in ServerManager.Instance.RainbowBattleMembers)
            {
                GenerateScore(rbb);
            }
        }
        public static void ReceiveFbs(RainbowBattleTeam rbb)
        {
            var output = (rbb.TeamEntity == RainbowTeamBattleType.Red ? rbb.SecondTeam : rbb);

            if (rbb == null) return;


            foreach (var bb in rbb.Session)
            {
                if (AreNotInMap(bb))
                {
                    continue;
                }

                bb.SendPacket(
                    $"fbs " +
                    $"{(byte)rbb.TeamEntity} " +
                    $"{rbb.Session.Count()} " +
                    $"{output.SecondTeam.Score} " +
                    $"{output.Score} " +
                    $"{GetFlag(rbb, RainbowNpcType.Small)} " +
                    $"{GetFlag(rbb, RainbowNpcType.Second)} " +
                    $"{GetFlag(rbb, RainbowNpcType.Large)} " +
                    $"{rbb.TeamEntity}");
            }

            if (rbb.SecondTeam == null)
            {
                return;
            }

            foreach (var bb in rbb.SecondTeam.Session)
            {
                if (AreNotInMap(bb))
                {
                    continue;
                }

                bb.SendPacket(
                    $"fbs " +
                    $"{(byte)rbb.SecondTeam.TeamEntity} " +
                    $"{rbb.SecondTeam.Session.Count()} " +
                    $"{output.SecondTeam.Score} " +
                    $"{output.Score} " +
                    $"{GetFlag(rbb.SecondTeam, RainbowNpcType.Small)} " +
                    $"{GetFlag(rbb.SecondTeam, RainbowNpcType.Second)} " +
                    $"{GetFlag(rbb.SecondTeam, RainbowNpcType.Large)} " +
                    $"{rbb.SecondTeam.TeamEntity}");
            }
        }
        //public static void ReceiveFbs(RainbowBattleTeam rbb)
        //{
        //    if (rbb == null) return;

        //    var output = (rbb.TeamEntity == RainbowTeamBattleType.Red ? rbb.SecondTeam : rbb);

        //    foreach (var bb in rbb.Session)
        //    {
        //        if (bb == null && AreNotInMap(bb)) continue;

        //        bb?.SendPacket(
        //            $"fbs " +
        //            $"{(byte)rbb.TeamEntity} " +
        //            $"{rbb.Session.Count()} " +
        //            $"{output.SecondTeam.Score} " +
        //            $"{output.Score} " +
        //            $"{GetFlag(rbb, RainbowNpcType.Small)} " +
        //            $"{GetFlag(rbb, RainbowNpcType.Second)} " +
        //            $"{GetFlag(rbb, RainbowNpcType.Large)} " +
        //            $"{rbb.TeamEntity}");
        //    }
        //}

        public static void RemoveFlag(RainbowBattleTeam RainbowBattle, RainbowNpcType type, int NpcId)
        {
            if (RainbowBattle == null) return;

            RainbowBattle.TotalFlag.RemoveAll(s => s.Item1 == NpcId && s.Item2 == type);
        }

        public static void SendFbs(MapInstance map)
        {

            if (map == null) return;

            foreach (var ses in map.Sessions)
            {
                if (AreNotInMap(ses)) continue;

                var rbb = ServerManager.Instance.RainbowBattleMembers.Find(s => s.Session.Contains(ses));

                if (rbb == null) continue;

                ReceiveFbs(rbb);
            }
        }

        public static void SendIcoFlagOnMinimap(ClientSession sess, long npcId, byte score, byte team)
        {
            sess.CurrentMapInstance?.Broadcast($"fbt 6 {npcId} {score} {team}");
        }

        private static int GetFlag(RainbowBattleTeam RainbowBattleTeam, RainbowNpcType type)
        {
            if (RainbowBattleTeam == null) return 0;
            return RainbowBattleTeam.TotalFlag.FindAll(s => s.Item2 == type).Count();
        }

        private static void SendGift(IEnumerable<ClientSession> sess, byte winner)
        {
            foreach (var ses in sess)
            {
                if (sess == null) continue;
                if (AreNotInMap(ses)) continue;

                ses.Character.Group?.LeaveGroup(ses);
                ServerManager.Instance.UpdateGroup(ses.Character.CharacterId);
                ses.SendPacket(ses.Character.GenerateRaid(2, true));

                Observable.Timer(TimeSpan.FromSeconds(5)).Subscribe(o =>
                {
                    ServerManager.Instance.ChangeMap(ses.Character.CharacterId, ses.Character.MapId, ses.Character.MapX, ses.Character.MapY);
                });

                // Loose
                if (winner == 0)
                {
                    ses.Character.GiftAdd(2361, 1);
                    ses.Character.RBBLose++;
                    ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg("You lost The Rainbow Battle!", 1));
                }

                // Win
                if (winner == 1)
                {
                    ses.Character.GiftAdd(5746, 1);
                    ses.Character.GiftAdd(2361, 3);
                    ses.Character.RBBWin++;
                    if (ses.Character.HeroLevel < 55)
                    {
                        ses.Character.HeroLevel++;
                    }
                    ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg("You won The Rainbow Battle!", 1));
                }

                // Equal
                if (winner == 2)
                {
                    ses.Character.GiftAdd(2361, 2);
                    ServerManager.Instance.Broadcast(UserInterfaceHelper.GenerateMsg("The Rainbow Battle ended in a draw!", 1));
                }
            }
        }

        #endregion
    }
}