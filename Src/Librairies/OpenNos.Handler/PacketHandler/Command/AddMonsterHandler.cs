using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class AddMonsterHandler : IPacketHandler
    {
        #region Instantiation

        public AddMonsterHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void AddMonster(AddMonsterPacket addMonsterPacket)
        {
            if (addMonsterPacket != null)
            {
                Session.AddLogsCmd(addMonsterPacket);
                if (!Session.HasCurrentMapInstance) return;

                var npcmonster = ServerManager.GetNpcMonster(addMonsterPacket.MonsterVNum);
                if (npcmonster == null) return;

                var monst = new MapMonsterDTO
                {
                    MonsterVNum = addMonsterPacket.MonsterVNum,
                    MapY = Session.Character.PositionY,
                    MapX = Session.Character.PositionX,
                    MapId = Session.Character.MapInstance.Map.MapId,
                    Position = Session.Character.Direction,
                    IsMoving = addMonsterPacket.IsMoving,
                    MapMonsterId = ServerManager.Instance.GetNextMobId()
                };
                if (!DAOFactory.MapMonsterDAO.DoesMonsterExist(monst.MapMonsterId))
                {
                    DAOFactory.MapMonsterDAO.Insert(monst);
                    if (DAOFactory.MapMonsterDAO.LoadById(monst.MapMonsterId) is MapMonsterDTO monsterDTO)
                    {
                        var monster = new MapMonster(monsterDTO);
                        monster.Initialize(Session.CurrentMapInstance);
                        Session.CurrentMapInstance.AddMonster(monster);
                        Session.CurrentMapInstance?.Broadcast(monster.GenerateIn());
                    }
                }

                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(AddMonsterPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}