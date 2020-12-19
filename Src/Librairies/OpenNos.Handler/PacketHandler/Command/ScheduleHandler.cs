using System;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    internal class ScheduleHandler : IPacketHandler
    {
        #region Instantiation

        public ScheduleHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Schedule(SchedulePacket schedulePacket)
        {
            var time = Session.Character.LastSchedule.AddSeconds(10);

            Session.AddLogsCmd(schedulePacket);

            if (DateTime.Now <= time)
            {
                return;
            }            
            Session.Character.LastSchedule = DateTime.Now;

            Session.SendPacket(Session.Character.GenerateSay("---------Event Schedule--------", 10));
            Session.SendPacket(Session.Character.GenerateSay($"LOD: 00:00", 12));
            Session.SendPacket(Session.Character.GenerateSay($"LOD: 02:00", 12));
            Session.SendPacket(Session.Character.GenerateSay($"LOD: 04:00", 12));
            Session.SendPacket(Session.Character.GenerateSay($"LOD: 06:00", 12));
            Session.SendPacket(Session.Character.GenerateSay($"LOD: 08:00", 12));
            Session.SendPacket(Session.Character.GenerateSay($"LOD: 10:00", 12));
            Session.SendPacket(Session.Character.GenerateSay($"LOD: 12:00", 12));
            Session.SendPacket(Session.Character.GenerateSay($"LOD: 14:00", 12));
            Session.SendPacket(Session.Character.GenerateSay($"LOD: 16:00", 12));
            Session.SendPacket(Session.Character.GenerateSay($"LOD: 18:00", 12));
            Session.SendPacket(Session.Character.GenerateSay($"LOD: 20:00", 12));
            Session.SendPacket(Session.Character.GenerateSay($"LOD: 22:00", 12));
            Session.SendPacket(Session.Character.GenerateSay($"RAINBOWBATTLE: 10:00", 12));
            Session.SendPacket(Session.Character.GenerateSay($"RAINBOWBATTLE: 13:00", 12));
            Session.SendPacket(Session.Character.GenerateSay($"RAINBOWBATTLE: 16:00", 12));
            Session.SendPacket(Session.Character.GenerateSay($"RAINBOWBATTLE: 19:00", 12));
            Session.SendPacket(Session.Character.GenerateSay($"RAINBOWBATTLE: 22:00", 12));
            Session.SendPacket(Session.Character.GenerateSay("INSTANT COMBAT: Every 2 hours except on autoreboot time", 12));
            Session.SendPacket(Session.Character.GenerateSay("METEORITE GAME: 15:40", 12));
            Session.SendPacket(Session.Character.GenerateSay("METEORITE GAME: 17:40", 12));
            Session.SendPacket(Session.Character.GenerateSay("METEORITE GAME: 19:15", 12));
            Session.SendPacket(Session.Character.GenerateSay("TALENTARENA: 15:00 ", 12));
            Session.SendPacket(Session.Character.GenerateSay("CALIGOR: 17:00", 12));
            Session.SendPacket(Session.Character.GenerateSay("AUTOMATIC TASK: AUTOREBOOT AT 4AM", 12));
            Session.SendPacket(Session.Character.GenerateSay("---------------------------------", 10));
        }

        #endregion
    }
}