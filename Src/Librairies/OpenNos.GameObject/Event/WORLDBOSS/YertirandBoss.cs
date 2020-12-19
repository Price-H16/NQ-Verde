using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenNos.GameObject.Event.WORLDBOSS
{

    public static class YertirandRad
    {
        #region Properties

        public static int AngelDamage { get; set; }

        public static MapInstance WorldMapinstance { get; set; }

        public static int DemonDamage { get; set; }

        public static bool IsLocked { get; set; }

        public static bool IsRunning { get; set; }

        public static int RemainingTime { get; set; }

        public static MapInstance UnknownLandMapInstance { get; set; }

        #endregion

        #region Methods

        public static void Run()
        {
            YertirandBoss raidThread = new YertirandBoss();
            Observable.Timer(TimeSpan.FromMinutes(0)).Subscribe(X => raidThread.Run());
        }

        #endregion
    }

    public class YertirandBoss
    {
        #region Methods

        public void Run()
        {
            CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
            {
                DestinationCharacterId = null,
                SourceCharacterId = 0,
                SourceWorldId = ServerManager.Instance.WorldId,
                Message = "The Darkness have summoned [Yertirand] a Fearsome Sorcerer, time to protect us again!",
                Type = MessageType.Shout
            });

            YertirandRad.RemainingTime = 3600;
            const int interval = 1;

            YertirandRad.WorldMapinstance = ServerManager.GenerateMapInstance(2622, MapInstanceType.YertirandBossInstance, new InstanceBag());
            YertirandRad.UnknownLandMapInstance = ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(1));



            YertirandRad.WorldMapinstance.CreatePortal(new Portal
            {
                SourceMapInstanceId = YertirandRad.WorldMapinstance.MapInstanceId,
                SourceX = 120,
                SourceY = 126,
                DestinationMapId = 0,
                DestinationX = 79,
                DestinationY = 125,
                DestinationMapInstanceId = YertirandRad.UnknownLandMapInstance.MapInstanceId,
                Type = 6,
            });

            YertirandRad.UnknownLandMapInstance.CreatePortal(new Portal
            {
                SourceMapId = 1,
                SourceX = 79,
                SourceY = 125,
                DestinationMapId = 0,
                DestinationX = 120,
                DestinationY = 126,
                DestinationMapInstanceId = YertirandRad.WorldMapinstance.MapInstanceId,
                Type = 6,
            });

            List<EventContainer> onDeathEvents = new List<EventContainer>
            {
               new EventContainer(YertirandRad.WorldMapinstance, EventActionType.SCRIPTEND, (byte)1)
            };

            #region Yertirand

            MapMonster YertirandMonster = new MapMonster
            {
                MonsterVNum = 3126,
                MapY = 106,
                MapX = 120,
                MapId = YertirandRad.WorldMapinstance.Map.MapId,
                Position = 2,
                IsMoving = true,
                MapMonsterId = YertirandRad.WorldMapinstance.GetNextMonsterId(),
                ShouldRespawn = false
            };
            YertirandMonster.Initialize(YertirandRad.WorldMapinstance);
            YertirandRad.WorldMapinstance.AddMonster(YertirandMonster);
            MapMonster Yertirand = YertirandRad.WorldMapinstance.Monsters.Find(s => s.Monster.NpcMonsterVNum == 3126);
            if (Yertirand != null)
            {
                Yertirand.BattleEntity.OnDeathEvents = onDeathEvents;
                Yertirand.IsBoss = true;
            }
            #endregion
            try
            {
                Observable.Timer(TimeSpan.FromMinutes(15)).Subscribe(X => LockRaid());
                Observable.Timer(TimeSpan.FromMinutes(60)).Subscribe(X => EndRaid());
            }
            catch (Exception ex)
            {

            }



        }

        public void EndRaid()
        {
            try
            {
                ServerManager.Shout(Language.Instance.GetMessageFromKey("WORDLBOSS_END"), true);

                foreach (ClientSession sess in YertirandRad.WorldMapinstance.Sessions.ToList())
                {
                    ServerManager.Instance.ChangeMapInstance(sess.Character.CharacterId, YertirandRad.UnknownLandMapInstance.MapInstanceId, sess.Character.MapX, sess.Character.MapY);
                    Thread.Sleep(100);
                }
                YertirandRad.IsRunning = false;
                YertirandRad.AngelDamage = 0;
                YertirandRad.DemonDamage = 0;
                ServerManager.Instance.StartedEvents.Remove(EventType.YERTIRANDBOSS);
            }
            catch (Exception ex)
            {

            }

        }

        private void LockRaid()
        {
            foreach (Portal p in YertirandRad.UnknownLandMapInstance.Portals.Where(s => s.DestinationMapInstanceId == YertirandRad.WorldMapinstance.MapInstanceId).ToList())
            {
                YertirandRad.UnknownLandMapInstance.Portals.Remove(p);
                YertirandRad.UnknownLandMapInstance.Broadcast(p.GenerateGp());
            }
            ServerManager.Shout("Yertirand Boss actually is closed.", true);
            YertirandRad.IsLocked = true;
        }

        #endregion
    }
}