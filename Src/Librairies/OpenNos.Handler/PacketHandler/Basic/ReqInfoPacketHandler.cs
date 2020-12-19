using System.Linq;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;
using OpenNos.GameObject.Extension;


namespace OpenNos.Handler.PacketHandler.Basic
{
    public class ReqInfoPacketHandler : IPacketHandler
    {
        #region Instantiation

        public ReqInfoPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void ReqInfo(ReqInfoPacket reqInfoPacket)
        {
            if (Session.Character != null)
            {
                if (reqInfoPacket.Type == 6)
                {
                    if (reqInfoPacket.MateVNum.HasValue)
                    {
                        var mate = Session.CurrentMapInstance?.Sessions?.FirstOrDefault(s =>
                                s?.Character?.Mates != null && s.Character.Mates.Any(o =>
                                    o.MateTransportId == reqInfoPacket.MateVNum.Value))?
                            .Character.Mates.FirstOrDefault(m => m.MateTransportId == reqInfoPacket.MateVNum.Value);

                        if (mate != null) Session.SendPacket(mate.GenerateEInfo());
                    }
                }
                else if (reqInfoPacket.Type == 5)
                {
                    var npc = ServerManager.GetNpcMonster((short) reqInfoPacket.TargetVNum);

                    if (Session.CurrentMapInstance?.GetMonsterById(Session.Character.LastNpcMonsterId)
                            is MapMonster monster &&
                        monster.Monster?.OriginalNpcMonsterVNum == reqInfoPacket.TargetVNum)
                        npc = ServerManager.GetNpcMonster(monster.Monster.NpcMonsterVNum);

                    if (npc != null) Session.SendPacket(npc.GenerateEInfo());
                }
                else if (reqInfoPacket.Type == 12)
                {
                    if (Session.Character.Inventory != null)
                        Session.SendPacket(Session.Character.Inventory
                            .LoadBySlotAndType((short) reqInfoPacket.TargetVNum, InventoryType.Equipment)
                            ?.GenerateReqInfo());
                }
                else
                {
                    if (ServerManager.Instance.GetSessionByCharacterId(reqInfoPacket.TargetVNum)?.Character is Character
                        character) Session.SendPacket(character.GenerateReqInfo());
                }
            }
        }

        #endregion
    }
}