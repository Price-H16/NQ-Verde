using System;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class MaintenanceHandler : IPacketHandler
    {
        #region Instantiation

        public MaintenanceHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void PlanMaintenance(MaintenancePacket maintenancePacket)
        {
            if (maintenancePacket != null)
            {
                Session.AddLogsCmd(maintenancePacket);
                var dateStart = DateTime.Now.AddMinutes(maintenancePacket.Delay);
                var maintenance = new MaintenanceLogDTO
                {
                    DateEnd = dateStart.AddMinutes(maintenancePacket.Duration),
                    DateStart = dateStart,
                    Reason = maintenancePacket.Reason
                };
                DAOFactory.MaintenanceLogDAO.Insert(maintenance);
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(MaintenancePacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}