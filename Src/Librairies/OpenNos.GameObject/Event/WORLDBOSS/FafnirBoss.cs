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

    public static class FafnirRad
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
            FafnirBoss raidThread = new FafnirBoss();
            Observable.Timer(TimeSpan.FromMinutes(0)).Subscribe(X => raidThread.Run());
        }

        #endregion
    }

    public class FafnirBoss
    {
        #region Methods

        public void Run()
        {
            CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
            {
                DestinationCharacterId = null,
                SourceCharacterId = 0,
                SourceWorldId = ServerManager.Instance.WorldId,
                Message = "The Dark Dragon [Fafnir] has stormed into Nosville, let’s protect this beautiful city together!",
                Type = MessageType.Shout
            });

            FafnirRad.RemainingTime = 3600;
            const int interval = 1;

            FafnirRad.WorldMapinstance = ServerManager.GenerateMapInstance(2610, MapInstanceType.FafnirBossInstance, new InstanceBag());
            FafnirRad.UnknownLandMapInstance = ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(1));



            FafnirRad.WorldMapinstance.CreatePortal(new Portal
            {
                SourceMapInstanceId = FafnirRad.WorldMapinstance.MapInstanceId,
                SourceX = 103,
                SourceY = 82,
                DestinationMapId = 0,
                DestinationX = 79,
                DestinationY = 125,
                DestinationMapInstanceId = FafnirRad.UnknownLandMapInstance.MapInstanceId,
                Type = 6
            });

            FafnirRad.UnknownLandMapInstance.CreatePortal(new Portal
            {
                SourceMapId = 1,
                SourceX = 79,
                SourceY = 125,
                DestinationMapId = 0,
                DestinationX = 103,
                DestinationY = 82,
                DestinationMapInstanceId = FafnirRad.WorldMapinstance.MapInstanceId,
                Type = 6
            });

            List<EventContainer> onDeathEvents = new List<EventContainer>
            {
               new EventContainer(FafnirRad.WorldMapinstance, EventActionType.SCRIPTEND, (byte)1)
            };

            #region Fafnir

            MapMonster FafnirMonster = new MapMonster
            {
                MonsterVNum = 3127,
                MapY = 71,
                MapX = 87,
                MapId = FafnirRad.WorldMapinstance.Map.MapId,
                Position = 2,
                IsMoving = true,
                MapMonsterId = FafnirRad.WorldMapinstance.GetNextMonsterId(),
                ShouldRespawn = false
            };
            FafnirMonster.Initialize(FafnirRad.WorldMapinstance);
            FafnirRad.WorldMapinstance.AddMonster(FafnirMonster);
            MapMonster Fafnir = FafnirRad.WorldMapinstance.Monsters.Find(s => s.Monster.NpcMonsterVNum == 3127);
            if (Fafnir != null)
            {
                Fafnir.BattleEntity.OnDeathEvents = onDeathEvents;
                Fafnir.IsBoss = true;
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

                foreach (ClientSession sess in FafnirRad.WorldMapinstance.Sessions.ToList())
                {
                    ServerManager.Instance.ChangeMapInstance(sess.Character.CharacterId, FafnirRad.UnknownLandMapInstance.MapInstanceId, sess.Character.MapX, sess.Character.MapY);
                    Thread.Sleep(100);
                }
                FafnirRad.IsRunning = false;
                FafnirRad.AngelDamage = 0;
                FafnirRad.DemonDamage = 0;
                ServerManager.Instance.StartedEvents.Remove(EventType.FAFNIRBOSS);
            }
            catch (Exception ex)
            {

            }

        }

        private void LockRaid()
        {
            foreach (Portal p in FafnirRad.UnknownLandMapInstance.Portals.Where(s => s.DestinationMapInstanceId == FafnirRad.WorldMapinstance.MapInstanceId).ToList())
            {
                FafnirRad.UnknownLandMapInstance.Portals.Remove(p);
                FafnirRad.UnknownLandMapInstance.Broadcast(p.GenerateGp());
            }
            ServerManager.Shout("Fafnir Boss actually is closed.", true);
            FafnirRad.IsLocked = true;
        }

        #endregion
    }
}
