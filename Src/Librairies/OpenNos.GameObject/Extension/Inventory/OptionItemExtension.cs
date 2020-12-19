using System;
using System.Linq;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject.Extension.Inventory
{
    public static class OptionItemExtension
    {
        #region Methods
   
        public static void OptionItem(this ItemInstance e, ClientSession session, ItemInstance cellon)
        {
            if (e.EquipmentSerialId == Guid.Empty)
            {
                e.EquipmentSerialId = Guid.NewGuid();
            }
            
            if (e.Item.MaxCellon <= e.CellonOptions.Count)
            {
                session.SendPacket($"info {Language.Instance.GetMessageFromKey("MAX_OPTIONS")}");
                session.SendPacket("shop_end 1");
                return;
            }
            
            if (session.Character.Inventory.CountItem(cellon.ItemVNum) > 0)
            {
                byte dataIndex = 0;
                var goldAmount = 0;
                switch (cellon.ItemVNum)
                {
                    case 1017:
                        dataIndex = 0;
                        goldAmount = 700;
                        break;

                    case 1018:
                        dataIndex = 1;
                        goldAmount = 1400;
                        break;

                    case 1019:
                        dataIndex = 2;
                        goldAmount = 3000;
                        break;

                    case 1020:
                        dataIndex = 3;
                        goldAmount = 5000;
                        break;

                    case 1021:
                        dataIndex = 4;
                        goldAmount = 10000;
                        break;

                    case 1022:
                        dataIndex = 5;
                        goldAmount = 20000;
                        break;

                    case 1023:
                        dataIndex = 6;
                        goldAmount = 32000;
                        break;

                    case 1024:
                        dataIndex = 7;
                        goldAmount = 58000;
                        break;

                    case 1025:
                        dataIndex = 8;
                        goldAmount = 95000;
                        break;

                    case 1026:
                        dataIndex = 9;
                        goldAmount = 150000;
                        return;

                        //no data known, not implemented in the client at all right now
                        //dataIndex = 9;
                        //break;
                }

                if (e.Item.MaxCellonLvl > dataIndex && session.Character.Gold >= goldAmount)
                {
                    short[][] minimumData = {
                    new short[] { 30, 50, 5, 8, 0, 0 },             //lv1
                    new short[] { 120, 150, 14, 16, 0, 0 },         //lv2
                    new short[] { 220, 280, 22, 28, 0, 0 },         //lv3
                    new short[] { 330, 350, 30, 38, 0, 0 },         //lv4
                    new short[] { 430, 450, 40, 50, 0, 0 },         //lv5
                    new short[] { 600, 600, 55, 65, 1, 1 },         //lv6
                    new short[] { 800, 800, 75, 75, 8, 11 },        //lv7
                    new short[] { 1000, 1000, 100, 100, 13, 21 },   //lv8
                    new short[] { 1100, 1100, 110, 110, 14, 22 },   //lv9
                    new short[] { 1400, 1400, 135, 135, 20, 25 }    //lv10
                    };
                    short[][] maximumData = {
                    new short[] { 100, 150, 10, 15, 0, 0 },         //lv1
                    new short[] { 200, 250, 20, 25, 0, 0 },         //lv2
                    new short[] { 300, 330, 28, 35, 0, 0 },         //lv3
                    new short[] { 400, 420, 38, 45, 0, 0 },         //lv4
                    new short[] { 550, 550, 50, 60, 0, 0 },         //lv5
                    new short[] { 750, 750, 70, 80, 7, 10 },        //lv6
                    new short[] { 1000, 1000, 90,90, 12, 20 },      //lv7
                    new short[] { 1300, 1300, 120, 120, 17, 35 },   //lv8
                    new short[] { 1500, 1500, 135, 135, 21, 45 },   //lv9
                    new short[] { 2000, 2000, 185, 185, 30, 55 }    //lv10
                    };

                    short[] generateOption()
                    {
                        byte option = 0;
                        if (dataIndex < 5)
                        {
                            option = (byte)ServerManager.RandomNumber(0, 4);
                        }
                        else
                        {
                            option = (byte)ServerManager.RandomNumber(0, 6);
                        }

                        if (e.CellonOptions.Any(s => s.Type == (CellonOptionType)option))
                        {
                            return new short[] { -1, -1 };
                        }

                        return new[] { option, (short)ServerManager.RandomNumber(minimumData[dataIndex][option], maximumData[dataIndex][option] + 1) };
                    }

                    var value = generateOption();
                    Logger.LogUserEvent("OPTION", session.GenerateIdentity(), $"[OptionItem]Serial: {e.EquipmentSerialId} Type: {value[0]} Value: {value[1]}");
                    if (value[0] != -1)
                    {
                        var cellonOptionDTO = new CellonOptionDTO
                        {
                            EquipmentSerialId = e.EquipmentSerialId,
                            Level = (byte)(dataIndex + 1),
                            Type = (CellonOptionType)value[0],
                            Value = value[1]
                        };

                        e.CellonOptions.Add(cellonOptionDTO);

                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("OPTION_SUCCESS"), e.Rare), 12));
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("OPTION_SUCCESS"), e.Rare), 0));
                        session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3005), session.Character.PositionX, session.Character.PositionY);
                        session.SendPacket("shop_end 1");
                    }
                    else
                    {
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("OPTION_FAIL"), e.Rare), 11));
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("OPTION_FAIL"), e.Rare), 0));
                        session.SendPacket("shop_end 1");
                    }
                    session.Character.Inventory.RemoveItemAmount(cellon.ItemVNum);
                    session.Character.Gold -= goldAmount;
                    session.SendPacket(session.Character.GenerateGold());
                }
            }

            foreach (var effect in e.CellonOptions)
            {
                effect.EquipmentSerialId = e.EquipmentSerialId;
                effect.CellonOptionId = DAOFactory.CellonOptionDAO.InsertOrUpdate(effect).CellonOptionId;
            }
        }

        #endregion
    }
}