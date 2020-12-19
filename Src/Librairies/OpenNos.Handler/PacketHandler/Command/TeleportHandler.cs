﻿using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class TeleportHandler : IPacketHandler
    {
        #region Instantiation

        public TeleportHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Teleport(TeleportPacket teleportPacket)
        {
            if (teleportPacket != null)
            {
                Session.AddLogsCmd(teleportPacket);
                if (Session.Character.HasShopOpened || Session.Character.InExchangeOrTrade)
                    Session.Character.DisposeShopAndExchange();

                if (Session.Character.IsChangingMapInstance) return;

                var session = ServerManager.Instance.GetSessionByCharacterName(teleportPacket.Data);

                if (session != null)
                {
                    var mapX = session.Character.PositionX;
                    var mapY = session.Character.PositionY;
                    if (session.Character.Miniland == session.Character.MapInstance)
                        ServerManager.Instance.JoinMiniland(Session, session);
                    else
                        ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId,
                            session.Character.MapInstanceId, mapX, mapY);
                }
                else if (short.TryParse(teleportPacket.Data, out var mapId))
                {
                    if (ServerManager.GetBaseMapInstanceIdByMapId(mapId) != default)
                    {
                        if (teleportPacket.X == 0 && teleportPacket.Y == 0)
                            ServerManager.Instance.TeleportOnRandomPlaceInMap(Session,
                                ServerManager.GetBaseMapInstanceIdByMapId(mapId));
                        else
                            ServerManager.Instance.ChangeMap(Session.Character.CharacterId, mapId, teleportPacket.X,
                                teleportPacket.Y);
                    }
                    else
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("MAP_NOT_FOUND"), 0));
                    }
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(TeleportPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}