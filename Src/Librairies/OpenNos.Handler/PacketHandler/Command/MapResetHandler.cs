﻿using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class MapResetHandler : IPacketHandler
    {
        #region Instantiation

        public MapResetHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void MapReset(MapResetPacket mapResetPacket)
        {
            if (mapResetPacket != null)
            {
                if (Session.Character.IsChangingMapInstance) return;
                if (Session.CurrentMapInstance != null)
                {
                    Session.AddLogsCmd(mapResetPacket);
                    var newMapInstance = ServerManager.ResetMapInstance(Session.CurrentMapInstance);

                    foreach (var sess in Session.CurrentMapInstance.Sessions)
                        ServerManager.Instance.ChangeMapInstance(sess.Character.CharacterId,
                            newMapInstance.MapInstanceId, sess.Character.PositionX, sess.Character.PositionY);
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(MapResetPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}