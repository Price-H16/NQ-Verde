using System;
using System.Collections.Generic;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class MobRainHandler : IPacketHandler
    {
        #region Instantiation

        public MobRainHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void MobRain(MobRainPacket mobRainPacket)
        {
            if (mobRainPacket != null)
            {
                Session.AddLogsCmd(mobRainPacket);
                if (Session.IsOnMap && Session.HasCurrentMapInstance)
                {
                    var npcmonster = ServerManager.GetNpcMonster(mobRainPacket.NpcMonsterVNum);
                    if (npcmonster == null) return;

                    var SummonParameters = new List<MonsterToSummon>();
                    SummonParameters.AddRange(Session.Character.MapInstance.Map.GenerateMonsters(
                        mobRainPacket.NpcMonsterVNum, mobRainPacket.Amount, mobRainPacket.IsMoving,
                        new List<EventContainer>()));
                    EventHelper.Instance.ScheduleEvent(TimeSpan.FromSeconds(1),
                        new EventContainer(Session.CurrentMapInstance, EventActionType.SPAWNMONSTERS,
                            SummonParameters));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(MobRainPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}