using System;
using System.Linq;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    internal class WarpHandler : IPacketHandler
    {
        #region Instantiation

        public WarpHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Warp(WarpPacket WarpPacket)
        {
            var time = Session.Character.LastWarp.AddSeconds(30);

            if (DateTime.Now <= time) // Anti spam
            {
                Session.SendPacket(Session.Character.GenerateSay("Warp command is in cooldown, you have to wait 30 seconds to use it again", 11));
                return;
            }

            if (Session.Character.MapInstance.MapInstanceType != MapInstanceType.BaseMapInstance) return;

            if (WarpPacket == null || WarpPacket.MapId == 0)
            {
                SendError();
                return;
            }

            if (Session.Character.CharacterVisitedMaps.All(x => x.MapId != WarpPacket.MapId))
            {
                SendError();
                return;
            }

            if ( ServerManager.Instance.ChannelId == 51)
            {
                Session.SendPacket(Session.Character.GenerateSay("You can't TP to any map while being in A4.", 11));
                return;
            }

            var foundVisitedMap = Session.Character.CharacterVisitedMaps.Find(x => x.MapId == WarpPacket.MapId);

            if (foundVisitedMap == null) // This shouldn't happen, but just in case
            {
                return;
            }

            if (ServerManager.GetMapInstanceByMapId((short)foundVisitedMap.MapId).Map.MapTypes.Any(s=> s.MapTypeId == (short)MapTypeEnum.Act4  || s.MapTypeId == (short)MapTypeEnum.LandOfTheDead))
            {
                Session.SendPacket(Session.Character.GenerateSay("There are certain maps you can't tp to, you know...", 11));
                return;
            }

            ServerManager.Instance.ChangeMap(Session.Character.CharacterId, (short)foundVisitedMap.MapId, (short)foundVisitedMap.MapX, (short)foundVisitedMap.MapY);

            //specific positions
            if (WarpPacket.MapId == 1)
            {
                ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 1, 79, 116);
            }
            if (WarpPacket.MapId == 2604)
            {
                ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 2604, 231, 154);
            }
            if (WarpPacket.MapId == 2601)
            {
                ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 2601, 35, 152);
            }

            Session.Character.LastWarp = DateTime.Now;
        }

        private void SendError()
        {
            string maps = "To warp the desired map, use $Warp [And write the desired map id from the list below]\n\n";


            Session.Character.CharacterVisitedMaps.OrderBy(x => x.MapId).ToList().ForEach(x =>
            {
                if (x.MapId != 0)
                {
                    maps += $"{x.MapId} - {ServerManager.GetAllMapInstances().Find(y => y.Map.MapId == x.MapId)?.Map?.Name ?? "Unknown"}\n";

                }
            });

            Session.SendPacket(Session.Character.GenerateSay(maps, 11));
        }

        #endregion
    }
}