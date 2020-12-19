﻿using System;
using System.Linq;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Basic
{
    public class RStartPacketHandler : IPacketHandler
    {
        #region Instantiation

        public RStartPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void GetRStart(RStartPacket rStartPacket)
        {
            if (Session.Character.Timespace != null)
            {
                if (rStartPacket.Type                            == 1 && Session.Character.Timespace.InstanceBag != null &&
                    Session.Character.Timespace.InstanceBag.Lock == false)
                {
                    if (Session.Character.Timespace.SpNeeded?[(byte) Session.Character.Class] != 0)
                    {
                        var specialist =
                                Session.Character.Inventory?.LoadBySlotAndType((byte) EquipmentType.Sp, InventoryType.Wear);
                        if (specialist == null || specialist.ItemVNum !=
                                Session.Character.Timespace.SpNeeded?[(byte) Session.Character.Class])
                        {
                            Session.SendPacket(
                                    UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TS_SP_NEEDED"),
                                            0));
                            return;
                        }
                    }

                    Session.Character.Timespace.InstanceBag.Lock = true;
                    new PreqPacketHandler(Session).Preq(new PreqPacket());
                    
                }
            }
        }

        #endregion
    }
}