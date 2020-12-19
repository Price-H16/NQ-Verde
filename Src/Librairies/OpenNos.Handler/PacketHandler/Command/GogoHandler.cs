﻿using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class GogoHandler : IPacketHandler
    {
        #region Instantiation

        public GogoHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Gogo(GogoPacket gogoPacket)
        {
            if (gogoPacket != null)
            {
                if (Session.Character.HasShopOpened || Session.Character.InExchangeOrTrade)
                    Session.Character.DisposeShopAndExchange();

                if (Session.Character.IsChangingMapInstance) return;

                if (Session.CurrentMapInstance != null)
                {
                    Session.AddLogsCmd(gogoPacket);
                    if (gogoPacket.X == 0 && gogoPacket.Y == 0)
                        ServerManager.Instance.TeleportOnRandomPlaceInMap(Session,
                            Session.CurrentMapInstance.MapInstanceId);
                    else
                        ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId,
                            Session.CurrentMapInstance.MapInstanceId, gogoPacket.X, gogoPacket.Y);
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(GogoPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}