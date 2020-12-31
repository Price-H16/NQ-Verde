
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using NosTale.Configuration.Utilities;
using NosTale.Configuration;

namespace OpenNos.GameObject.Extension.Inventory
{
    public static class PerfectSpExtension
    {
        #region Methods

        public static void PerfectSp(this ItemInstance e, ClientSession session)
        {
            var conf = DependencyContainer.Instance.GetInstance<JsonItemConfiguration>().PerfectSp;

            short stonevnum;
            byte upmode = 1;

            switch ((SpecialistMorphType)e.Item.Morph)
            {
                case SpecialistMorphType.Warrior:
                case SpecialistMorphType.RedMage:
                case SpecialistMorphType.Jajamaru:
                case SpecialistMorphType.BombArtificer:
                case SpecialistMorphType.Drakenfer:
                    stonevnum = 2514;
                    break;

                case SpecialistMorphType.Ninja:
                case SpecialistMorphType.Ranger:
                case SpecialistMorphType.IceMage:
                case SpecialistMorphType.MysticalArts:
                    stonevnum = 2515;
                    break;

                case SpecialistMorphType.Assassin:
                case SpecialistMorphType.Berserker:
                case SpecialistMorphType.DarkGunner:
                case SpecialistMorphType.DarkAM:
                    stonevnum = 2516;
                    break;

                case SpecialistMorphType.Crusader:
                case SpecialistMorphType.WildKeeper:
                case SpecialistMorphType.HolyMage:
                case SpecialistMorphType.WolfMaster:
                    stonevnum = 2517;
                    break;

                case SpecialistMorphType.Gladiator:
                case SpecialistMorphType.Canoneer:
                case SpecialistMorphType.Volcanor:
                    stonevnum = 2518;
                    break;

                case SpecialistMorphType.BlueMonk:
                case SpecialistMorphType.Scout:
                case SpecialistMorphType.TideLord:
                    stonevnum = 2519;
                    break;

                case SpecialistMorphType.DeathRipper:
                case SpecialistMorphType.DemonHunter:
                case SpecialistMorphType.Seer:
                    stonevnum = 2520;
                    break;

                case SpecialistMorphType.Renegade:
                case SpecialistMorphType.AvengingAngel:
                case SpecialistMorphType.ArchMage:
                    stonevnum = 2521;
                    break;

                default:
                    return;
            }

            if (e.SpStoneUpgrade > 99) return;

            if (e.SpStoneUpgrade > 80)
                upmode = 5;
            else if (e.SpStoneUpgrade > 60)
                upmode = 4;
            else if (e.SpStoneUpgrade > 40)
                upmode = 3;
            else if (e.SpStoneUpgrade > 20) upmode = 2;

            if (e.IsFixed) return;

            if (session.Character.Gold < conf.GoldPrice[upmode - 1]) return;

            if (session.Character.Inventory.CountItem(stonevnum) < conf.StonePrice[upmode - 1]) return;

            var specialist = session.Character.Inventory.GetItemInstanceById(e.Id);
            var rnd = ServerManager.RandomNumber();
            if (rnd < conf.UpSuccess[upmode - 1]
            ) // + DependencyContainer.Instance.Get<JsonGameConfiguration>().DefaultEvent.PerfectionSp)
            {
                byte type = (byte)ServerManager.RandomNumber(0, 16), count = 1;
                switch (upmode)
                {
                    case 4:
                        count = 2;
                        break;

                    case 5:
                        count = (byte)ServerManager.RandomNumber(3, 6);
                        break;
                }

                session.CurrentMapInstance.Broadcast(
                    StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3005),
                    session.Character.MapX, session.Character.MapY);

                if (type < 3)
                {
                    specialist.SpDamage += count;
                    session.SendPacket(session.Character.GenerateSay(
                        string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                            Language.Instance.GetMessageFromKey("PERFECTSP_ATTACK"), count), 12));
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                            Language.Instance.GetMessageFromKey("PERFECTSP_ATTACK"), count), 0));
                }
                else if (type < 6)
                {
                    specialist.SpDefence += count;
                    session.SendPacket(session.Character.GenerateSay(
                        string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                            Language.Instance.GetMessageFromKey("PERFECTSP_DEFENSE"), count), 12));
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                            Language.Instance.GetMessageFromKey("PERFECTSP_DEFENSE"), count), 0));
                }
                else if (type < 9)
                {
                    specialist.SpElement += count;
                    session.SendPacket(session.Character.GenerateSay(
                        string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                            Language.Instance.GetMessageFromKey("PERFECTSP_ELEMENT"), count), 12));
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                            Language.Instance.GetMessageFromKey("PERFECTSP_ELEMENT"), count), 0));
                }
                else if (type < 12)
                {
                    specialist.SpHP += count;
                    session.SendPacket(session.Character.GenerateSay(
                        string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                            Language.Instance.GetMessageFromKey("PERFECTSP_HPMP"), count), 12));
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                            Language.Instance.GetMessageFromKey("PERFECTSP_HPMP"), count), 0));
                }
                else if (type == 12)
                {
                    specialist.SpFire += count;
                    session.SendPacket(session.Character.GenerateSay(
                        string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                            Language.Instance.GetMessageFromKey("PERFECTSP_FIRE"), count), 12));
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                            Language.Instance.GetMessageFromKey("PERFECTSP_FIRE"), count), 0));
                }
                else if (type == 13)
                {
                    specialist.SpWater += count;
                    session.SendPacket(session.Character.GenerateSay(
                        string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                            Language.Instance.GetMessageFromKey("PERFECTSP_WATER"), count), 12));
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                            Language.Instance.GetMessageFromKey("PERFECTSP_WATER"), count), 0));
                }
                else if (type == 14)
                {
                    specialist.SpLight += count;
                    session.SendPacket(session.Character.GenerateSay(
                        string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                            Language.Instance.GetMessageFromKey("PERFECTSP_LIGHT"), count), 12));
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                            Language.Instance.GetMessageFromKey("PERFECTSP_LIGHT"), count), 0));
                }
                else if (type == 15)
                {
                    specialist.SpDark += count;
                    session.SendPacket(session.Character.GenerateSay(
                        string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                            Language.Instance.GetMessageFromKey("PERFECTSP_SHADOW"), count), 12));
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                            Language.Instance.GetMessageFromKey("PERFECTSP_SHADOW"), count), 0));
                }

                session.SendPacket(session.Character.GenerateSay(
                    string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                        Language.Instance.GetMessageFromKey("PERFECTSP_WATER"), count), 12));
                session.SendPacket(UserInterfaceHelper.GenerateMsg(
                    string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"),
                        Language.Instance.GetMessageFromKey("PERFECTSP_WATER"), count), 0));
                e.SpStoneUpgrade++;
            }
            else
            {
                session.SendPacket(
                    session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PERFECTSP_FAILURE"), 11));
                session.SendPacket(
                    UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("PERFECTSP_FAILURE"), 0));
            }

            session.SendPacket(specialist.GenerateInventoryAdd());
            session.GoldLess(conf.GoldPrice[upmode - 1]);
            session.Character.Inventory.RemoveItemAmount(stonevnum, conf.StonePrice[upmode - 1]);
            session.SendPacket("shop_end 1");
        }

        #endregion
    }
}