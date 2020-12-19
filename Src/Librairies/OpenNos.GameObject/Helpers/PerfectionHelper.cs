using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject.Helpers
{
    public class PerfectionHelper
    {
        #region Members

        private static PerfectionHelper _instance;

        #endregion

        #region Properties

        public static PerfectionHelper Instance => _instance ?? (_instance = new PerfectionHelper());

        #endregion

        #region Methods

        public void RemovePerfection(ClientSession session, ItemInstance SP, ItemInstance inv = null)
        {
            SP.SpFire = 0;
            SP.SpWater = 0;
            SP.SpLight = 0;
            SP.SpDark = 0;
            SP.SpDamage = 0;
            SP.SpDefence = 0;
            SP.SpHP = 0;
            SP.SpElement = 0;
            SP.SpStoneUpgrade = 0;
            session.SendPacket(SP.GenerateInventoryAdd());
            session.SendPacket(SP.GenerateStashPacket());
            session.SendPacket(SP.GeneratePStash());
            session.CurrentMapInstance.Broadcast(
            StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3003), session.Character.MapX, session.Character.MapY);

            session.SendPacket(session.Character.GenerateSay("SP Perfection points have been reset to 0.", 12));
        }

        public void SpeedPerfection(ClientSession session, ItemInstance SP, ItemInstance inv = null)
        {
            int num, fire, water, light, dark, failure, atk, def, ele, HP;
            num = fire = water = light = dark = failure = atk = def = ele = HP = 0;
            byte flag = 0;
            int rnd;
            short[] upsuccess = {50, 40, 30, 20, 10};
            int[] goldprice = {5000, 10000, 20000, 50000, 100000};
            byte[] stoneprice = {1, 2, 3, 4, 5};
            short stonevnum;
            byte upmode = 1;

            switch (SP.Item.VNum)
            {
                case 901:
                case 905:
                case 908:
                case 911:
                case 4486:
                    stonevnum = 2514;
                    break;

                case 902:
                case 903:
                case 913:
                case 4485:
                    stonevnum = 2515;
                    break;

                case 904:
                case 910:
                case 914:
                case 4532:
                    stonevnum = 2516;
                    break;

                case 906:
                case 909:
                case 912:
                case 4437:
                    stonevnum = 2517;
                    break;

                case 4500:
                case 4501:
                case 4502:
                    stonevnum = 2518;
                    break;

                case 4497:
                case 4498:
                case 4499:
                    stonevnum = 2519;
                    break;

                case 4491:
                case 4492:
                case 4493:
                    stonevnum = 2520;
                    break;

                case 4487:
                case 4488:
                case 4489:
                    stonevnum = 2521;
                    break;

                default:
                    return;
            }

            while (flag == 0)
            {
                upmode = 1;
                if (SP.SpStoneUpgrade > 99)
                {
                    session.SendPacket(session.Character.GenerateSay("Max SP Perfections reached!", 0));
                    flag = 1;
                }
                else if (SP.SpStoneUpgrade > 80)
                {
                    upmode = 5;
                }
                else if (SP.SpStoneUpgrade > 60)
                {
                    upmode = 4;
                }
                else if (SP.SpStoneUpgrade > 40)
                {
                    upmode = 3;
                }
                else if (SP.SpStoneUpgrade > 20)
                {
                    upmode = 2;
                }

                if (SP.IsFixed)
                {
                    session.SendPacket(session.Character.GenerateSay("Sp fixed!", 0));
                    flag = 1;
                }

                if (session.Character.Gold < goldprice[upmode - 1])
                {
                    session.SendPacket(session.Character.GenerateSay("I don't have enough Gold", 0));
                    flag = 1;
                }

                if (session.Character.Inventory.CountItem(stonevnum) < stoneprice[upmode - 1])
                {
                    session.SendPacket(session.Character.GenerateSay("I don't have enough Perfection stones!", 0));
                    flag = 1;
                }

                if (flag == 0)
                {
                    rnd = ServerManager.RandomNumber();
                    if (rnd < upsuccess[upmode - 1])
                    {
                        byte type = (byte) ServerManager.RandomNumber(0, 16), count = 1;
                        var canPerf = true;

                        if (type < 3 && SP.SpDamage >= 50)
                            canPerf = false;
                        else if (type < 6 && SP.SpDefence >= 50)
                            canPerf = false;
                        else if (type < 9 && SP.SpElement >= 50)
                            canPerf = false;
                        else if (type < 12 && SP.SpHP >= 50)
                            canPerf = false;
                        else if (type == 12 && SP.SpFire >= 50)
                            canPerf = false;
                        else if (type == 13 && SP.SpWater >= 50)
                            canPerf = false;
                        else if (type == 14 && SP.SpLight >= 50)
                            canPerf = false;
                        else if (type == 15 && SP.SpDark >= 50) canPerf = false;

                        if (canPerf)
                        {
                            if (upmode == 4) count = 2;
                            if (upmode == 5) count = (byte) ServerManager.RandomNumber(3, 6);

                            if (type < 3)
                            {
                                SP.SpDamage += count;
                                atk += count;
                                session.SendPacket(session.Character.GenerateSay(
                                    string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                                        Language.Instance.GetMessageFromKey("PERFECTSP_ATTACK"), count), 12));
                                session.CurrentMapInstance.Broadcast(
                                    StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId,
                                        3005), session.Character.MapX, session.Character.MapY);
                            }
                            else if (type < 6)
                            {
                                SP.SpDefence += count;
                                def += count;
                                session.SendPacket(session.Character.GenerateSay(
                                    string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                                        Language.Instance.GetMessageFromKey("PERFECTSP_DEFENSE"), count), 12));
                                session.CurrentMapInstance.Broadcast(
                                    StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId,
                                        3005), session.Character.MapX, session.Character.MapY);
                            }
                            else if (type < 9)
                            {
                                SP.SpElement += count;
                                ele += count;
                                session.SendPacket(session.Character.GenerateSay(
                                    string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                                        Language.Instance.GetMessageFromKey("PERFECTSP_ELEMENT"), count), 12));
                                session.CurrentMapInstance.Broadcast(
                                    StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId,
                                        3005), session.Character.MapX, session.Character.MapY);
                            }
                            else if (type < 12)
                            {
                                SP.SpHP += count;
                                HP += count;
                                session.SendPacket(session.Character.GenerateSay(
                                    string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                                        Language.Instance.GetMessageFromKey("PERFECTSP_HPMP"), count), 12));
                                session.CurrentMapInstance.Broadcast(
                                    StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId,
                                        3005), session.Character.MapX, session.Character.MapY);
                                session.CurrentMapInstance.Broadcast(
                                    StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId,
                                        3005), session.Character.MapX, session.Character.MapY);
                            }
                            else if (type == 12)
                            {
                                SP.SpFire += count;
                                fire += count;
                                session.SendPacket(session.Character.GenerateSay(
                                    string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                                        Language.Instance.GetMessageFromKey("PERFECTSP_FIRE"), count), 12));
                                session.CurrentMapInstance.Broadcast(
                                    StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId,
                                        3005), session.Character.MapX, session.Character.MapY);
                            }
                            else if (type == 13)
                            {
                                SP.SpWater += count;
                                water += count;
                                session.SendPacket(session.Character.GenerateSay(
                                    string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                                        Language.Instance.GetMessageFromKey("PERFECTSP_WATER"), count), 12));
                                session.CurrentMapInstance.Broadcast(
                                    StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId,
                                        3005), session.Character.MapX, session.Character.MapY);
                            }
                            else if (type == 14)
                            {
                                SP.SpLight += count;
                                light += count;
                                session.SendPacket(session.Character.GenerateSay(
                                    string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                                        Language.Instance.GetMessageFromKey("PERFECTSP_LIGHT"), count), 12));
                                session.CurrentMapInstance.Broadcast(
                                    StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId,
                                        3005), session.Character.MapX, session.Character.MapY);
                            }
                            else if (type == 15)
                            {
                                SP.SpDark += count;
                                dark += count;
                                session.SendPacket(session.Character.GenerateSay(
                                    string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                                        Language.Instance.GetMessageFromKey("PERFECTSP_SHADOW"), count), 12));
                                session.CurrentMapInstance.Broadcast(
                                    StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId,
                                        3005), session.Character.MapX, session.Character.MapY);
                            }

                            SP.SpStoneUpgrade++;
                            num++;
                        }
                    }
                    else
                    {
                        session.SendPacket(
                            session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PERFECTSP_FAILURE"), 0));
                        failure++;
                    }

                    session.SendPacket(SP.GenerateInventoryAdd());
                    session.Character.Gold -= goldprice[upmode - 1];
                    session.SendPacket(session.Character.GenerateGold());
                    session.Character.Inventory.RemoveItemAmount(stonevnum, stoneprice[upmode - 1]);
                }
            }

            //if (inv != null) return;
            session.SendPacket(session.Character.GenerateSay("--------------- Perfection Results ---------------", 12));
            session.SendPacket(session.Character.GenerateSay("Number of tries : " + (num + failure), 12));
            session.SendPacket(session.Character.GenerateSay("Succeded : " + num, 12));
            session.SendPacket(session.Character.GenerateSay("Failed : " + failure, 12));
            session.SendPacket(session.Character.GenerateSay("---------------------- SLs -----------------------", 12));
            session.SendPacket(session.Character.GenerateSay("Damage points added : +" + atk, 12));
            session.SendPacket(session.Character.GenerateSay("Defence points added : +" + def, 12));
            session.SendPacket(session.Character.GenerateSay("Element points added : +" + ele, 12));
            session.SendPacket(session.Character.GenerateSay("HP/MP points added : +" + HP, 12));
            session.SendPacket(session.Character.GenerateSay("-------------------- Elements --------------------", 12));
            session.SendPacket(session.Character.GenerateSay("Fire points added : +" + fire, 12));
            session.SendPacket(session.Character.GenerateSay("Water points added : +" + water, 12));
            session.SendPacket(session.Character.GenerateSay("Light points added : +" + light, 12));
            session.SendPacket(session.Character.GenerateSay("Darkness points added : +" + dark, 12));
            session.SendPacket(session.Character.GenerateSay("------------------- End Result -------------------", 12));
        }

        #endregion
    }
}