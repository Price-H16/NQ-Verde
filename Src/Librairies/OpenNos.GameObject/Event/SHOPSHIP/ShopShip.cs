using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;

namespace OpenNos.GameObject.SHOPSHIP
{

    public static class ShopShipev
    {
        #region Properties

        public static MapInstance ShopMapInstance { get; set; }

        public static MapInstance UnknownLandMapInstance { get; set; }

        #endregion

        #region Methods

        public static void Run()
        {
            ShopShip raidThread = new ShopShip();
            Observable.Timer(TimeSpan.FromMinutes(0)).Subscribe(X => raidThread.Run());
        }

        #endregion
    }

    public class ShopShip
    {
        #region Methods

        public void Run()
        {
            CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
            {
                DestinationCharacterId = null,
                SourceCharacterId = 0,
                SourceWorldId = ServerManager.Instance.WorldId,
                Message = "Crylos has returned to the surface, you have (5) minutes to explore the well!",
                Type = MessageType.Shout
            });

            ShopShipev.ShopMapInstance = ServerManager.GenerateMapInstance(2587, MapInstanceType.ShopShip, new InstanceBag());
            ShopShipev.UnknownLandMapInstance = ServerManager.GetMapInstance(ServerManager.GetBaseMapInstanceIdByMapId(1));

            MapNpc CaptainEnter = new MapNpc
            {
                NpcVNum = 2358,
                MapX = 35,
                MapY = 110,
                MapId = 1,
                ShouldRespawn = false,
                IsMoving = false,
                MapNpcId = ShopShipev.UnknownLandMapInstance.GetNextNpcId(),
                Position = 4,
                Name = $"Crylos"
            };
            CaptainEnter.Initialize(ShopShipev.UnknownLandMapInstance);
            ShopShipev.UnknownLandMapInstance.AddNPC(CaptainEnter);
            ShopShipev.UnknownLandMapInstance.Broadcast(CaptainEnter.GenerateIn());

            MapNpc CaptainExit = new MapNpc
            {
                NpcVNum = 2358,
                MapX = 13,
                MapY = 43,
                MapId = 2587,
                ShouldRespawn = false,
                IsMoving = false,
                MapNpcId = ShopShipev.ShopMapInstance.GetNextNpcId(),
                Position = 1,
                Name = $"Crylos"
            };
            CaptainExit.Initialize(ShopShipev.ShopMapInstance);
            ShopShipev.ShopMapInstance.AddNPC(CaptainExit);
            ShopShipev.ShopMapInstance.Broadcast(CaptainExit.GenerateIn());

            void RemoveNpc()
            {
                ShopShipev.ShopMapInstance.RemoveNpc(CaptainExit);
                CaptainExit.MapInstance.Broadcast(StaticPacketHelper.Out(UserType.Npc, CaptainExit.MapNpcId));

                ShopShipev.UnknownLandMapInstance.RemoveNpc(CaptainEnter);
                CaptainEnter.MapInstance.Broadcast(StaticPacketHelper.Out(UserType.Npc, CaptainEnter.MapNpcId));
            }
            List<EventContainer> onDeathEvents = new List<EventContainer>
            {
               new EventContainer(ShopShipev.ShopMapInstance, EventActionType.SCRIPTEND, (byte)1)
            };

            try
            {
                Observable.Timer(TimeSpan.FromMinutes(1)).Subscribe(X => FirstAnounce());
                Observable.Timer(TimeSpan.FromMinutes(2)).Subscribe(X => FirstAnounce2());
                Observable.Timer(TimeSpan.FromMinutes(3)).Subscribe(X => FirstAnounce3());
                Observable.Timer(TimeSpan.FromMinutes(4)).Subscribe(X => FirstAnounce4());
                Observable.Timer(TimeSpan.FromSeconds(270)).Subscribe(X => FirstAnounce5());

                Observable.Timer(TimeSpan.FromMinutes(5)).Subscribe(X => EndRaid());
                Observable.Timer(TimeSpan.FromSeconds(295)).Subscribe(X => RemoveNpc());
            }
            catch (Exception ex)
            {

            }



        }

        public void EndRaid()
        {
            try
            {
                CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                {
                    DestinationCharacterId = null,
                    SourceCharacterId = 0,
                    SourceWorldId = ServerManager.Instance.WorldId,
                    Message = "The oxygen has run out, it will be sent to the surface!!",
                    Type = MessageType.Shout
                });

                foreach (ClientSession sess in ShopShipev.ShopMapInstance.Sessions.ToList())
                {
                    ServerManager.Instance.ChangeMapInstance(sess.Character.CharacterId, ShopShipev.UnknownLandMapInstance.MapInstanceId, sess.Character.MapX, sess.Character.MapY);
                    Thread.Sleep(100);
                }
                EventHelper.Instance.RunEvent(new EventContainer(ShopShipev.ShopMapInstance, EventActionType.DISPOSEMAP, null));
                ServerManager.Instance.StartedEvents.Remove(EventType.SHOPSHIP);
            }
            catch (Exception ex)
            {

            }

        }


        #region Anuncios
        public void FirstAnounce()
        {
            CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
            {
                DestinationCharacterId = null,
                SourceCharacterId = 0,
                SourceWorldId = ServerManager.Instance.WorldId,
                Message = "[4] Minutes of Oxygen Left!",
                Type = MessageType.Shout
            });
        }

        public void FirstAnounce2()
        {
            CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
            {
                DestinationCharacterId = null,
                SourceCharacterId = 0,
                SourceWorldId = ServerManager.Instance.WorldId,
                Message = "[3] Minutes of Oxygen Left!",
                Type = MessageType.Shout
            });
        }

        public void FirstAnounce3()
        {
            CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
            {
                DestinationCharacterId = null,
                SourceCharacterId = 0,
                SourceWorldId = ServerManager.Instance.WorldId,
                Message = "[2] Minutes of Oxygen Left!",
                Type = MessageType.Shout
            });
        }

        public void FirstAnounce4()
        {
            CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
            {
                DestinationCharacterId = null,
                SourceCharacterId = 0,
                SourceWorldId = ServerManager.Instance.WorldId,
                Message = "[1] Minutes of Oxygen Left!",
                Type = MessageType.Shout
            });
        }

        public void FirstAnounce5()
        {
            CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
            {
                DestinationCharacterId = null,
                SourceCharacterId = 0,
                SourceWorldId = ServerManager.Instance.WorldId,
                Message = "[30] Seconds of Oxygen Left!",
                Type = MessageType.Shout
            });
        }
        #endregion
        #endregion
    }
}