using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class BuffHandler : IPacketHandler
    {
        #region Instantiation

        public BuffHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Buff(BuffPacket buffPacket)
        {         
            if (buffPacket != null)
            {
                Session.AddLogsCmd(buffPacket);
                var buff = new Buff(buffPacket.CardId, buffPacket.Level ?? (byte) 1);
                Session.Character.AddBuff(buff, Session.Character.BattleEntity);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(BuffPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}