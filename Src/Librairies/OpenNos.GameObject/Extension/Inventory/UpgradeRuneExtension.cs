using System.Linq;
using ChickenAPI.Enums.Game.BCard;
using NosTale.Configuration;
using NosTale.Configuration.Configuration.Item;
using NosTale.Configuration.Utilities;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject.Extension.Inventory
{
    public static class UpgradeRuneExtension
    {
        #region Constants

        public const short PREMIUM_RUNE_SCROLL = 5813;
        public const short BASIC_RUNE_SCROLL = 5823;

        #endregion

        #region Methods

        public static void UpgradeRune(this ItemInstance equipment, ClientSession session,
            UpgradeRuneType protectionType)
        {
            //// Not finished
            //s.SendShopEnd();
            //return;

            #region Basic checks to see if you can upgrade

            if (!CanUpgrade(equipment))
            {
                session.SendShopEnd();
                return;
            }

            if (protectionType != UpgradeRuneType.None)
            {
                if (session.Character.Inventory.CountItem(protectionType == UpgradeRuneType.Premium
                    ? PREMIUM_RUNE_SCROLL
                    : BASIC_RUNE_SCROLL) < 1)
                {
                    session.SendShopEnd();
                    return;
                }
            }

            #endregion

            var isProtected = false;
            int value = equipment.RuneAmount;

            var runeUpgradeRate = DependencyContainer.Instance.GetInstance<JsonItemConfiguration>().RUpgrade;

            if (session.Character.Gold < runeUpgradeRate.GoldPrice[value])
            {
                session.SendShopEnd();
                return;
            }

            foreach (var item in runeUpgradeRate.Item[value])
            {
                if (session.Character.Inventory.CountItem(item.Id) < item.Quantity)
                {
                    // Not Enough Item
                    session.SendShopEnd();
                    return;
                }
            }

            switch (protectionType)
            {
                case UpgradeRuneType.Premium:
                case UpgradeRuneType.Basic:
                    isProtected = true;
                    break;
            }

            var probsOfBreaking = runeUpgradeRate.PercentBroken[value];
            var probsOfFailing =
                runeUpgradeRate.PercentFail[value] + (protectionType == UpgradeRuneType.Premium ? 2 : 0);

            bool upgraded = false;
            string msg;
            int effectId;
            if (ServerManager.RandomProbabilityCheck(probsOfBreaking)) // fail + level --
            {
                if (!isProtected)
                {
                    equipment.IsBreaked = true;
                    effectId = 3003;
                    msg = $"Your {equipment.Item.Name} rune upgrade failed. The rune has been broken.";
                }
                else
                {
                    effectId = 3004;
                    msg = $"Your {equipment.Item.Name} rune upgrade failed, but the scroll saved it.";
                }
            }
            else if (ServerManager.RandomProbabilityCheck(probsOfFailing)) // fail
            {
                effectId = 3004;
                msg = $"Your {equipment.Item.Name} rune upgrade failed.";
            }
            else // success
            {
                equipment.RuneAmount++;
                effectId = 3005;
                msg = $"Your {equipment.Item.Name} rune has been upgraded.";
                equipment.ApplyRandomRune(session, msg);
                upgraded = true;
            }

            ConsumeMaterials(equipment, session, protectionType, isProtected, runeUpgradeRate, upgraded);

            session.SendPacket(equipment.GenerateInventoryAdd());
            session.GoldLess(runeUpgradeRate.GoldPrice[value]);
            session.CurrentMapInstance.Broadcast(
                StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, effectId),
                session.Character.PositionX, session.Character.PositionY);
            session.SendPacket(UserInterfaceHelper.GenerateMsg(msg, 0));
            session.SendPacket(UserInterfaceHelper.GenerateSay(msg, 11));
            session.SendPacket(UserInterfaceHelper.GenerateGuri(19, 1, session.Character.CharacterId, 2388));
            session.SendShopEnd();
        }

        private static void ConsumeMaterials(ItemInstance equipment, ClientSession session,
            UpgradeRuneType protectionType, bool isProtected, UpgradeRuneConfiguration runeUpgradeRate, bool upgraded)
        {
            var value = equipment.RuneAmount;

            if (ServerManager.IsUnderDebugMode) return;

            if (isProtected && !upgraded)
            {
                if (ServerManager.RandomProbabilityCheck(50))
                {
                    RemoveItems(runeUpgradeRate, session, value);
                }
                else
                {
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        "Luckly, materials weren't consumed after this try thanks to premium scroll.", 0));
                }
            }
            else
            {
                RemoveItems(runeUpgradeRate, session, value);
            }

            switch (protectionType)
            {
                case UpgradeRuneType.Premium:
                    session.Character.Inventory.RemoveItemAmount(PREMIUM_RUNE_SCROLL);
                    break;
                case UpgradeRuneType.Basic:
                    session.Character.Inventory.RemoveItemAmount(BASIC_RUNE_SCROLL);
                    break;
            }
        }

        private static void RemoveItems(UpgradeRuneConfiguration runeUpgradeValues, ClientSession session, byte value)
        {
            foreach (var item in runeUpgradeValues.Item[value])
            {
                session.Character.Inventory.RemoveItemAmount(item.Id, item.Quantity);
            }
        }

        private static void ApplyRandomRune(this ItemInstance item, ClientSession session, string message)
        {
            switch (item.RuneAmount)
            {
                case 3:
                case 6:
                case 9:
                case 12:
                case 15:
                    item.ApplyRuneBuff(session, message);
                    break;

                default:
                    item.ApplyRuneEffect(session, message);
                    break;
            }
        }

        private static void ApplyRuneBuff(this ItemInstance equipment, ClientSession session, string message)
        {
            var possibleBuffByTypeAndSubType = new[]
            {
                new PossibleTypeAndSubtype
                {
                    Type = 105,
                    SubType = 0,
                    ValueByLevel = new short[] {1900, 1901, 1902, 1903, 1904}
                },
                new PossibleTypeAndSubtype
                {
                    Type = 105,
                    SubType = 1,
                    ValueByLevel = new short[] {1905, 1906, 1907, 1908, 1909}
                },
                new PossibleTypeAndSubtype
                {
                    Type = 105,
                    SubType = 2,
                    ValueByLevel = new short[] {1910, 1911, 1912, 1913, 1914}
                },
                new PossibleTypeAndSubtype
                {
                    Type = 105,
                    SubType = 3,
                    ValueByLevel = new short[] {1915, 1916, 1917, 1918, 1919}
                },
                new PossibleTypeAndSubtype
                {
                    Type = 105,
                    SubType = 4,
                    ValueByLevel = new short[] {1920, 1921, 1922, 1923, 1924}
                },
                new PossibleTypeAndSubtype
                {
                    Type = 106,
                    SubType = 0,
                    ValueByLevel = new short[] {1925, 1926, 1927, 1928, 1929}
                },
                new PossibleTypeAndSubtype
                {
                    Type = 106,
                    SubType = 1,
                    ValueByLevel = new short[] {1930, 1931, 1932, 1933, 1934}
                },
                new PossibleTypeAndSubtype
                {
                    Type = 106,
                    SubType = 2,
                    ValueByLevel = new short[] {1935, 1936, 1937, 1938, 1939}
                },
                new PossibleTypeAndSubtype
                {
                    Type = 106,
                    SubType = 3,
                    ValueByLevel = new short[] {1940, 1941, 1942, 1943, 1944}
                },
                new PossibleTypeAndSubtype
                {
                    Type = 106,
                    SubType = 4,
                    ValueByLevel = new short[] {1945, 1946, 1947, 1948, 1949}
                }
            };

            var rndmBuff = ServerManager.RandomNumber(0, possibleBuffByTypeAndSubType.Length);

            var getBuff = possibleBuffByTypeAndSubType[rndmBuff];

            var runeBuff = DAOFactory.RuneEffectDAO.LoadByEquipmentSerialId(equipment.EquipmentSerialId).Where(
                    s => s.SubType == getBuff.SubType && (byte) s.Type == (byte) getBuff.Type)
                .FirstOrDefault();

            //105.2.4.7640.1 106.1.4.7720.1
            if (runeBuff != null)
            {
                if (runeBuff.ThirdData == 5)
                {
                    equipment.ApplyRuneBuff(session, message);
                    return;
                }

                equipment.RuneEffects.Remove(runeBuff);
                runeBuff.ThirdData++;
                runeBuff.FirstData++;

                runeBuff = new RuneEffectDTO
                {
                    RuneEffectId = runeBuff.RuneEffectId,
                    SubType = (byte) getBuff.SubType,
                    Type = (BCardType) getBuff.Type,
                    FirstData = runeBuff.FirstData,
                    SecondData = getBuff.ValueByLevel[runeBuff.ThirdData - 1],
                    ThirdData = runeBuff.ThirdData,
                    EquipmentSerialId = equipment.EquipmentSerialId,
                    IsPower = true
                };
            }
            else
            {
                if (equipment.RuneEffects.Where(x => x.Type == BCardType.A7Powers1 || x.Type == BCardType.A7Powers2)
                    .Count() >= 2)
                {
                    equipment.ApplyRuneBuff(session, message);
                    return;
                }

                runeBuff = new RuneEffectDTO
                {
                    SubType = (byte) getBuff.SubType,
                    Type = (BCardType) getBuff.Type,
                    FirstData = 1,
                    SecondData = getBuff.ValueByLevel[0],
                    ThirdData = 1,
                    EquipmentSerialId = equipment.EquipmentSerialId
                };
            }

            equipment.RuneEffects.Add(runeBuff);
            DAOFactory.RuneEffectDAO.InsertOrUpdate(runeBuff);

            session.SendPacket($"ru_suc 0 {runeBuff.Type}.{(byte) runeBuff.SubType}.{runeBuff.FirstData * 4}.{runeBuff.SecondData * 4}.{runeBuff.ThirdData} " + message);
        }

        private static void ApplyRuneEffect(this ItemInstance equipment, ClientSession session, string message)
        {
            // 13 Possible Type
            var possibleListTypeandSubType = new[]
            {
                new PossibleTypeAndSubtype
                {
                    Type = 3,
                    SubType = 0,
                    ValueByLevel = new short[] {20, 40, 80, 150, 200}
                },
                new PossibleTypeAndSubtype
                {
                    Type = 44,
                    SubType = 1,
                    ValueByLevel = new short[] {1, 2, 4, 7, 10}
                },
                new PossibleTypeAndSubtype
                {
                    Type = 9,
                    SubType = 0,
                    ValueByLevel = new short[] {20, 40, 80, 150, 200}
                },
                new PossibleTypeAndSubtype
                {
                    Type = 44,
                    SubType = 1,
                    ValueByLevel = new short[] {1, 2, 4, 7, 10}
                },
                new PossibleTypeAndSubtype
                {
                    Type = 4,
                    SubType = (short) (equipment.Item.Class == 8 ? 3 : 0),
                    ValueByLevel = equipment.Item.Class == 8
                        ? new short[] {1, 3, 5, 7, 15}
                        : new short[] {20, 40, 70, 110, 150}
                },
                new PossibleTypeAndSubtype
                {
                    Type = (short) (equipment.Item.Class == 8 ? 103 : 102),
                    SubType = 4,
                    ValueByLevel = new short[] {1, 2, 4, 7, 10}
                },
                new PossibleTypeAndSubtype
                {
                    Type = 7,
                    SubType = 4,
                    ValueByLevel = new short[] {10, 15, 20, 30, 50}
                },
                new PossibleTypeAndSubtype
                {
                    Type = 13,
                    SubType = 0,
                    ValueByLevel = new short[] {3, 5, 7, 10, 15}
                },
                new PossibleTypeAndSubtype
                {
                    Type = 33,
                    SubType = 0,
                    ValueByLevel = new short[] {200, 400, 800, 1300, 2000}
                },
                new PossibleTypeAndSubtype
                {
                    Type = 110,
                    SubType = 2,
                    ValueByLevel = new short[] {1, 2, 4, 7, 10}
                },
                new PossibleTypeAndSubtype
                {
                    Type = 33,
                    SubType = 1,
                    ValueByLevel = new short[] {200, 400, 800, 1300, 2000}
                },
                new PossibleTypeAndSubtype
                {
                    Type = 110,
                    SubType = 3,
                    ValueByLevel = new short[] {1, 2, 4, 7, 10}
                },
                new PossibleTypeAndSubtype
                {
                    Type = 104,
                    SubType = 3,
                    ValueByLevel = new short[] {1, 2, 4, 7, 10}
                }
            };

            var rndm = ServerManager.RandomNumber(0, possibleListTypeandSubType.Length);

            var getTypeAndSubtype = possibleListTypeandSubType[rndm];

            var runeEffect = DAOFactory.RuneEffectDAO.LoadByEquipmentSerialId(equipment.EquipmentSerialId).Where(
                s => s.SubType == getTypeAndSubtype.SubType &&
                     s.Type == (BCardType) getTypeAndSubtype.Type).FirstOrDefault();

            if (runeEffect != null)
            {
                if (runeEffect.ThirdData == 5)
                {
                    equipment.ApplyRuneEffect(session, message);
                    return;
                }

                equipment.RuneEffects.Remove(runeEffect);
                runeEffect.ThirdData++;

                runeEffect = new RuneEffectDTO
                {
                    RuneEffectId = runeEffect.RuneEffectId,
                    SubType = (byte) getTypeAndSubtype.SubType,
                    Type = (BCardType) getTypeAndSubtype.Type,
                    FirstData = getTypeAndSubtype.ValueByLevel[runeEffect.ThirdData - 1],
                    SecondData = 0,
                    ThirdData = runeEffect.ThirdData,
                    EquipmentSerialId = equipment.EquipmentSerialId
                };
            }
            else
            {
                if (equipment.RuneEffects.Where(x => x.Type != BCardType.A7Powers1 && x.Type == BCardType.A7Powers2)
                    .Count() >= 7)
                {
                    equipment.ApplyRuneEffect(session, message);
                    return;
                }

                runeEffect = new RuneEffectDTO
                {
                    SubType = (byte) getTypeAndSubtype.SubType,
                    Type = (BCardType) getTypeAndSubtype.Type,
                    FirstData = getTypeAndSubtype.ValueByLevel[0],
                    SecondData = 0,
                    ThirdData = 1,
                    EquipmentSerialId = equipment.EquipmentSerialId
                };
            }

            equipment.RuneEffects.Add(runeEffect);
            DAOFactory.RuneEffectDAO.InsertOrUpdate(runeEffect);

            session.SendPacket(
                $"ru_suc 0 {(byte) runeEffect.Type}.{(byte) runeEffect.SubType}.{runeEffect.FirstData * 4}.{runeEffect.SecondData * 4}.{runeEffect.ThirdData} " +
                message);
        }

        private static bool CanUpgrade(ItemInstance item)
        {
            if (item.Item.EquipmentSlot != EquipmentType.MainWeapon) return false;

            if (item.RuneAmount == 15) return false;

            if (item.Item.LevelMinimum < 80 && !item.Item.IsHeroic) return false;

            if (item.IsBreaked) return false;

            return true;
        }

        #endregion
    }
}