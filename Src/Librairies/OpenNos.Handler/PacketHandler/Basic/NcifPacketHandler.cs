using System.Linq;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Basic
{
    public class NcifPacketHandler : IPacketHandler
    {
        #region Instantiation

        public NcifPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void GetNamedCharacterInformation(NcifPacket ncifPacket)
        {
            switch (ncifPacket.Type)
            {
                // characters
                case 1:
                    Session.SendPacket(ServerManager.Instance.GetSessionByCharacterId(ncifPacket.TargetId)?.Character
                        ?.GenerateStatInfo());
                    break;

                // npcs/mates
                case 2:
                    if (Session.HasCurrentMapInstance)
                    {
                        Session.CurrentMapInstance.Npcs.Where(n => n.MapNpcId == (int) ncifPacket.TargetId).ToList()
                            .ForEach(npc =>
                            {
                                var npcinfo = ServerManager.GetNpcMonster(npc.NpcVNum);
                                if (npcinfo == null) return;

                                Session.Character.LastNpcMonsterId = npc.MapNpcId;
                                Session.SendPacket(
                                    $"st 2 {ncifPacket.TargetId} {npcinfo.Level} {npcinfo.HeroLevel} {(int) ((float) npc.CurrentHp / (float) npc.MaxHp * 100)} {(int) ((float) npc.CurrentMp / (float) npc.MaxMp * 100)} {npc.CurrentHp} {npc.CurrentMp}{npc.Buff.GetAllItems().Aggregate("", (current, buff) => current + $" {buff.Card.CardId}.{buff.Level}")}");
                            });
                        foreach (var session in Session.CurrentMapInstance.Sessions)
                        {
                            var mate = session.Character.Mates.Find(
                                s => s.MateTransportId == (int) ncifPacket.TargetId);
                            if (mate != null) Session.SendPacket(mate.GenerateStatInfo());
                        }
                    }

                    break;

                // monsters
                case 3:
                    if (Session.HasCurrentMapInstance)
                        Session.CurrentMapInstance.Monsters.Where(m => m.MapMonsterId == (int) ncifPacket.TargetId)
                            .ToList().ForEach(monster =>
                            {
                                var monsterinfo = ServerManager.GetNpcMonster(monster.MonsterVNum);
                                if (monsterinfo == null) return;

                                Session.Character.LastNpcMonsterId = monster.MapMonsterId;
                                Session.SendPacket(
                                    $"st 3 {ncifPacket.TargetId} {monsterinfo.Level} {monsterinfo.HeroLevel} {(int) ((float) monster.CurrentHp / (float) monster.MaxHp * 100)} {(int) ((float) monster.CurrentMp / (float) monster.MaxMp * 100)} {monster.CurrentHp} {monster.CurrentMp}{monster.Buff.GetAllItems().Aggregate("", (current, buff) => current + $" {buff.Card.CardId}.{buff.Level}")}");
                            });

                    break;
            }
        }

        #endregion
    }
}