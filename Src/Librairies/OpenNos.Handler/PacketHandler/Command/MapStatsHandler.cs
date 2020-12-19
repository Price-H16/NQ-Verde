﻿using System.Linq;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class MapStatsHandler : IPacketHandler
    {
        #region Instantiation

        public MapStatsHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void MapStats(MapStatisticsPacket mapStatPacket)
        {
            // lower the boilerplate
            void SendMapStats(MapDTO map, MapInstance mapInstance)
            {
                if (map != null && mapInstance != null)
                {
                    Session.SendPacket(Session.Character.GenerateSay("-------------MapData-------------", 10));
                    Session.SendPacket(Session.Character.GenerateSay(
                        $"MapId: {map.MapId}\n" +
                        $"MapMusic: {map.Music}\n" +
                        $"MapName: {map.Name}\n" +
                        $"MapShopAllowed: {map.ShopAllowed}", 10));
                    Session.SendPacket(Session.Character.GenerateSay("---------------------------------", 10));
                    Session.SendPacket(Session.Character.GenerateSay("---------MapInstanceData---------", 10));
                    Session.SendPacket(Session.Character.GenerateSay(
                        $"MapInstanceId: {mapInstance.MapInstanceId}\n" +
                        $"MapInstanceType: {mapInstance.MapInstanceType}\n" +
                        $"MapMonsterCount: {mapInstance.Monsters.Count}\n" +
                        $"MapNpcCount: {mapInstance.Npcs.Count}\n" +
                        $"MapPortalsCount: {mapInstance.Portals.Count}\n" +
                        $"MapInstanceUserShopCount: {mapInstance.UserShops.Count}\n" +
                        $"SessionCount: {mapInstance.Sessions.Count()}\n" +
                        $"MapInstanceXpRate: {mapInstance.XpRate}\n" +
                        $"MapInstanceDropRate: {mapInstance.DropRate}\n" +
                        $"MapInstanceMusic: {mapInstance.InstanceMusic}\n" +
                        $"ShopsAllowed: {mapInstance.ShopAllowed}\n" +
                        $"DropAllowed: {mapInstance.DropAllowed}\n" +
                        $"IsPVP: {mapInstance.IsPVP}\n" +
                        $"IsSleeping: {mapInstance.IsSleeping}\n" +
                        $"Dance: {mapInstance.IsDancing}", 10));
                    Session.SendPacket(Session.Character.GenerateSay("---------------------------------", 10));
                }
            }

            if (mapStatPacket != null)
            {
                Session.AddLogsCmd(mapStatPacket);
                if (mapStatPacket.MapId.HasValue)
                {
                    var map = DAOFactory.MapDAO.LoadById(mapStatPacket.MapId.Value);
                    var mapInstance = ServerManager.GetMapInstanceByMapId(mapStatPacket.MapId.Value);
                    if (map != null && mapInstance != null) SendMapStats(map, mapInstance);
                }
                else if (Session.HasCurrentMapInstance)
                {
                    var map = DAOFactory.MapDAO.LoadById(Session.CurrentMapInstance.Map.MapId);
                    if (map != null) SendMapStats(map, Session.CurrentMapInstance);
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(MapStatisticsPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}