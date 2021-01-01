using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CharacterState = OpenNos.Domain.CharacterState;
using GenderType = OpenNos.Domain.GenderType;
using HairColorType = OpenNos.Domain.HairColorType;
using HairStyleType = OpenNos.Domain.HairStyleType;

namespace NosTale.Extension.GameExtension.Character
{
    public static class CharacterExt
    {
        #region Methods

        public static bool CanCreateCharacter(this ClientSession Session, byte slot, string characterName)
        {
            if (slot > 3 || DAOFactory.CharacterDAO.LoadBySlot(Session.Account.AccountId, slot) != null)
            {
                return false;
            }

            if (characterName.Length <= 4 || characterName.Length >= 15)
            {
                return false;
            }

            var rg = new Regex(@"^[A-Za-z0-9_äÄöÖüÜß~*<>°+-.!_-Ð™¤£±†‡×ßø^\u0021-\u007E\u00A1-\u00AC\u00AE-\u00FF\u4E00-\u9FA5\u0E01-\u0E3A\u0E3F-\u0E5B\u002E]*$");

            //@"^[\u0021-\u007E\u00A1-\u00AC\u00AE-\u00FF\u4E00-\u9FA5\u0E01-\u0E3A\u0E3F-\u0E5B\u002E]*$");

            if (rg.Matches(characterName).Count != 1)
            {
                Session.SendPacketFormat($"info {Language.Instance.GetMessageFromKey("INVALID_CHARNAME")}");
                return false;
            }

            if (DAOFactory.CharacterDAO.LoadByName(characterName) != null)
            {
                Session.SendPacketFormat($"info {Language.Instance.GetMessageFromKey("ALREADY_TAKEN")}");
                return false;
            }

            var BlackListed = new List<string>
            {
                "[",
                "]",
                "[gm]",
                "[supporter]",
                "bitch",
                "ass",
                "fuck",
                "fucker",
                "nigger",
                "owner",
                "Hacker",
                "motherfucker",
                "anal",
                "pussy",
            };

            if (BlackListed.Any(s => characterName.ToLower().Contains(s)))
            {
                Session.SendPacketFormat($"info {Language.Instance.GetMessageFromKey("BLACKLIST")}");
                return false;
            }

            if (slot > 3)
            {
                return false;
            }

            return true;
        }

        public static void CreateCharacter(this ClientSession session, GenderType gender, HairColorType hairColor, HairStyleType hairStyle, string Name, byte slot, bool isMartial)
        {
            var newCharacter = new CharacterDTO
            {
                Class = isMartial ? ClassType.MartialArtist : ClassType.Adventurer,
                Mp = isMartial ? 3156 : 69,
                Hp = isMartial ? 9401 : 515,
                Level = (byte)(isMartial ? 80 : 1),
                JobLevel = (byte)(isMartial ? 20 : 1),
                Gender = gender,
                HairColor = hairColor,
                HairStyle = hairStyle,
                MapId = 1,
                MapX = ServerManager.RandomNumber<short>(73, 77),
                MapY = ServerManager.RandomNumber<short>(123, 124),
                MaxMateCount = 10,
                MaxPartnerCount = 2,
                SpPoint = 5000,
                SpAdditionPoint = 0,
                Name = Name,
                Slot = slot,
                AccountId = session.Account.AccountId,
                MinilandMessage = "Welcome",
                State = CharacterState.Active,
                MinilandPoint = 2000,
                UnlockedHLevel = 60
            };

            DAOFactory.CharacterDAO.InsertOrUpdate(ref newCharacter);

            // init skills
            var wSkills = new List<CharacterSkillDTO>
            {
                new CharacterSkillDTO {CharacterId = newCharacter.CharacterId, SkillVNum = 1525},
                new CharacterSkillDTO {CharacterId = newCharacter.CharacterId, SkillVNum = 1526},
                new CharacterSkillDTO {CharacterId = newCharacter.CharacterId, SkillVNum = 1527},
                new CharacterSkillDTO {CharacterId = newCharacter.CharacterId, SkillVNum = 1528},
                new CharacterSkillDTO {CharacterId = newCharacter.CharacterId, SkillVNum = 1529},
                new CharacterSkillDTO {CharacterId = newCharacter.CharacterId, SkillVNum = 1530},
                new CharacterSkillDTO {CharacterId = newCharacter.CharacterId, SkillVNum = 1531},
                new CharacterSkillDTO {CharacterId = newCharacter.CharacterId, SkillVNum = 1532},
                new CharacterSkillDTO {CharacterId = newCharacter.CharacterId, SkillVNum = 1533},
                new CharacterSkillDTO {CharacterId = newCharacter.CharacterId, SkillVNum = 1534},
                new CharacterSkillDTO {CharacterId = newCharacter.CharacterId, SkillVNum = 1535},
                new CharacterSkillDTO {CharacterId = newCharacter.CharacterId, SkillVNum = 1536},
                new CharacterSkillDTO {CharacterId = newCharacter.CharacterId, SkillVNum = 1537},
                new CharacterSkillDTO {CharacterId = newCharacter.CharacterId, SkillVNum = 1538},
                new CharacterSkillDTO {CharacterId = newCharacter.CharacterId, SkillVNum = 1539},
                new CharacterSkillDTO {CharacterId = newCharacter.CharacterId, SkillVNum = 209}
            };

            var Skills = new List<CharacterSkillDTO>
            {
                new CharacterSkillDTO {CharacterId = newCharacter.CharacterId, SkillVNum = 200},
                new CharacterSkillDTO {CharacterId = newCharacter.CharacterId, SkillVNum = 201},
                new CharacterSkillDTO {CharacterId = newCharacter.CharacterId, SkillVNum = 209}
            };

            var QuickList = new List<QuicklistEntryDTO>
            {
                new QuicklistEntryDTO
                {
                    CharacterId = newCharacter.CharacterId,
                    Type = 1,
                    Slot = 1,
                    Pos = 1
                },
                new QuicklistEntryDTO
                {
                    CharacterId = newCharacter.CharacterId,
                    Q2 = 1,
                    Slot = 2
                },
                new QuicklistEntryDTO
                {
                    CharacterId = newCharacter.CharacterId,
                    Q2 = 8,
                    Type = 1,
                    Slot = 1,
                    Pos = 16
                },
                new QuicklistEntryDTO
                {
                    CharacterId = newCharacter.CharacterId,
                    Q2 = 9,
                    Type = 1,
                    Slot = 3,
                    Pos = 1
                }
            };

            var firstQuest = new CharacterQuestDTO
            {
                CharacterId = newCharacter.CharacterId,
                QuestId = isMartial ? 3340 : 1997,
                IsMainQuest = true
            };

            DAOFactory.CharacterQuestDAO.InsertOrUpdate(firstQuest);

            if (isMartial)
            {
                DAOFactory.CharacterSkillDAO.InsertOrUpdate(wSkills);
            }
            else
            {
                DAOFactory.CharacterSkillDAO.InsertOrUpdate(Skills);
                DAOFactory.QuicklistEntryDAO.InsertOrUpdate(QuickList);
            }

            using (var startupInventory = new Inventory(new OpenNos.GameObject.Character(newCharacter)))
            {
                if (isMartial)
                {
                    startupInventory.AddNewToInventory(5826, 1, InventoryType.Main); // box
                }
                else
                {
                    startupInventory.AddNewToInventory(1, 1, InventoryType.Wear, 5, 5);
                    startupInventory.AddNewToInventory(8, 1, InventoryType.Wear, 5, 5);
                    startupInventory.AddNewToInventory(12, 1, InventoryType.Wear, 5, 5);
                }

                startupInventory.AddNewToInventory(1008, 10, InventoryType.Main);
                startupInventory.AddNewToInventory(1012, 5, InventoryType.Main); //seeds
                startupInventory.AddNewToInventory(5332, 1, InventoryType.Main);
                startupInventory.AddNewToInventory(9041, 1, InventoryType.Main);
                startupInventory.AddNewToInventory(800, 5, InventoryType.Equipment);
                startupInventory.AddNewToInventory(801, 5, InventoryType.Equipment);
                startupInventory.AddNewToInventory(802, 5, InventoryType.Equipment);
                startupInventory.AddNewToInventory(803, 5, InventoryType.Equipment);
                startupInventory.AddNewToInventory(2081, 5, InventoryType.Etc);

                startupInventory.ForEach(i => DAOFactory.ItemInstanceDAO.InsertOrUpdate(i));
            }
        }

        public static void OpenBank(this ClientSession Session)
        {
            Session.SendPacket(Session.Character.GenerateGB((byte)GoldBankPacketType.OpenBank));
            Session.SendPacket(UserInterfaceHelper.GenerateShopMemo((byte)SmemoType.Information, Language.Instance.GetMessageFromKey("OPEN_BANK")));
        }

        #endregion
    }
}