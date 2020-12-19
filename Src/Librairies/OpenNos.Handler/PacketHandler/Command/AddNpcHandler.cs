using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class AddNpcHandler : IPacketHandler
    {
        #region Instantiation

        public AddNpcHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void AddNpc(AddNpcPacket addNpcPacket)
        {
            if (addNpcPacket != null)
            {
                Session.AddLogsCmd(addNpcPacket);

                if (!Session.HasCurrentMapInstance) return;

                var npcmonster = ServerManager.GetNpcMonster(addNpcPacket.NpcVNum);
                if (npcmonster == null) return;

                var newNpc = new MapNpcDTO
                {
                    NpcVNum = addNpcPacket.NpcVNum,
                    MapY = Session.Character.PositionY,
                    MapX = Session.Character.PositionX,
                    MapId = Session.Character.MapInstance.Map.MapId,
                    Position = Session.Character.Direction,
                    IsMoving = addNpcPacket.IsMoving,
                    MapNpcId = ServerManager.Instance.GetNextNpcId()
                };
                if (!DAOFactory.MapNpcDAO.DoesNpcExist(newNpc.MapNpcId))
                {
                    DAOFactory.MapNpcDAO.Insert(newNpc);
                    if (DAOFactory.MapNpcDAO.LoadById(newNpc.MapNpcId) is MapNpcDTO npcDTO)
                    {
                        var npc = new MapNpc(npcDTO);
                        npc.Initialize(Session.CurrentMapInstance);
                        Session.CurrentMapInstance.AddNPC(npc);
                        Session.CurrentMapInstance?.Broadcast(npc.GenerateIn());
                    }
                }

                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(AddNpcPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}