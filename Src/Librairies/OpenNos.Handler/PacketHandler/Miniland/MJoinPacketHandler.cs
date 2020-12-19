﻿using System.Linq;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Miniland
{
    public class MJoinPacketHandler : IPacketHandler
    {
        #region Instantiation

        public MJoinPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void JoinMiniland(MJoinPacket mJoinPacket)
        {
            if (Session.Character.MapInstance.MapInstanceType == MapInstanceType.BaseMapInstance ||
                Session.Character.IsSeal)
            {
                var sess = ServerManager.Instance.GetSessionByCharacterId(mJoinPacket.CharacterId);
                if (sess?.Character != null && (ServerManager.Instance.ChannelId != 51 ||
                                                sess.Character.Faction == Session.Character.Faction))
                {
                    if (!Session.Character.IsFriendOfCharacter(sess.Character.CharacterId) && !sess.Character
                            .BattleEntity.GetOwnedNpcs()
                            .Any(s => sess.Character.BattleEntity.IsSignpost(s.NpcVNum)))
                    {
                        return;
                    }

                    if (sess.Character.MinilandState == MinilandState.Open)
                    {
                        ServerManager.Instance.JoinMiniland(Session, sess);
                    }
                    else
                    {
                        Session.SendPacket(
                                UserInterfaceHelper.GenerateInfo(
                                        Language.Instance.GetMessageFromKey("MINILAND_CLOSED_BY_FRIEND")));
                    }
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_DO_THAT"),
                    10));
            }
        }

        #endregion
    }
}