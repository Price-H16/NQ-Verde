using System.Linq;
using NosTale.Extension.Extension.Packet;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Battle;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Battle
{
    public class UseIconFalconSkillPacketHandler : IPacketHandler
    {
        #region Instantiation

        public UseIconFalconSkillPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void UseIconFalconSkill(UseIconFalconSkillPacket useIconFalconSkillPacket)
        {
            if (Session.Character.BattleEntity.FalconFocusedEntityId > 0)
            {
                var iconSkillHitRequest = new HitRequest(TargetHitType.SingleTargetHit, Session,
                    ServerManager.GetSkill(1248), 4283);
                if (Session.CurrentMapInstance.BattleEntities.FirstOrDefault(s =>
                        s.MapEntityId == Session.Character.BattleEntity.FalconFocusedEntityId) is BattleEntity
                    FalconFocusedEntity)
                {
                    Session.SendPacket("ob_ar");
                    switch (FalconFocusedEntity.EntityType)
                    {
                        case EntityType.Player:
                            Session.PvpHit(iconSkillHitRequest, FalconFocusedEntity.Character.Session);
                            break;

                        case EntityType.Monster:
                            FalconFocusedEntity.MapMonster.HitQueue.Enqueue(iconSkillHitRequest);
                            break;

                        case EntityType.Mate:
                            FalconFocusedEntity.Mate.HitRequest(iconSkillHitRequest);
                            break;
                    }
                    Session.Character.BattleEntity.FalconFocusedEntityId = -2;

                    Session.CurrentMapInstance.Broadcast(Session,
                        $"eff_ob {(byte)FalconFocusedEntity.UserType} {FalconFocusedEntity.MapEntityId} 0 4269",
                        ReceiverType.AllExceptMe);
                }

            }
            else if (Session.Character.BattleEntity.FalconFocusedEntityId == -2)
            {
                Logger.Info($"ob_a packet abuse try {Session.Character.GenerateIdentity()} {Session.Character.Name}");
            }
        }

        #endregion
    }
}