using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class EffectHandler : IPacketHandler
    {
        #region Instantiation

        public EffectHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Effect(EffectCommandPacket effectCommandpacket)
        {
            if (effectCommandpacket != null)
            {
                Session.AddLogsCmd(effectCommandpacket);
                Session.CurrentMapInstance?.Broadcast(
                    StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId,
                        effectCommandpacket.EffectId), Session.Character.PositionX, Session.Character.PositionY);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(EffectCommandPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}