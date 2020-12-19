using System;
using System.Collections.Generic;
using OpenNos.Domain;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject.Helpers
{
    public class MateHelper
    {
        #region Instantiation

        public MateHelper()
        {
            LoadConcentrate();
            LoadHpData();
            LoadMinDamageData();
            LoadMaxDamageData();
            LoadMpData();
            LoadXpData();
            LoadDefences();
            LoadPetSkills();
            LoadMateBuffs();
            LoadTrainerUpgradeHits();
            LoadTrainerUpRate();
            LoadTrainerDownRate();
        }

        #endregion

        #region Members

        #endregion

        #region Properties

        public short[,] MeleeDefenseData { get; private set; }

        public short[,] RangeDefenseData { get; private set; }

        public short[,] MagicDefenseData { get; private set; }

        public short[,] MeleeDefenseDodgeData { get; private set; }

        public short[,] RangeDefenseDodgeData { get; private set; }

        public short[] TrainerUpgradeHits { get; private set; }

        public short[] TrainerUpRate { get; private set; }

        public short[] TrainerDownRate { get; private set; }

        public short[,] Concentrate { get; private set; }

        public short[,] HpData { get; private set; }
        
        public short[,] MpData { get; private set; }

        public short[,] MinDamageData { get; private set; }

        public short[,] MaxDamageData { get; private set; }
        
        public double[] XpData { get; private set; }

        // Vnum - CardId
        public Dictionary<int, int> MateBuffs { get; set; }

        public Dictionary<int, int> PartnerSpBuffs { get; set; }

        public List<int> PetSkills { get; set; }

        #endregion

        #region Methods

        public void AddPartnerSkill(ClientSession session, Mate mate)
        {
            if (session == null || mate == null)
            {
                return;
            }

            if (mate.MateType != MateType.Partner)
            {
                return;
            }

            NpcMonster mateNpc = ServerManager.GetNpcMonster(mate.NpcMonsterVNum);
            if (mateNpc.Skills != null)
            {
                foreach (NpcMonsterSkill skill in mateNpc.Skills)
                {
                    if (mate.Level >= skill.Skill.LevelMinimum)
                    {
                        CharacterSkill characterSkillMate = new CharacterSkill
                        {
                            SkillVNum = skill.SkillVNum,
                            CharacterId = session.Character.CharacterId
                        };
                        session.Character.Skills[skill.SkillVNum] = new CharacterSkill(characterSkillMate);
                    }
                }
            }

            session.SendPacket(session.Character.GenerateSki());
            session.SendPackets(session.Character.GenerateQuicklist());
            session.SendPacket(session.Character.GenerateStat());
            session.SendPackets(session.Character.GenerateStatChar());
            session.CurrentMapInstance?.Broadcast(session.Character.GenerateEq());
        }


        #region PartnerSPSkills

        public short GetUpgradeType(short morph)
        {
            switch (morph)
            {
                case 2043:
                    return 1;
                case 2044:
                    return 2;
                case 2045:
                    return 3;
                case 2046:
                    return 4;
                case 2047:
                    return 5;
                case 2048:
                    return 6;
                case 2310:
                    return 7;
                case 2317:
                    return 8;
                case 2323:
                    return 9;
                case 2325:
                    return 10;
                case 2333:
                    return 11;
                case 2334:
                    return 12;
                case 2343:
                    return 13;
                case 2355:
                    return 14;
                case 2356:
                    return 15;
                case 2367:
                    return 16;
                case 2368:
                    return 17;
                case 2371:
                    return 18;
                case 2372:
                    return 19;
                case 2373:
                    return 20;
                case 2374:
                    return 21;
                case 2376:
                    return 22;
                case 2377:
                    return 23;
                case 2378:
                    return 24;
                case 2537:
                    return 25;
                case 2538:
                    return 26;
            }
            return -1;
        }

        #endregion
        
        #region Concentrate

        public void LoadConcentrate()
        {
            Concentrate = new short[3, 100];

            int baseValue = 0;

            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        baseValue = 424;
                        break;
                    case 1:
                        baseValue = 702;
                        break;
                    case 2:
                        baseValue = 702;
                        break;
                }

                for (int j = 0; j < 100; j++)
                {
                    Concentrate[i, j] = (short)(baseValue * j / 100);
                }
            }
        }

        #endregion

        #region HP

        public void LoadHpData()
        {
            HpData = new short[3, 100];

            int baseValue = 0;

            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        baseValue = 22628;
                        break;
                    case 1:
                        baseValue = 12728;
                        break;
                    case 2:
                        baseValue = 32573;
                        break;
                }

                for (int j = 0; j < 100; j++)
                {
                    HpData[i, j] = (short)(baseValue * j / 100);
                }
            }
        }

        #endregion

        #region Damage
        
        public void LoadMinDamageData()
        {
            MinDamageData = new short[3, 151];

            int baseValue = 0;

            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        baseValue = 2200;
                        break;
                    case 1:
                        baseValue = 1900;
                        break;
                    case 2:
                        baseValue = 1200;
                        break;
                }

                for (int j = 0; j < 100; j++)
                {
                    MinDamageData[i, j] = (short)(baseValue * j / 100);
                }
            }
        }

        public void LoadMaxDamageData()
        {
            MaxDamageData = new short[3, 151];

            int baseValue = 0;

            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        baseValue = 4000;
                        break;
                    case 1:
                        baseValue = 3000;
                        break;
                    case 2:
                        baseValue = 2500;
                        break;
                }

                for (int j = 0; j < 100; j++)
                {
                    MaxDamageData[i, j] = (short)(baseValue * j / 100);
                }
            }
        }

        #endregion

        #region List

        public void LoadPetSkills()
        {
            PetSkills = new List<int>
            {
                1513, // Purcival
                1514, // Baron scratch ?
                1515, // Amiral (le chat chelou)
                1516, // roi des pirates pussifer
                1524, // Miaou fou
                1575, // Marié Bouhmiaou
                1576, // Marie Bouhmiaou
                1601, // Mechamiaou
                1627, // Boris the polar bear
                663, // Otter
                740,
                743
            };
        }

        private void LoadMateBuffs()
        {
            MateBuffs = new Dictionary<int, int>
            {   //Monster / Buff
                //{501, 4066}, // Justin
                //{500, 4067}, // Kupei
                //{503, 4068}, // Felix
                //{501, 4066}, // Seina
                //{500, 4067}, // Daisy
                //{503, 4068}, // Whitney
                //{439, 4062}, // Revenant skeleton
                //{2521, 4063}, // Sentinel
                //{2525, 4064}, // Spearman
                //{317, 4048}, // BOB CUSTOM
                //{318, 4049}, // TOM  CUSTOM
                //{319, 4050}, // KLIFF  CUSTOM
                {536, 162}, // LUCKY PIG
                {178, 108}, // LUCKY PIG
                {670, 374}, // FIBI
                {829, 377}, // RUDY ROWDY
                {836, 381}, // FLUFFY BALLY
                {838, 385}, // NAVY BUSHTAIL
                {840, 442}, // LEO THE COWARD
                {841, 394}, // RATUFU NINJA
                {842, 399}, // INDIAN BUSHTAIL
                {843, 403}, // VIKING BUSHTAIL
                {844, 391}, // COWBOY BUSHTAIL
                {997, 825}, // DEATH LANCER
                {1456, 828}, // SPIRIT KING AVATAR
                {1470, 827}, // BEAST KING AVATAR
                {682, 1011}, // RAINBOW PEGASUS
                {671,  687}, // MINI FIRE DEVIL
                {2105, 383}, // INFERNO
                {683, 742}, // OTTER
                {2708, 708}, // MARCO
                {2704, 710}, // FORTUNE BUSHTAIL
                {2709, 711}, // SUPER FORTUNE BUSHTAIL
                {2710, 783}, // MR TRIKLES
                {709, 840}, // GOLDEN GLOOPY CAKE
                {687, 842} // GLOOPY CAKE
            };

            PartnerSpBuffs = new Dictionary<int, int>
            {
                { 4825, 3000 },
                { 8247, 3000 }, //VENUS 
                { 4326, 3007 },
                { 8300, 3007 }, //BONE WARRIOR
                { 4405, 3014 },
                { 8367, 3014 }, // YUNA
                { 4413, 3021 },
                { 8375, 3021 }, // AMORA
                { 4446, 3028 },
                { 8398, 3028 }, // PERTI
                { 4547, 3035 }  //  AKHENATON
            };
        }

        #endregion

        #region MP

        public void LoadMpData()
        {
            MpData = new short[3, 100];

            int baseValue = 0;

            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        baseValue = 7071;
                        break;
                    case 1:
                        baseValue = 9900;
                        break;
                    case 2:
                        baseValue = 18385;
                        break;
                }

                for (int j = 0; j < 100; j++)
                {
                    MpData[i, j] = (short)(baseValue * j / 100);
                }
            }
        }

        #endregion

        #region XP

        public void LoadXpData()
        {
            // Load XpData
            XpData = new double[256];
            double[] v = new double[256];
            double var = 1;
            v[0] = 540;
            v[1] = 960;
            XpData[0] = 300;
            for (int i = 2; i < v.Length; i++)
            {
                v[i] = v[i - 1] + 420 + 120 * (i - 1);
            }

            for (int i = 1; i < XpData.Length; i++)
            {
                if (i < 79)
                {
                    switch (i)
                    {
                        case 14:
                            var = 6 / 3d;
                            break;
                        case 39:
                            var = 19 / 3d;
                            break;
                        case 59:
                            var = 70 / 3d;
                            break;
                    }

                    XpData[i] = Convert.ToInt64(XpData[i - 1] + var * v[i - 1]);
                }

                if (i < 79)
                {
                    continue;
                }

                switch (i)
                {
                    case 79:
                        var = 5000;
                        break;
                    case 82:
                        var = 9000;
                        break;
                    case 84:
                        var = 13000;
                        break;
                }

                XpData[i] = Convert.ToInt64(XpData[i - 1] + var * (i + 2) * (i + 2));
            }
        }

        #endregion

        #region Defences
        
        public void LoadDefences()
        {
            MeleeDefenseData = new short[3, 151];
            int baseValue = 0;
            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        baseValue = 640;
                        break;

                    case 1:
                        baseValue = 682;
                        break;

                    case 2:
                        baseValue = 520;
                        break;
                }

                for (int j = 0; j < 100; j++)
                {
                    MeleeDefenseData[i, j] = (short)(baseValue * j / 100);
                }
            }

            RangeDefenseData = new short[3, 100];
            baseValue = 0;
            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        baseValue = 612;
                        break;
                    case 1:
                        baseValue = 510;
                        break;
                    case 2:
                        baseValue = 425;
                        break;
                }

                for (int j = 0; j < 100; j++)
                {
                    RangeDefenseData[i, j] = (short)(baseValue * j / 100);
                }
            }

            MagicDefenseData = new short[3, 100];
            baseValue = 0;
            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        baseValue = 510;
                        break;
                    case 1:
                        baseValue = 425;
                        break;
                    case 2:
                        baseValue = 680;
                        break;
                }

                for (int j = 0; j < 100; j++)
                {
                    MagicDefenseData[i, j] = (short)(baseValue * j / 100);
                }
            }

            MeleeDefenseDodgeData = new short[3, 100];
            baseValue = 0;
            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        baseValue = 540;
                        break;
                    case 1:
                        baseValue = 945;
                        break;
                    case 2:
                        baseValue = 475;
                        break;
                }

                for (int j = 0; j < 100; j++)
                {
                    MeleeDefenseDodgeData[i, j] = (short)(baseValue * j / 100);
                }
            }

            RangeDefenseDodgeData = new short[3, 100];
            baseValue = 0;
            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        baseValue = 540;
                        break;
                    case 1:
                        baseValue = 945;
                        break;
                    case 2:
                        baseValue = 475;
                        break;
                }

                for (int j = 0; j < 100; j++)
                {
                    RangeDefenseDodgeData[i, j] = (short)(baseValue * j / 100);
                }
            }
        }

        #endregion

        #region Trainers

        public void LoadTrainerUpgradeHits()
        {
            TrainerUpgradeHits = new short[10];

            short baseValue = 0;

            for (int i = 0; i < 10; i++)
            {
                baseValue += 50;
                TrainerUpgradeHits[i] = baseValue;
            }
        }
        
        public void LoadTrainerUpRate()
        {
            TrainerUpRate = new short[] { 67, 67, 44, 34, 22, 15, 14, 8, 1, 0 };
        }

        public void LoadTrainerDownRate()
        {
            TrainerDownRate = new short[] { 0, 7, 13, 16, 28, 29, 33, 36, 50, 60 };
        }

        #endregion

        #endregion

        #region Singleton

        private static MateHelper _instance;

        public static MateHelper Instance => _instance ?? (_instance = new MateHelper());

        #endregion
    }
}
