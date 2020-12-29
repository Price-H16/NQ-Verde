using System;
using System.Linq;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;

namespace OpenNos.GameObject.Helpers
{
    public class RewardsHelper
    {
        #region Members

        private static RewardsHelper _instance;

        #endregion

        #region Properties

        public static RewardsHelper Instance => _instance ?? (_instance = new RewardsHelper());

        #endregion

        #region Methods

        public static int ArenaXpReward(byte characterLevel)
        {
            if (characterLevel <= 39)
            {
                // 25%
                return (int)(CharacterHelper.XPData[characterLevel] / 4);
            }

            if (characterLevel <= 55)
            {
                // 20%
                return (int)(CharacterHelper.XPData[characterLevel] / 5);
            }

            if (characterLevel <= 75)
            {
                // 10%
                return (int)(CharacterHelper.XPData[characterLevel] / 10);
            }

            if (characterLevel <= 79)
            {
                // 5%
                return (int)(CharacterHelper.XPData[characterLevel] / 20);
            }

            if (characterLevel <= 85)
            {
                // 2%
                return (int)(CharacterHelper.XPData[characterLevel] / 50);
            }

            if (characterLevel <= 90)
            {
                return (int)(CharacterHelper.XPData[characterLevel] / 80);
            }

            if (characterLevel <= 93)
            {
                return (int)(CharacterHelper.XPData[characterLevel] / 100);
            }

            if (characterLevel <= 99)
            {
                return (int)(CharacterHelper.XPData[characterLevel] / 1000);
            }

            return 0;

        }

        public void DailyReward(ClientSession session)
        {
            var isMartial = session.Character.Class.Equals(ClassType.MartialArtist);
            var count = DAOFactory.GeneralLogDAO.LoadByAccount(session.Account.AccountId).Count(s => s.LogData == (isMartial ? "DAILY_REWARD" : "DAILY_REWARD") && s.Timestamp.Day >= DateTime.Now.Day);

            if (count != 0)
            {
                return;
            }

            // if(DAOFactory.AccountDAO)
            session.Character.GiftAdd((short)(isMartial ? 1043 : 1043), (short)(isMartial ? 1 : 1));
            session.SendPacket(UserInterfaceHelper.GenerateInfo(string.Format(Language.Instance.GetMessageFromKey(isMartial ? "DAILY_REWARD" : "DAILY_REWARD"), 0)));

            session.Character.GeneralLogs.Add(new GeneralLogDTO
            {
                AccountId = session.Account.AccountId,
                CharacterId = session.Character.CharacterId,
                IpAddress = session.IpAddress,
                LogData = isMartial ? "DAILY_REWARD" : "DAILY_REWARD",
                LogType = "World",
                Timestamp = DateTime.Now
            });
        }

        public void MobKillRewards(ClientSession session)
        {
            switch (session.Character.MobKillCounter)
            {
                case 1000: //Conqueror I
                    session.Character.GiftAdd(9378, 1);
                    break;

                case 2500: //Conqueror II
                    session.Character.GiftAdd(9379, 1);
                    break;

                case 4000: //Conqueror III
                    session.Character.GiftAdd(9380, 1);
                    break;

                case 10000: //Conqueror IV
                    session.Character.GiftAdd(9381, 1);
                    break;

                case 20000: //Conqueror V
                    session.Character.GiftAdd(9382, 1);
                    break;


            }
        }

        //public void GetHeroRewards(ClientSession session)
        //{
        //    switch (session.Character.HeroLevel)
        //    {
        //        case 10:

        //            session.SendPacket(UserInterfaceHelper.GenerateInfo(
        //                string.Format(Language.Instance.GetMessageFromKey("HERO_REWARD_LEVEL"), 0)));

        //            session.Character.GiftAdd(1244, 50); // Full pot
        //            session.Character.GiftAdd(2367, 50);
        //            session.Character.GiftAdd(2368, 50);
        //            session.Character.GiftAdd(2345, 5);
        //            session.Character.GiftAdd(1252, 50);
        //            session.Character.GiftAdd(2799, 15);
        //            break;

        //        case 20:

        //            session.SendPacket(UserInterfaceHelper.GenerateInfo(
        //                string.Format(Language.Instance.GetMessageFromKey("HERO_REWARD_LEVEL"), 0)));

        //            session.Character.GiftAdd(1244, 50); // Full pot
        //            session.Character.GiftAdd(2367, 50);
        //            session.Character.GiftAdd(2368, 50);
        //            session.Character.GiftAdd(2345, 5);
        //            session.Character.GiftAdd(1252, 50);
        //            session.Character.GiftAdd(4262, 1);
        //            session.Character.GiftAdd(2799, 15);
        //            break;

        //        case 30:

        //            session.SendPacket(UserInterfaceHelper.GenerateInfo(
        //                string.Format(Language.Instance.GetMessageFromKey("HERO_REWARD_LEVEL"), 0)));

        //            session.Character.GiftAdd(1244, 99); // Full pot
        //            session.Character.GiftAdd(2367, 100);
        //            session.Character.GiftAdd(2368, 100);
        //            session.Character.GiftAdd(2345, 5);
        //            session.Character.GiftAdd(2799, 25);
        //            session.Character.GiftAdd(4262, 1);
        //            session.Character.GiftAdd(5895, 5);
        //            break;

        //        case 40:

        //            session.SendPacket(UserInterfaceHelper.GenerateInfo(
        //                string.Format(Language.Instance.GetMessageFromKey("HERO_REWARD_LEVEL"), 0)));

        //            session.Character.GiftAdd(1244, 99); // Full pot
        //            session.Character.GiftAdd(2367, 100);
        //            session.Character.GiftAdd(2368, 100);
        //            session.Character.GiftAdd(2345, 5);
        //            session.Character.GiftAdd(2799, 25);
        //            session.Character.GiftAdd(4262, 1);
        //            session.Character.GiftAdd(5895, 5);
        //            session.Character.GiftAdd(2339, 5);
        //            break;

        //        case 50:

        //            session.SendPacket(UserInterfaceHelper.GenerateInfo(
        //                string.Format(Language.Instance.GetMessageFromKey("HERO_REWARD_LEVEL"), 0)));

        //            session.Character.GiftAdd(1244, 99); // Full pot
        //            session.Character.GiftAdd(2367, 100);
        //            session.Character.GiftAdd(2368, 100);
        //            session.Character.GiftAdd(2345, 5);
        //            session.Character.GiftAdd(2799, 25);
        //            session.Character.GiftAdd(4262, 1);
        //            session.Character.GiftAdd(5895, 5);
        //            session.Character.GiftAdd(2339, 10);
        //            break;
        //    }
        //}

        //public void GetJobRewards(ClientSession session) // SP Reward
        //{
        //    switch (session.Character.JobLevel)
        //    {
        //        case 20:
        //            session.SendPacket(
        //                UserInterfaceHelper.GenerateInfo(
        //                    string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_SP"), 0)));
        //            switch (session.Character.Class)
        //            {
        //                case ClassType.Swordsman:
        //                    session.Character.GiftAdd(901, 0);
        //                    session.Character.GiftAdd(902, 0);
        //                    session.Character.GiftAdd(909, 0);
        //                    session.Character.GiftAdd(910, 0);
        //                    break;

        //                case ClassType.Archer:
        //                    session.Character.GiftAdd(903, 0);
        //                    session.Character.GiftAdd(904, 0);
        //                    session.Character.GiftAdd(911, 0);
        //                    session.Character.GiftAdd(912, 0);
        //                    break;

        //                case ClassType.Magician:
        //                    session.Character.GiftAdd(905, 0);
        //                    session.Character.GiftAdd(906, 0);
        //                    session.Character.GiftAdd(913, 0);
        //                    session.Character.GiftAdd(914, 0);
        //                    break;
        //            }

        //            break;

        //        case 30:
        //            session.SendPacket(
        //                UserInterfaceHelper.GenerateInfo(
        //                    string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_LVL"), 0)));
        //            switch (session.Character.Class)
        //            {
        //                case ClassType.Swordsman:
        //                    session.Character.GiftAdd(262, 1, 5, 5);
        //                    session.Character.GiftAdd(291, 1, 5, 5);
        //                    session.Character.GiftAdd(102, 1, 5, 5);
        //                    break;

        //                case ClassType.Archer:
        //                    session.Character.GiftAdd(265, 1, 5, 5);
        //                    session.Character.GiftAdd(289, 1, 5, 5);
        //                    session.Character.GiftAdd(115, 1, 5, 5);
        //                    break;

        //                case ClassType.Magician:
        //                    session.Character.GiftAdd(268, 1, 5, 5);
        //                    session.Character.GiftAdd(293, 1, 5, 5);
        //                    session.Character.GiftAdd(128, 1, 5, 5);
        //                    break;
        //            }

        //            break;

        //        case 85:
        //            switch (session.Character.Class)
        //            {
        //                case ClassType.MartialArtist:
        //                    session.Character.GiftAdd(1216, 4);
        //                    session.Character.GiftAdd(1244, 20);
        //                    session.Character.GiftAdd(2345, 2);
        //                    break;
        //            }

        //            break;
        //    }
        //}

        //public void GetLevelUpRewards(ClientSession session) // Auto-reward
        //{
        //    switch (session.Character.Level)
        //    {
        //        case 20:
        //            session.SendPacket(
        //                UserInterfaceHelper.GenerateInfo(
        //                    string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_LVL"), 0)));

        //            session.Character.GiftAdd(1122, 20); // 3K/3K Healing Pots
        //            session.Character.GiftAdd(5370, 1); // Fairy Experience Potion
        //            session.Character.GiftAdd(2168, 1); // Dignity Potion
        //            break;

        //        case 30:
        //            session.SendPacket(
        //                UserInterfaceHelper.GenerateInfo(
        //                    string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_LVL"), 0)));

        //            session.Character.GiftAdd(1122, 20); // 3K/3K healing pots
        //            session.Character.GiftAdd(1452, 1); // Ancelloan's blessing
        //            session.Character.GiftAdd(5370, 1); // Fairy Experience Potion
        //            session.Character.GiftAdd(2168, 1); // Dignity Potion
        //            break;

        //        case 40:
        //            session.SendPacket(
        //                UserInterfaceHelper.GenerateInfo(
        //                    string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_LVL"), 0)));

        //            session.Character.GiftAdd(1122, 20); // 3K/3K healing pots
        //            session.Character.GiftAdd(1452, 1); // ancelloan's blessing
        //            session.Character.GiftAdd(1363, 1); // blue sp scroll
        //            session.Character.GiftAdd(2282, 50); //  WOA
        //            session.Character.GiftAdd(1030, 50); //  WOA
        //            session.Character.GiftAdd(2168, 1); // Dignity Potion
        //            break;

        //        case 50:
        //            session.SendPacket(
        //                UserInterfaceHelper.GenerateInfo(
        //                    string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_LVL"), 0)));

        //            session.Character.GiftAdd(1122, 30); // 3K/3K healing pots
        //            session.Character.GiftAdd(1452, 1); // Ancelloan's blessing
        //            session.Character.GiftAdd(1363, 2); // Blue sp scroll
        //            session.Character.GiftAdd(2282, 50); //  WOA
        //            session.Character.GiftAdd(1030, 50); //  WOA
        //            session.Character.GiftAdd(1218, 2); // equipment scroll
        //            session.Character.GiftAdd(2168, 1); // Dignity Potion
        //            break;

        //        case 60:
        //            session.SendPacket(
        //                UserInterfaceHelper.GenerateInfo(
        //                    string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_LVL"), 0)));

        //            session.Character.GiftAdd(1122, 40); // 3K/3K healing pots
        //            session.Character.GiftAdd(1452, 1); // Ancelloan's blessing
        //            session.Character.GiftAdd(1363, 2); // Blue sp scroll
        //            session.Character.GiftAdd(2282, 50); //  WOA
        //            session.Character.GiftAdd(1030, 50); //  WOA
        //            session.Character.GiftAdd(1218, 3); // Equipment scroll
        //            session.Character.GiftAdd(2168, 1); // Dignity Potion
        //            break;

        //        case 70:
        //            session.SendPacket(
        //                UserInterfaceHelper.GenerateInfo(
        //                    string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_LVL"), 0)));

        //            session.Character.GiftAdd(1244, 15); // Full pot
        //            session.Character.GiftAdd(1452, 2); // Ancelloan's blessing
        //            session.Character.GiftAdd(1363, 5); // Blue sp scroll
        //            session.Character.GiftAdd(1364, 5); // red sp scroll
        //            session.Character.GiftAdd(2282, 100); //  WOA
        //            session.Character.GiftAdd(1030, 70); //  MOONS
        //            session.Character.GiftAdd(2168, 1); // Dignity Potion
        //            session.Character.GiftAdd(573, 1, 6, 0, 0, false, 6); // pvp rune wep
        //            session.Character.GiftAdd(583, 1, 6, 0, 0, false, 6); // pvp rune armor
        //            break;

        //        case 80:
        //            session.SendPacket(
        //                UserInterfaceHelper.GenerateInfo(
        //                    string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_LVL"), 0)));

        //            session.Character.GiftAdd(1244, 25); // Full pots
        //            session.Character.GiftAdd(1452, 3); // Ancelloan's blessing
        //            session.Character.GiftAdd(1364, 5); // red sp scroll
        //            session.Character.GiftAdd(1364, 5); // red sp scroll
        //            session.Character.GiftAdd(2282, 100); //  WOA
        //            session.Character.GiftAdd(1030, 80); //  moons
        //            session.Character.GiftAdd(282, 1); // betting amulet
        //            session.Character.GiftAdd(573, 1, 6, 0, 0, false, 6); // pvp rune wep
        //            session.Character.GiftAdd(583, 1, 6, 0, 0, false, 6); // pvp rune armor
        //            break;

        //        case 90:
        //            session.SendPacket(
        //                UserInterfaceHelper.GenerateInfo(
        //                    string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_LVL"), 0)));

        //            session.Character.GiftAdd(1244, 50); // Full pot
        //            session.Character.GiftAdd(1452, 5); // Ancelloan's blessing
        //            session.Character.GiftAdd(1363, 7); // red sp scroll
        //            session.Character.GiftAdd(1364, 7); // red sp scroll
        //            session.Character.GiftAdd(1030, 100); //  MOONS
        //            session.Character.GiftAdd(2282, 200); //  WOA
        //            session.Character.GiftAdd(282, 1); // betting amulet
        //            break;

        //        case 100:
        //            session.SendPacket(
        //                UserInterfaceHelper.GenerateInfo(
        //                    string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_LVL"), 0)));
        //            session.Character.GiftAdd(5009, 1); // Random Box With One Pet.
        //            session.Character.GiftAdd(1244, 99); // Full pot
        //            session.Character.GiftAdd(1218, 20);
        //            session.Character.GiftAdd(5369, 10);
        //            session.Character.GiftAdd(1363, 10); // blue sp scroll
        //            session.Character.GiftAdd(1364, 10); // red sp scroll
        //            session.Character.GiftAdd(573, 1, 7, 0, 0, false, 7); // pvp rune wep
        //            session.Character.GiftAdd(583, 1, 7, 0, 0, false, 7); // pvp rune armor
        //            break;

        //        case 105:
        //            session.SendPacket(
        //                UserInterfaceHelper.GenerateInfo(
        //                    string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_LVL"), 0)));
        //            session.Character.GiftAdd(1244, 99); // Full pot
        //            session.Character.GiftAdd(1218, 20);
        //            session.Character.GiftAdd(5369, 10);
        //            session.Character.GiftAdd(1363, 10); // blue sp scroll
        //            session.Character.GiftAdd(1364, 10); // red sp scroll
        //            break;

        //        case 110:
        //            session.SendPacket(
        //                UserInterfaceHelper.GenerateInfo(
        //                    string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_LVL"), 0)));
        //            session.Character.GiftAdd(1244, 99); // Full pot
        //            session.Character.GiftAdd(1218, 20);
        //            session.Character.GiftAdd(5369, 10);
        //            session.Character.GiftAdd(2345, 2);
        //            session.Character.GiftAdd(1363, 10); // blue sp scroll
        //            session.Character.GiftAdd(1364, 10); // red sp scroll
        //            session.Character.GiftAdd(573, 1, 7, 0, 0, false, 7); // pvp rune wep
        //            session.Character.GiftAdd(583, 1, 7, 0, 0, false, 7); // pvp rune armor
        //            break;

        //        case 115:
        //            session.SendPacket(
        //                UserInterfaceHelper.GenerateInfo(
        //                    string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_LVL"), 0)));
        //            session.Character.GiftAdd(1244, 99); // Full pot
        //            session.Character.GiftAdd(1218, 20);
        //            session.Character.GiftAdd(5369, 10);
        //            session.Character.GiftAdd(2345, 3);
        //            session.Character.GiftAdd(1363, 15); // blue sp scroll
        //            session.Character.GiftAdd(1364, 15); // red sp scroll
        //            break;

        //        case 120:
        //            session.SendPacket(
        //                UserInterfaceHelper.GenerateInfo(
        //                    string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_LVL"), 0)));
        //            session.Character.GiftAdd(1244, 99); // Full pot
        //            session.Character.GiftAdd(1218, 20);
        //            session.Character.GiftAdd(2345, 5);
        //            session.Character.GiftAdd(5369, 10);
        //            session.Character.GiftAdd(2799, 15);
        //            session.Character.GiftAdd(1363, 20); // blue sp scroll
        //            session.Character.GiftAdd(1364, 20); // red sp scroll
        //            session.Character.GiftAdd(573, 1, 7, 0, 0, false, 7); // pvp rune wep
        //            session.Character.GiftAdd(583, 1, 7, 0, 0, false, 7); // pvp rune armor
        //            break;

        //        case 130:
        //            session.SendPacket(
        //                UserInterfaceHelper.GenerateInfo(
        //                    string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_LVL"), 0)));
        //            session.Character.GiftAdd(1244, 99); // Full pot
        //            session.Character.GiftAdd(2367, 100);
        //            session.Character.GiftAdd(2368, 100);
        //            session.Character.GiftAdd(2345, 5);
        //            session.Character.GiftAdd(1363, 10); // blue sp scroll
        //            session.Character.GiftAdd(1364, 10); // red sp scroll
        //            session.Character.GiftAdd(2799, 25);
        //            session.Character.GiftAdd(573, 1, 7, 0, 0, false, 7); // pvp rune wep
        //            session.Character.GiftAdd(583, 1, 7, 0, 0, false, 7); // pvp rune armor
        //            break;

        //        case 140: //Level Max
        //            session.SendPacket(
        //                UserInterfaceHelper.GenerateInfo(
        //                    string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_LVL"), 0)));
        //            session.Character.GiftAdd(2367, 100);
        //            session.Character.GiftAdd(2368, 100);
        //            session.Character.GiftAdd(4262, 1);
        //            session.Character.GiftAdd(2345, 5);
        //            session.Character.GiftAdd(2799, 20);
        //            session.Character.GiftAdd(573, 1, 7, 0, 0, false, 7); // pvp rune wep
        //            session.Character.GiftAdd(583, 1, 7, 0, 0, false, 7); // pvp rune armor
        //            break;

        //        case 150: //Level Max
        //            session.SendPacket(UserInterfaceHelper.GenerateInfo(
        //                string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_LVL_MAXIMUM"), 0)));
        //            session.Character.GiftAdd(2367, 100);
        //            session.Character.GiftAdd(2368, 100);
        //            session.Character.GiftAdd(4262, 1);
        //            session.Character.GiftAdd(2799, 25);
        //            session.Character.GiftAdd(5859, 1);
        //            session.Character.GiftAdd(9338, 1); //title PvP Legend
        //            session.Character.GiftAdd(573, 1, 7, 0, 0, false, 7); // pvp rune wep
        //            session.Character.GiftAdd(583, 1, 7, 0, 0, false, 7); // pvp rune armor
        //            break;
        //    }
        //}

        //public void MartialRewards(ClientSession session) // TODO
        //{
        //    switch (session.Character.Level)
        //    {
        //        case 82:
        //            switch (session.Character.Class)
        //            {
        //                case ClassType.MartialArtist:
        //                    session.Character.GiftAdd(1216, 4);
        //                    session.Character.GiftAdd(1244, 20);
        //                    session.SendPacket(UserInterfaceHelper.GenerateInfo(
        //                        string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_MARTIAL"), 0)));
        //                    break;
        //            }

        //            break;

        //        case 90:
        //            switch (session.Character.Class)
        //            {
        //                case ClassType.MartialArtist:
        //                    session.Character.GiftAdd(1244, 20);
        //                    session.Character.GiftAdd(1012, 50);
        //                    session.Character.GiftAdd(1296, 1);
        //                    session.Character.GiftAdd(1286, 1);
        //                    session.Character.GiftAdd(1326, 6);
        //                    session.Character.GiftAdd(1397, 6);
        //                    session.Character.GiftAdd(1252, 50);

        //                    session.SendPacket(UserInterfaceHelper.GenerateInfo(
        //                        string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_MARTIAL"), 0)));
        //                    break;
        //            }

        //            break;

        //        case 100:
        //            switch (session.Character.Class)
        //            {
        //                case ClassType.MartialArtist:
        //                    session.Character.GiftAdd(1244, 30);
        //                    session.Character.GiftAdd(1012, 50);
        //                    session.Character.GiftAdd(1296, 1);
        //                    session.Character.GiftAdd(1286, 1);
        //                    session.Character.GiftAdd(2209, 8);
        //                    session.Character.GiftAdd(1207, 8);
        //                    session.Character.GiftAdd(2367, 25);
        //                    session.Character.GiftAdd(2368, 25);

        //                    session.SendPacket(UserInterfaceHelper.GenerateInfo(
        //                        string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_MARTIAL"), 0)));
        //                    break;
        //            }

        //            break;

        //        case 110:
        //            switch (session.Character.Class)
        //            {
        //                case ClassType.MartialArtist:
        //                    session.Character.GiftAdd(1244, 40);
        //                    session.Character.GiftAdd(1012, 50);
        //                    session.Character.GiftAdd(1252, 50);
        //                    session.Character.GiftAdd(5507, 7);
        //                    session.Character.GiftAdd(2448, 7);
        //                    session.Character.GiftAdd(2799, 10);
        //                    session.Character.GiftAdd(2367, 25);
        //                    session.Character.GiftAdd(2368, 25);

        //                    session.SendPacket(UserInterfaceHelper.GenerateInfo(
        //                        string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_MARTIAL"), 0)));
        //                    break;
        //            }

        //            break;

        //        case 120:
        //            switch (session.Character.Class)
        //            {
        //                case ClassType.MartialArtist:
        //                    session.Character.GiftAdd(1244, 40);
        //                    session.Character.GiftAdd(1012, 50);
        //                    session.Character.GiftAdd(1252, 80);
        //                    session.Character.GiftAdd(2209, 8);
        //                    session.Character.GiftAdd(4262, 1);
        //                    session.Character.GiftAdd(8316, 1);
        //                    session.Character.GiftAdd(2799, 10);
        //                    session.Character.GiftAdd(2367, 20);
        //                    session.Character.GiftAdd(2368, 20);

        //                    session.SendPacket(UserInterfaceHelper.GenerateInfo(
        //                        string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_MARTIAL"), 0)));
        //                    break;
        //            }

        //            break;

        //        case 130:
        //            switch (session.Character.Class)
        //            {
        //                case ClassType.MartialArtist:
        //                    session.Character.GiftAdd(4485, 1); //Sp2
        //                    session.Character.GiftAdd(1244, 40);
        //                    session.Character.GiftAdd(1012, 50);
        //                    session.Character.GiftAdd(4262, 1);
        //                    session.Character.GiftAdd(8318, 1);
        //                    session.Character.GiftAdd(2799, 20);
        //                    session.Character.GiftAdd(2367, 25);
        //                    session.Character.GiftAdd(2368, 25);

        //                    session.SendPacket(UserInterfaceHelper.GenerateInfo(
        //                        string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_MARTIAL"), 0)));
        //                    break;
        //            }

        //            break;

        //        case 140:
        //            switch (session.Character.Class)
        //            {
        //                case ClassType.MartialArtist:
        //                    session.Character.GiftAdd(4437, 1); //Sp3
        //                    session.Character.GiftAdd(1244, 40);
        //                    session.Character.GiftAdd(4262, 1);
        //                    session.Character.GiftAdd(8317, 1);
        //                    session.Character.GiftAdd(5895, 5);
        //                    session.Character.GiftAdd(2799, 20);
        //                    session.Character.GiftAdd(2367, 50);
        //                    session.Character.GiftAdd(2368, 50);

        //                    session.SendPacket(UserInterfaceHelper.GenerateInfo(
        //                        string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_MARTIAL"), 0)));
        //                    break;
        //            }

        //            break;

        //        case 150:
        //            switch (session.Character.Class)
        //            {
        //                case ClassType.MartialArtist:
        //                    session.Character.GiftAdd(1244, 40);
        //                    session.Character.GiftAdd(4262, 1);
        //                    session.Character.GiftAdd(8319, 1);
        //                    session.Character.GiftAdd(5895, 10);
        //                    session.Character.GiftAdd(2367, 100);
        //                    session.Character.GiftAdd(2368, 100);
        //                    session.Character.GiftAdd(5859, 1);
        //                    session.Character.GiftAdd(9321, 1); //title Enter The Dragon

        //                    session.SendPacket(UserInterfaceHelper.GenerateInfo(
        //                        string.Format(Language.Instance.GetMessageFromKey("AUTO_REWARD_LVL_MAXIMUM"), 0)));
        //                    break;
        //            }

        //            break;
        //}
        //}

        #endregion
    }
}