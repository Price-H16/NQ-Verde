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

    public static class GrasslinRad
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
            GrasslinBoss raidThread = new GrasslinBoss();
            Observable.Timer(TimeSpan.FromMinutes(0)).Subscribe(X => raidThread.Run());
        }

        #endregion
    }

    public class GrasslinBoss
    {
        #region Methods

        public void Run()
        {
            CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
            {
                DestinationCharacterId = null,
                SourceCharacterId = 0,
                SourceWorldId = ServerManager.Instance.WorldId,
                Message = "The Grasslin has appeared, who will plot in Nosville? , Let’s go after him!",
                Type = MessageType.Shout
            });

            GrasslinRad.RemainingTime = 3600;
            const int interval = 1;

            GrasslinRad.WorldMapinstance = ServerManager.GenerateMapInstance(2552, MapInstanceType.GrasslinBossInstance, new InstanceBag());
            GrasslinRad.UnknownLandMapInstance = ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(1));



            GrasslinRad.WorldMapinstance.CreatePortal(new Portal
            {
                SourceMapInstanceId = GrasslinRad.WorldMapinstance.MapInstanceId,
                SourceX = 17,
                SourceY = 38,
                DestinationMapId = 0,
                DestinationX = 79,
                DestinationY = 125,
                DestinationMapInstanceId = GrasslinRad.UnknownLandMapInstance.MapInstanceId,
                Type = 6,
            });

            GrasslinRad.UnknownLandMapInstance.CreatePortal(new Portal
            {
                SourceMapId = 1,
                SourceX = 79,
                SourceY = 125,
                DestinationMapId = 0,
                DestinationX = 17,
                DestinationY = 38,
                DestinationMapInstanceId = GrasslinRad.WorldMapinstance.MapInstanceId,
                Type = 6,
            });

            List<EventContainer> onDeathEvents = new List<EventContainer>
            {
               new EventContainer(GrasslinRad.WorldMapinstance, EventActionType.SCRIPTEND, (byte)1)
            };

            #region Grasslin

            MapMonster GrasslinMonster = new MapMonster
            {
                MonsterVNum = 3128,
                MapY = 18,
                MapX = 21,
                MapId = GrasslinRad.WorldMapinstance.Map.MapId,
                Position = 2,
                IsMoving = true,
                MapMonsterId = GrasslinRad.WorldMapinstance.GetNextMonsterId(),
                ShouldRespawn = false
            };
            GrasslinMonster.Initialize(GrasslinRad.WorldMapinstance);
            GrasslinRad.WorldMapinstance.AddMonster(GrasslinMonster);
            MapMonster Grasslin = GrasslinRad.WorldMapinstance.Monsters.Find(s => s.Monster.NpcMonsterVNum == 3128);
            if (Grasslin != null)
            {
                Grasslin.BattleEntity.OnDeathEvents = onDeathEvents;
                Grasslin.IsBoss = true;
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

                foreach (ClientSession sess in GrasslinRad.WorldMapinstance.Sessions.ToList())
                {
                    ServerManager.Instance.ChangeMapInstance(sess.Character.CharacterId, GrasslinRad.UnknownLandMapInstance.MapInstanceId, sess.Character.MapX, sess.Character.MapY);
                    Thread.Sleep(100);
                }
                GrasslinRad.IsRunning = false;
                GrasslinRad.AngelDamage = 0;
                GrasslinRad.DemonDamage = 0;
                ServerManager.Instance.StartedEvents.Remove(EventType.GRASSLINBOSS);
            }
            catch (Exception ex)
            {

            }

        }

        private void LockRaid()
        {
            foreach (Portal p in GrasslinRad.UnknownLandMapInstance.Portals.Where(s => s.DestinationMapInstanceId == GrasslinRad.WorldMapinstance.MapInstanceId).ToList())
            {
                GrasslinRad.UnknownLandMapInstance.Portals.Remove(p);
                GrasslinRad.UnknownLandMapInstance.Broadcast(p.GenerateGp());
            }
            ServerManager.Shout("Grasslin Boss actually is closed.", true);
            GrasslinRad.IsLocked = true;
        }

        #endregion
    }
}