using NosTale.GameObject.CommandPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;
using System;
using System.Reactive.Linq;
using System.Threading;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class SpawnDbHandler : IPacketHandler
    {
        #region Instantiation

        public SpawnDbHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void SpanwMobPerma(SpawnpermaPacket packet)
        {
            if (packet != null)
            {
                int count = packet.Count;
                int time = packet.Time;
                int a = packet.MapMonsterId;
                if (Session.IsOnMap && Session.HasCurrentMapInstance)
                {
                    MapInstance instance = Session.CurrentMapInstance;
                    NpcMonster npcmonster = ServerManager.GetNpcMonster(packet.VNum);

                    if (npcmonster == null)
                    {
                        return;
                    }

                    Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe(observer =>
                    {
                        for (int i = 0; i < count; i++)
                        {
                            MapCell cell = instance.Map.GetRandomPosition();
                            int MonsterId = a++;
                            MapMonster monster = new MapMonster { MonsterVNum = packet.VNum, MapX = cell.X, MapY = cell.Y, MapMonsterId = MonsterId, IsHostile = true, IsMoving = true, Position = 0, MapId = Session.CurrentMapInstance.Map.MapId };
                            monster.Initialize(instance);
                            instance.AddMonster(monster);
                            instance.Broadcast(monster.GenerateIn());
                            DAOFactory.MapMonsterDAO.Insert(monster);
                            Thread.Sleep(time * 1000 / count);
                        }
                    });
                }
            }

            #endregion
        }
    }
}