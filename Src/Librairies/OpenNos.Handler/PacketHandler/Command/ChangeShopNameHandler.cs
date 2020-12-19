using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class ChangeShopNameHandler : IPacketHandler
    {
        #region Instantiation

        public ChangeShopNameHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void ChangeShopName(ChangeShopNamePacket changeShopNamePacket)
        {
            if (Session.HasCurrentMapInstance)
            {
                if (!string.IsNullOrEmpty(changeShopNamePacket.Name))
                {
                    if (Session.CurrentMapInstance.GetNpc(Session.Character.LastNpcMonsterId) is MapNpc npc)
                    {
                        if (npc.Shop is Shop shop)
                        {
                            Session.AddLogsCmd(changeShopNamePacket);

                            if (DAOFactory.ShopDAO.LoadById(shop.ShopId) is ShopDTO shopDTO)
                            {
                                shop.Name = changeShopNamePacket.Name;
                                shopDTO.Name = changeShopNamePacket.Name;
                                DAOFactory.ShopDAO.Update(ref shopDTO);

                                Session.CurrentMapInstance.Broadcast(
                                    $"shop 2 {npc.MapNpcId} {npc.Shop.ShopId} {npc.Shop.MenuType} {npc.Shop.ShopType} {npc.Shop.Name}");
                            }
                        }
                    }
                    else
                    {
                        Session.SendPacket(
                            Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NPCMONSTER_NOT_FOUND"),
                                11));
                    }
                }
                else
                {
                    Session.SendPacket(Session.Character.GenerateSay(ChangeShopNamePacket.ReturnHelp(), 10));
                }
            }
        }

        #endregion
    }
}