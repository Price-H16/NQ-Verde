using OpenNos.GameObject.Networking;
using OpenNos.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenNos.Data;
using OpenNos.Core;
using System.Runtime.ExceptionServices;

namespace OpenNos.GameObject.Helpers
{
    public static class CraveRuneHelper
    {
        private static readonly short[][][] BasicRunes = new short[][][]
            {
                new short[][]{ //All Attacks Increease
                        new short[]
                        {
                            3, 1
                        },
                        new short[]
                        {
                            20,40,80,150,200,250
                        }
                    },
                    new short[][]{ //All Attacks Increease %
                        new short[]
                        {
                            44, 1
                        },
                        new short[]
                        {
                            1,2,4,7,10,13
                        }
                    },
                    new short[][]{ //All Defenses Increease
                        new short[]
                        {
                            9, 1
                        },
                        new short[]
                        {
                            20,40,80,150,200,250
                        }
                    },
                    new short[][]{ //All Defenses Increease %
                        new short[]
                        {
                           44, 2
                        },
                        new short[]
                        {
                            -1,-2,-4,-7,-10,-13
                        }
                    },
                    new short[][]{    //Concentration and HitRate Plain
                        new short[]
                        {
                            4, 1
                        },
                        new short[]   //HitRate
                        {
                            20,40,70,110,150,190
                        },
                        new short[]   //Concentration
                        {
                           4,4
                        },
                        new short[]
                        {
                            1,3,5,7,15,22
                        }
                    },
                    new short[][]{    //HitRate Concentration %
                        new short[]   //Hit Rate
                        {
                            102, 5
                        },
                        new short[]   //HitRate
                        {
                            1,2,4,7,10,13
                        },
                        new short[]   //Concentration
                        {
                            103, 5
                        },
                        new short[]   //Concentration %
                        {
                            1,2,4,7,10,13
                        }
                    },
                    new short[][]{ //All elements Energy Increase
                        new short[]
                        {
                            7, 5
                        },
                        new short[]
                        {
                            10,15,20,30,50,60
                        }
                    },
                    new short[][]{ //All elemental resistance is increased by X.
                        new short[]
                        {
                            13,1
                        },
                        new short[]
                        {
                            3,5,7,10,15,20
                        }
                    },
                    new short[][]{ //Maximum HP is increased by X.
                        new short[]
                        {
                            33,1
                        },
                        new short[]
                        {
                            200,400,800,1300,2000,2700
                        }
                    },
                    new short[][]{ //Maximum HP is increased by X%.
                        new short[]
                        {
                            110,3
                        },
                        new short[]
                        {
                            1,2,4,7,10,13
                        }
                    },
                    new short[][]{ //Maximum MP is increased by X.
                        new short[]
                        {
                            33,2
                        },
                        new short[]
                        {
                            200,400,800,1300,2000,2500
                        }
                    },
                    new short[][]{ //Maximum MP is increased by X%.
                        new short[]
                        {
                            110,4
                        },
                        new short[]
                        {
                            1,2,4,7,10,13
                        }
                    },
            };
        private static readonly Dictionary<byte, byte[]> MatsAmmount = new Dictionary<byte, byte[]>
        {
            {0, new byte[] { 10, 5, 0, 0, 0, 0, 0, 0, 0 } },
            {1, new byte[] { 12, 7, 0, 0, 0, 0 , 0, 0, 0 }},
            {2, new byte[] { 16, 11, 10, 1, 0, 0 , 0, 0, 0 }},
            {3, new byte[] { 14, 9, 0, 0, 0, 0 , 0, 0, 0 }},
            {4, new byte[] { 16, 11, 0, 0, 0, 0 , 0, 0, 0 }},
            {5, new byte[] { 20, 15, 15, 2, 0, 0 , 0, 0, 0 }},
            {6, new byte[] { 18, 13, 0, 0, 0, 0 , 0, 0, 0 }},
            {7, new byte[] { 20, 15, 0, 0, 0, 0, 0, 0, 0  }},
            {8, new byte[] { 50, 40, 40, 1, 1, 0, 0, 0, 0  }},
            {9, new byte[] { 44, 34, 0, 0, 0, 0, 0, 0, 0  }},
            {10, new byte[] { 48, 38, 0, 0, 0, 0, 0, 0, 0  }},
            {11, new byte[] { 60, 50, 50, 0, 1, 1, 0, 0, 0  }},
            {12, new byte[] { 52, 42, 0, 0, 0, 0, 0, 0, 0  }},
            {13, new byte[] { 56, 46, 0, 0, 0, 0, 0, 0, 0  }},
            {14, new byte[] { 80, 60, 60, 0, 2, 2, 0, 0, 0 }},
            {15, new byte[] { 0, 0, 24, 0, 0, 0, 40, 0, 0 } },
            {16, new byte[] { 0, 0, 25, 0, 0, 0, 42, 0, 0 } },
            {17, new byte[] { 0, 0, 35, 0, 0, 0, 66, 1, 1 } },
            {18, new byte[] { 0, 0, 27, 0, 0, 0, 42, 0, 0 } },
            {19, new byte[] { 0, 0, 29, 0, 0, 0, 48, 0, 0 } },
            {20, new byte[] { 0, 0, 40, 0, 0, 0, 80, 2, 2 } },
        };

        private static readonly short[] MatsVNums = new short[]
        {
            2416,
            2411,
            2430,
            2475,
            2413,
            2462,
            2483, //paimon's silver whatever
            2484, //corrupted celestial spire fragment
            2482, //Paimon soul silver
        };

        private static readonly short[][] SpecialRunes = new short[][]
            {
                new short[]{105,1,1,1900,1},  //Apocalypse Power
                    new short[]{105,2,1,1905,1},  //Reflection Power
                    new short[]{105,3,1,1910,1},  //Wolf Power
                    new short[]{105,4,1,1915,1},  //Kickback Power
                    new short[]{105,5,1,1920,1},  //Explosion Power

                    new short[]{106,1,1,1925,1},  //Agility Power
                    new short[]{106,2,1,1930,1},  //Lightning Power
                    new short[]{106,3,1,1935,1},  //Curse Power
                    new short[]{106,4,1,1940,1},  //Bear Power
                    new short[]{106,5,1,1945,1},  //Frost Power
            
            };

        private static readonly short[] SuccessRate = new short[]
        {
                100,
                90,
                79,
                65,
                45,
                25,
                20,
                15,
                7,
                7,
                5,
                1,
                3,
                2,
                1,
                2,
                1,
                1,
                2,
                1,
                1,
        };

        private static readonly short[] MajorFailRate = new short[]
        {
                0,
                0,
                4,
                5,
                6,
                7,
                8,
                9,
                10,
                12,
                14,
                16,
                18,
                20,
                25,
                21,
                22,
                25,
                23,
                24,
                25
        };
        private static readonly int[] Gold = { 3000, 16000, 99000, 55000, 110000, 280000, 220000, 310000, 500000, 450000, 560000, 790000, 700000, 880000, 1100000, 1320000, 1584000, 1916640, 1642400, 1916640, 2108304};

        public static void GenerateCraveRune(this ItemInstance item, ClientSession Session, RuneScrollType type)
        {
            int u = item.RuneCount;
            ServerManager.Shout($"{item.RuneCount}");
            if (item.RuneCount > 20)
            {
                Session.SendPacket(Session.Character.GenerateSay("Rune at max Level!", 11));
                return;
            }
            try
            {

                //GoldCheck
                if (Session.Character.Gold < Gold[u])
                {
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 11));
                    return;
                }
                if (!MatsAmmount.TryGetValue((byte)u, out byte[] ammounts))
                {
                    return;
                }

                int i = 0;
                if ((byte)type != 100)
                {
                    //ItemCheck
                    foreach (int vnum in MatsVNums)
                    {
                        if (ammounts[i] != 0)
                        {
                            if (!(Session.Character.Inventory.CountItem(vnum) >= ammounts[i]))
                            {
                                ServerManager.Shout($"{vnum} {ammounts[i]} {u}");
                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEM"), 11));
                                return;
                            }
                        }
                        i++;
                    }

                    //Scrolls Check
                    if ((byte)type == 1)
                    {
                        if (!(Session.Character.Inventory.CountItem(5823) >= 1))
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEM"), 11));
                            return;
                        }
                    }
                    else if ((byte)type == 2)
                    {
                        if (!(Session.Character.Inventory.CountItem(5813) >= 1))
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEM"), 11));
                            return;
                        }
                    }
                    i = 0;

                    //Gold take
                    Session.Character.Gold -= Gold[u];
                    Session.SendPacket(Session.Character.GenerateGold());



                }
                bool materialConsume = false;
                if (ServerManager.RandomNumber() <= SuccessRate[u] + ((byte)type > 1 ? 2 : 0) || (byte)type == 100)
                {
                    if (u % 3 == 2 && ServerManager.RandomNumber() <= 95 + ((byte)type > 0 ? 5 : 0))
                    {
                        AddSpecialRune(item);
                    }
                    else
                    {
                        AddBasicRune(item);
                    }

                    item.RuneCount++;
                    item.LoadRuneBcards();
                    Session.SendPacket($"shop_end {((byte)type == 0 ? 2 : 1)}");
                    Session.SendPacket(item.GenerateInventoryAdd());
                    Session.SendPacket(Session.Character.GenerateSay("Rune Upgrade Succeded!", 12));
                }
                else if (ServerManager.RandomNumber() <= MajorFailRate[u] && (byte)type < 1)
                {
                    Session.SendPacket($"shop_end {((byte)type == 0 ? 2 : 1)}");
                    item.RuneBroke = true;
                    Session.SendPacket(Session.Character.GenerateSay("Rune Upgrade Failed! The weapon was damaged in the proccess.", 11));
                }
                else
                {
                    materialConsume = (byte)type > 0 ? true : false;
                    Session.SendPacket($"shop_end {((byte)type == 0 ? 2 : 1)}");
                    Session.SendPacket(Session.Character.GenerateSay("Rune upgrade failed!", 11));
                }

                if (ServerManager.RandomNumber() < 50 && materialConsume)
                {
                    Session.SendPacket(Session.Character.GenerateSay("Materials Saved", 12));
                }
                else
                {
                    //Item take
                    foreach (int vnum in MatsVNums)
                    {
                        if (ammounts[i] != 0)
                        {
                            Session.Character.Inventory.RemoveItemAmount(vnum, ammounts[i]);
                        }
                        i++;
                    }
                }

                if ((byte)type == 1)
                {
                    Session.Character.Inventory.RemoveItemAmount(5823, 1);
                }
                else if ((byte)type == 2)
                {
                    Session.Character.Inventory.RemoveItemAmount(5813, 1);
                }
            }
            catch (Exception e)
            {
                throw;
            }

        }

        private static void AddBasicRune(ItemInstance item)
        {
            byte Position = (byte)ServerManager.RandomNumber(0, BasicRunes.Length);
            short[][] Rune = BasicRunes[Position];
            bool alreadyExist = false;

            //ReAjust if is Concentration or Hitrate // Concentration is only for Magicians
            if (Position == 4 || Position == 5)
            {
                if (item.Item.Class == 8)
                {
                    Rune[0][1] = Rune[2][1];
                    Rune[1][0] = Rune[3][0];
                    Rune[1][1] = Rune[3][1];
                    Rune[1][2] = Rune[3][2];
                    Rune[1][3] = Rune[3][3];
                    Rune[1][4] = Rune[3][4];
                }
            }
            byte safetycheck = 0;
            bool loop = true;
            if (item.RuneEffects.Any((s => s.Type == Rune[0][0] && s.SubType == Rune[0][1] && s.FirstData != Rune[1][4]))
                || item.RuneEffects.Where(s => s.Type != 105 && s.Type != 106).Count() > 4)
            {
                do
                {
                    if (safetycheck > 10)
                    {
                        loop = false;
                    }
                    if (item.RuneEffects.FirstOrDefault(s => s.Type == Rune[0][0] && s.SubType == Rune[0][1] && s.FirstData != Rune[1][5]) is RuneEffectsDTO runeEffect && runeEffect != null)
                    {
                        for (byte i = 0; i < 5; i++)
                        {
                            if (runeEffect.FirstData == Rune[1][i])
                            {
                                runeEffect.FirstData = Rune[1][i + 1];
                                loop = false;
                                break;
                            }
                        }
                    }
                    Position = (byte)ServerManager.RandomNumber(0, BasicRunes.Length);
                    Rune = BasicRunes[Position];
                    safetycheck++;
                }
                while (loop);
            }
            else
            {
                RuneEffectsDTO Effect = new RuneEffectsDTO()
                {
                    EquipmentSerialId = item.EquipmentSerialId,
                    Type = (byte)Rune[0][0],
                    SubType = (byte)Rune[0][1],
                    FirstData = Rune[1][0],
                    SecondData = 0,
                    ThirdDada = 0
                };
                item.RuneEffects.Add(Effect);
            }

        }

        private static void AddSpecialRune(ItemInstance item)
        {
            byte Position = (byte)ServerManager.RandomNumber(0, SpecialRunes.Length);
            short[] Rune = SpecialRunes[Position];
            bool alreadyExist = true;
            if (item.RuneEffects.Where(s => s.Type == 105 || s.Type == 106).ToList() is List<RuneEffectsDTO> specialrunes)
            {
                if (specialrunes.Count() > 1)
                {
                    if (specialrunes.OrderBy(s => ServerManager.RandomNumber()).FirstOrDefault() is RuneEffectsDTO runeToUp)
                    {
                        item.RuneEffects.ForEach(rf =>
                        {
                            if (rf.Type == runeToUp.Type && rf.SubType == runeToUp.SubType)
                            {
                                rf.FirstData++;
                                rf.SecondData++;
                            }
                        });
                    }
                }
                else if (specialrunes.Count() > 0 && specialrunes.FirstOrDefault(s => s.Type == Rune[0] && s.SubType == Rune[1]) is RuneEffectsDTO runeToUp && runeToUp != null)
                {
                    item.RuneEffects.ForEach(rf =>
                    {
                        if (rf.Type == runeToUp.Type && rf.SubType == runeToUp.SubType)
                        {
                            rf.FirstData++;
                            rf.SecondData++;
                        }
                    });
                }
                else
                {
                    alreadyExist = false;
                }
            }
            else
            {
                alreadyExist = false;
            }
            if (!alreadyExist)
            {
                RuneEffectsDTO Effect = new RuneEffectsDTO()
                {
                    EquipmentSerialId = item.EquipmentSerialId,
                    Type = (byte)Rune[0],
                    SubType = (byte)Rune[1],
                    FirstData = Rune[2],
                    SecondData = Rune[3],
                    ThirdDada = Rune[4]
                };
                item.RuneEffects.Add(Effect);
            }
        }
    }
}
