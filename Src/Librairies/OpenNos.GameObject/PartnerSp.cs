using System;
using System.Collections.Generic;
using System.Linq;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Data.Enums;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject
{
    public class PartnerSp
    {
        #region Instantiation

        public PartnerSp(ItemInstance instance)
        {
            Instance = instance;

            Initialize();
        }

        #endregion

        #region Properties

        public ItemInstance Instance { get; }

        private List<PartnerSkill> Skills { get; set; }

        private long XpMax => ServerManager.Instance.Configuration.PartnerSpXp;

        #endregion

        #region Methods

        public bool AddSkill(byte castId)
        {
            Skill skill = GetNewSkill(castId);
            short[] vnums =
                {
                1235, 1237, 1239,
                1268, 1269, 1273,
                1292, 1293, 1294,
                1299, 1300, 1301,
                1358, 1359, 1360,
                1368, 1369, 1370,
                1439, 1440, 1441,
                1449, 1450, 1451,
                1490, 1491, 1492,
                1521, 1522, 1523,
                660, 661, 662,
                1236, 1238, 1240,
                1270, 1271, 1275,
                1318, 1319, 1320,
                1371, 1372, 1373,
                1436, 1437, 1438,
                1446, 1447, 1448,
                1628, 1630, 1629,
                1631, 1633, 1632,
                1241, 1242, 1243,
                1272, 1274, 1276,
                1315, 1316, 1317,
                1333, 1334, 1335,
                1379, 1380, 1381,
                1433, 1434, 1435,
                1442, 1443, 1444,
                1602, 1603, 1604,
                782, 784, 783,
            };
            short skillvnum;
            switch (Instance.Item.Morph)
            {
                case 2043:
                    skillvnum = vnums[0 + castId];
                    break;

                case 2046:
                    skillvnum = vnums[3 + castId];
                    break;

                case 2310:
                case 2371:
                    skillvnum = vnums[6 + castId];
                    break;

                case 2317:
                case 2323:
                    skillvnum = vnums[9 + castId];
                    break;

                case 2355:
                case 2709:
                    skillvnum = vnums[12 + castId];
                    break;

                case 2356:
                    skillvnum = vnums[15 + castId];
                    break;

                case 2374:
                    skillvnum = vnums[18 + castId];
                    break;

                case 2378:
                    skillvnum = vnums[21 + castId];
                    break;

                case 2537:
                    skillvnum = vnums[24 + castId];
                    break;

                case 2539:
                    skillvnum = vnums[27 + castId];
                    break;

                case 2731:
                    skillvnum = vnums[30 + castId];
                    break;

                case 2044:
                    skillvnum = vnums[33 + castId];
                    break;

                case 2047:
                    skillvnum = vnums[36 + castId];
                    break;

                case 2343:
                case 2707:
                    skillvnum = vnums[39 + castId];
                    break;

                case 2367:
                case 2708:
                    skillvnum = vnums[42 + castId];
                    break;

                case 2372:
                    skillvnum = vnums[45 + castId];
                    break;

                case 2377:
                    skillvnum = vnums[48 + castId];
                    break;

                case 2716:
                    skillvnum = vnums[51 + castId];
                    break;

                case 2721:
                    skillvnum = vnums[54 + castId];
                    break;

                case 2045:
                    skillvnum = vnums[57 + castId];
                    break;

                case 2048:
                    skillvnum = vnums[60 + castId];
                    break;

                case 2333:
                case 2334:
                    skillvnum = vnums[63 + castId];
                    break;

                case 2325:
                    skillvnum = vnums[66 + castId];
                    break;

                case 2368:
                    skillvnum = vnums[69 + castId];
                    break;

                case 2373:
                    skillvnum = vnums[72 + castId];
                    break;

                case 2376:
                    skillvnum = vnums[75 + castId];
                    break;

                case 2379:
                    skillvnum = vnums[78 + castId];
                    break;

                case 2740:
                    skillvnum = vnums[81 + castId];
                    break;

                default:
                    Logger.Warn($"Partner skill not found (Morph: {Instance.Item.Morph}, CastId: {castId})");
                    return false;
            }

            PartnerSkillDTO partnerSkillDTO = new PartnerSkillDTO
            {
                EquipmentSerialId = Instance.EquipmentSerialId,
                SkillVNum = skillvnum,
                Level = ServerManager.RandomNumber<byte>(1, 8)
            };

            if (DAOFactory.PartnerSkillDAO.Insert(partnerSkillDTO) is PartnerSkillDTO result)
            {
                Skills.Add(new PartnerSkill(result));
                return true;
            }

            Logger.Warn($"Partner skill not found (Morph: {Instance.Item.Morph}, CastId: {castId})");
            return false;
        }

        public void AddXp(long amount)
        {
            if (Instance.XP < XpMax)
            {
                Instance.XP = Math.Min(Instance.XP + amount, XpMax);
            }
        }

        public bool CanLearnSkill() => Instance.XP >= XpMax;

        public void ClearSkills()
        {
            for (byte castId = 0; castId < 3; castId++)
            {
                RemoveSkill(castId);
            }

            ReloadSkills();
        }

        public void FullXp()
        {
            Instance.XP = XpMax;
        }

        public string GeneratePski()
        {
            string pski = "pski";

            foreach (PartnerSkill partnerSkill in Skills.OrderBy(s => s.Skill.CastId))
            {
                pski += $" {partnerSkill.Skill.SkillVNum}";
            }

            return pski;
        }

        public string GenerateSkills(bool withLevel = true, bool justLevels = false)
        {
            string skills = "";

            for (byte castId = 0; castId < 3; castId++)
            {
                PartnerSkill partnerSkill = GetSkill(castId);
                if (justLevels)
                {
                    if (partnerSkill != null)
                        skills += $" {(partnerSkill.Level == 7 ? 4237 + castId : 0)}";
                    else
                        skills += $" 0";

                    continue;
                }
                if (partnerSkill != null)
                {
                    skills += withLevel ? $" {partnerSkill.SkillVNum}.{partnerSkill.Level}" : $" {partnerSkill.SkillVNum}";
                }
                else
                {
                    skills += withLevel ? $" 0.0" : $" 0";
                }
            }

            return skills;
        }

        public int GetCooldown()
        {
            double maxCooldown = 30000;

            foreach (PartnerSkill partnerSkill in Skills.ToList().Where(s => !s.CanBeUsed()))
            {
                double remaining = (partnerSkill.LastUse - DateTime.Now).TotalMilliseconds + (partnerSkill.Skill.Cooldown * 100);

                if (remaining > maxCooldown)
                {
                    maxCooldown = remaining;
                }
            }

            return (int)(maxCooldown / 1000D);
        }

        public string GetName()
        {
            switch (Instance.ItemVNum)
            {
                case 4324: return "Guardian^Lucifer";
                case 4325: return "Sheriff^Chloe";
                case 4326: return "Bone^Warrior^Ragnar";
                case 4343: return "Mad^Professor^Macavity";
                case 4349: return "Archdaemon^Amon";
                case 4405: return "Magic^Student^Yuna";
                case 4413: return "Amora";
                case 4417: return "Mad^March^Hare";
                case 4547: return "Akhenaton^the^Cursed^Pharaoh";
                case 4800: return "Aegir^the^Angry";
                case 4802: return "Barni^the^Clever";
                case 4803: return "Freya^the^Fateful";
                case 4804: return "Shinobi^the^Silent";
                case 4805: return "Lotus^the^Graceful";
                case 4806: return "Orkani^the^Turbulent";
                case 4807: return "Foxy";
                case 4808: return "Maru";
                case 4809: return "Maru^in^Mother's^Fur";
                case 4810: return "Hongbi";
                case 4811: return "Cheongbi";
                case 4812: return "Lucifer";
                case 4813: return "Witch^Laurena";
                case 4814: return "Amon";
                case 4815: return "Lucy^Lopea﻿rs";
                case 4817: return "Cowgirl^Chloe";
                case 4818: return "Fiona";
                case 4819: return "Jinn";
                case 4820: return "Ice^Princess^Eliza";
                case 4821: return "Daniel^Ducats";
                case 4822: return "Palina^Puppet^Master";
                case 4823: return "Harlequin";
                case 4824: return "Nelia^Nymph";
                case 4825: return "Little^Pri﻿ncess^Venus";
            }

            return Instance.Item.Name.Replace(' ', '^');
        }

        public PartnerSkill GetSkill(byte castId)
            => Skills.FirstOrDefault(s => s.Skill.CastId == castId);

        public int GetSkillsCount() => Skills.Count;

        public int GetSkillsLevels() // #PSPBUFF
        {
            int levels = 0;
            Skills.ForEach(p => levels += p.Level);
            levels = levels % 3 == 2 ? levels + 1 : levels;
            return levels;
        }

        public int GetXpPercent() => (int)Math.Floor(100D / XpMax * Instance.XP);

        public void ReloadSkills() => LoadSkills();

        public bool RemoveSkill(byte castId)
        {
            PartnerSkill partnerSkill = GetSkill(castId);

            return partnerSkill != null && DAOFactory.PartnerSkillDAO.Remove(partnerSkill.PartnerSkillId) != DeleteResult.Error;
        }

        public void ResetXp()
        {
            Instance.XP = 0;
        }

        private Skill GetNewSkill(byte castId) => ServerManager.GetAllSkill().FirstOrDefault(s => s.CastId == castId && (s.Class == 28 || s.Class == 29)
                                                            && s.UpgradeType == MateHelper.Instance.GetUpgradeType(Instance.Item.Morph));

        private void Initialize()
        {
            if (Instance.EquipmentSerialId == Guid.Empty)
            {
                Instance.EquipmentSerialId = Guid.NewGuid();
            }

            LoadSkills();
        }

        private void LoadSkills()
        {
            Skills = DAOFactory.PartnerSkillDAO.LoadByEquipmentSerialId(Instance.EquipmentSerialId)
                .Select(partnerSkillDTO => new PartnerSkill(partnerSkillDTO)).ToList();
        }

        #endregion
    }
}