using System;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class UnstuckHandler : IPacketHandler
    {
        #region Instantiation

        public UnstuckHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Unstuck(UnstuckPacket unstuckPacket)
        {
            var time = Session.Character.LastCMD.AddSeconds(10);
            Session.AddLogsCmd(unstuckPacket);
            if (DateTime.Now <= time) // Anti spam
                return;
            Session.Character.LastCMD = DateTime.Now;

            if (Session?.Character != null)
            {
                if (Session.CurrentMapInstance.MapInstanceType.Equals(MapInstanceType.RainbowBattleInstance))
                {
                    Session.SendPacket(StaticPacketHelper.Cancel(2));
                }

                if (Session.Character.Miniland == Session.Character.MapInstance)
                {
                    ServerManager.Instance.JoinMiniland(Session, Session);
                }
                else if (!Session.Character.IsSeal
                         && !Session.CurrentMapInstance.MapInstanceType.Equals(MapInstanceType.TalentArenaMapInstance)
                      && !Session.CurrentMapInstance.MapInstanceType.Equals(MapInstanceType.IceBreakerInstance)
                      && !Session.CurrentMapInstance.MapInstanceType.Equals(MapInstanceType.RainbowBattleInstance))
                {
                    ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId,
                        Session.Character.MapInstanceId, Session.Character.PositionX, Session.Character.PositionY,
                        true);
                    Session.SendPacket(StaticPacketHelper.Cancel(2));
                }
            }
        }

        #endregion
    }
}