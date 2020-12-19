using System.Linq;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class ClearMapHandler : IPacketHandler
    {
        #region Instantiation

        public ClearMapHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void ClearMap(ClearMapPacket clearMapPacket)
        {
            if (clearMapPacket != null && Session.HasCurrentMapInstance)
            {
                Session.AddLogsCmd(clearMapPacket);
                foreach (var monster in Session.CurrentMapInstance.Monsters.Where(s => s.ShouldRespawn != true))
                {
                    Session.CurrentMapInstance.Broadcast(StaticPacketHelper.Out(UserType.Monster,
                        monster.MapMonsterId));
                    monster.SetDeathStatement();
                    Session.CurrentMapInstance.RemoveMonster(monster);
                }

                foreach (var drop in Session.CurrentMapInstance.DroppedList.GetAllItems())
                {
                    Session.CurrentMapInstance.Broadcast(StaticPacketHelper.Out(UserType.Object, drop.TransportId));
                    Session.CurrentMapInstance.DroppedList.Remove(drop.TransportId);
                }

                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ClearMapPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}