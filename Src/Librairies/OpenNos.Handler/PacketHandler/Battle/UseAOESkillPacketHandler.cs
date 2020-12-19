using System;
using NosTale.Extension.Extension.Packet;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Battle
{
    public class UseAOESkillPacketHandler : IPacketHandler
    {
        #region Instantiation

        public UseAOESkillPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void UseZonesSkill(UseAOESkillPacket useAoeSkillPacket)
        {
            Session.Character.Direction = Session.Character.BeforeDirection;

            var isMuted = Session.Character.MuteMessage();
            if (isMuted || Session.Character.IsVehicled)
            {
                Session.SendPacket(StaticPacketHelper.Cancel());
            }
            else
            {
                if (Session.Character.LastTransform.AddSeconds(3) > DateTime.Now)
                {
                    Session.SendPacket(StaticPacketHelper.Cancel());
                    Session.SendPacket(
                        UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CANT_ATTACK"), 0));
                    return;
                }

                if (Session.Character.CanFight && Session.Character.Hp > 0)
                    Session.ZoneHit(useAoeSkillPacket.CastId, useAoeSkillPacket.MapX, useAoeSkillPacket.MapY);
            }
        }

        #endregion
    }
}