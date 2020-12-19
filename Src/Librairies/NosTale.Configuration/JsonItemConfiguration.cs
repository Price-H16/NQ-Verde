using System.Collections.Generic;
using NosTale.Configuration.Configuration.Item;

namespace NosTale.Configuration
{
    public class JsonItemConfiguration
    {
        #region Properties

        public CraftA6FairyConfiguration Fairy { get; set; } = new CraftA6FairyConfiguration
        {
            FairyVnum = new[] {4129, 4130, 4131, 4132}, // 4129, 4130, 4131, 4132
            GoldPrice = new[] {1500000, 1500000, 50000000}, //2000000, 2000000, 50000000
            PercentSucess = new[] {10, 10, 100}, //10, 10, 100
            SuccesVnumFairy = new[] {4705, 4709, 4713}, //4705, 4709, 4713
            Item = new[]
            {
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 5875,
                        Quantity = 1
                    },
                    new RequiredItem
                    {
                        Id = 2434,
                        Quantity = 1
                    }
                },
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 5876,
                        Quantity = 1
                    },
                    new RequiredItem
                    {
                        Id = 2435,
                        Quantity = 1
                    }
                },
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 5877,
                        Quantity = 1
                    },
                    new RequiredItem
                    {
                        Id = 2444,
                        Quantity = 1
                    }
                }
            }
        };

        public InventoryConfiguration Inventory { get; set; } = new InventoryConfiguration
        {
            // MaxValue = 32767 / short.MaxValue
            MaxItemPerSlot = 9999
        };

        public PerfectSpConfiguration PerfectSp { get; set; } = new PerfectSpConfiguration
        {
            UpMode = 1,
            GoldPrice = new[] {5000, 10000, 20000, 50000, 100000},
            StonePrice = new short[] {1, 2, 3, 4, 5},
            UpSuccess = new short[] {50, 40, 30, 20, 10}
        };

        public RarifyChancesConfiguration RarifyChances { get; set; } = new RarifyChancesConfiguration
        {
            Raren2 = 80,
            Raren1 = 70,
            Rare0 = 60,
            Rare1 = 40,
            Rare2 = 30,
            Rare3 = 15,
            Rare4 = 10,
            Rare5 = 6,
            Rare6 = 5,
            Rare7 = 3,
            Rare8 = 1,
            GoldPrice = 500,
            ReducedChanceFactor = 1.1,
            ReducedPriceFactor = 0.5,
            RarifyItemNeededQuantity = 5,
            RarifyItemNeededVnum = 1014,
            ScrollVnum = 1218
        };

        public RemoveRuneConfiguration RRemove { get; set; } = new RemoveRuneConfiguration
        {
            ItemVnum = 5812
        };

        public UpgradeRuneConfiguration RUpgrade { get; set; } = new UpgradeRuneConfiguration
        {
            GoldPrice = new[]
            {
                3000, 16000, 99000, 55000, 110000, 280000, 220000, 310000, 500000, 450000, 560000, 790000, 700000,
                880000, 1100000
            },
            PercentSucess = new[] {100, 90, 75, 65, 45, 25, 20, 15, 7, 7, 5, 1, 3, 2, 0.1},
            PercentFail = new[] {0, 10, 17, 30, 49, 68, 72, 76, 83, 81, 81, 83, 79, 80.0, 85.9},
            PercentBroken = new double[] {0, 0, 4, 5, 6, 7, 8, 9, 10, 12, 14, 16, 18, 25, 30},

            // 2416 2411 2430 2475 2413 2462
            Item = new[]
            {
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 2416,
                        Quantity = 10
                    },
                    new RequiredItem
                    {
                        Id = 2411,
                        Quantity = 5
                    }
                },
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 2416,
                        Quantity = 12
                    },
                    new RequiredItem
                    {
                        Id = 2411,
                        Quantity = 7
                    }
                },
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 2416,
                        Quantity = 16
                    },
                    new RequiredItem
                    {
                        Id = 2411,
                        Quantity = 11
                    },
                    new RequiredItem
                    {
                        Id = 2430,
                        Quantity = 10
                    },
                    new RequiredItem
                    {
                        Id = 2475,
                        Quantity = 1
                    }
                },
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 2416,
                        Quantity = 14
                    },
                    new RequiredItem
                    {
                        Id = 2411,
                        Quantity = 9
                    }
                },
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 2416,
                        Quantity = 16
                    },
                    new RequiredItem
                    {
                        Id = 2411,
                        Quantity = 11
                    }
                },
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 2416,
                        Quantity = 20
                    },
                    new RequiredItem
                    {
                        Id = 2411,
                        Quantity = 15
                    },
                    new RequiredItem
                    {
                        Id = 2430,
                        Quantity = 15
                    },
                    new RequiredItem
                    {
                        Id = 2475,
                        Quantity = 2
                    }
                },
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 2416,
                        Quantity = 18
                    },
                    new RequiredItem
                    {
                        Id = 2411,
                        Quantity = 13
                    }
                },
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 2416,
                        Quantity = 20
                    },
                    new RequiredItem
                    {
                        Id = 2411,
                        Quantity = 15
                    }
                },
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 2416,
                        Quantity = 50
                    },
                    new RequiredItem
                    {
                        Id = 2411,
                        Quantity = 40
                    },
                    new RequiredItem
                    {
                        Id = 2430,
                        Quantity = 40
                    },
                    new RequiredItem
                    {
                        Id = 2475,
                        Quantity = 1
                    },
                    new RequiredItem
                    {
                        Id = 2413,
                        Quantity = 1
                    }
                },
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 2416,
                        Quantity = 44
                    },
                    new RequiredItem
                    {
                        Id = 2411,
                        Quantity = 34
                    }
                },
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 2416,
                        Quantity = 48
                    },
                    new RequiredItem
                    {
                        Id = 2411,
                        Quantity = 38
                    }
                },
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 2416,
                        Quantity = 60
                    },
                    new RequiredItem
                    {
                        Id = 2411,
                        Quantity = 50
                    },
                    new RequiredItem
                    {
                        Id = 2430,
                        Quantity = 50
                    },
                    new RequiredItem
                    {
                        Id = 2413,
                        Quantity = 1
                    },
                    new RequiredItem
                    {
                        Id = 2462,
                        Quantity = 1
                    }
                },
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 2416,
                        Quantity = 52
                    },
                    new RequiredItem
                    {
                        Id = 2411,
                        Quantity = 42
                    }
                },
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 2416,
                        Quantity = 56
                    },
                    new RequiredItem
                    {
                        Id = 2411,
                        Quantity = 46
                    }
                },
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 2416,
                        Quantity = 80
                    },
                    new RequiredItem
                    {
                        Id = 2411,
                        Quantity = 60
                    },
                    new RequiredItem
                    {
                        Id = 2430,
                        Quantity = 60
                    },
                    new RequiredItem
                    {
                        Id = 2413,
                        Quantity = 2
                    },
                    new RequiredItem
                    {
                        Id = 2462,
                        Quantity = 2
                    }
                }
            }
        };

        public SummingConfiguration Summing { get; set; } = new SummingConfiguration
        {
            MaxSum = 6,
            SandVnum = 1027,
            GoldPrice = new[] {1500, 3000, 6000, 12000, 24000, 48000},
            SandAmount = new short[] {5, 10, 15, 20, 25, 30},
            UpSucess = new short[] {100, 100, 85, 70, 50, 20}
        };

        public RemoveTattooConfiguration TRemove { get; set; } = new RemoveTattooConfiguration
        {
            ItemVnum = 5799
        };

        public UpgradeTattooConfiguration TUpgrade { get; set; } = new UpgradeTattooConfiguration
        {
            GoldPrice = new[] {30000, 67000, 140000, 230000, 380000, 540000, 770000, 960000, 1200000},
            PercentSucess = new[] {80, 60, 50, 35, 20, 10, 5, 2, 1},
            PercentFail = new[] {20, 30, 35, 40, 50, 55, 45, 28, 9},
            PercentDestroyed = new[] {0, 10, 15, 25, 30, 35, 50, 70, 90},
            Item = new[]
            {
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 2411,
                        Quantity = 17
                    },
                    new RequiredItem
                    {
                        Id = 2416,
                        Quantity = 7
                    },
                    new RequiredItem
                    {
                        Id = 2408,
                        Quantity = 6
                    }
                },
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 2411,
                        Quantity = 19
                    },
                    new RequiredItem
                    {
                        Id = 2416,
                        Quantity = 10
                    },
                    new RequiredItem
                    {
                        Id = 2408,
                        Quantity = 7
                    }
                },
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 2411,
                        Quantity = 21
                    },
                    new RequiredItem
                    {
                        Id = 2416,
                        Quantity = 13
                    },
                    new RequiredItem
                    {
                        Id = 2408,
                        Quantity = 8
                    }
                },
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 2411,
                        Quantity = 25
                    },
                    new RequiredItem
                    {
                        Id = 2416,
                        Quantity = 16
                    },
                    new RequiredItem
                    {
                        Id = 2408,
                        Quantity = 9
                    },
                    new RequiredItem
                    {
                        Id = 2410,
                        Quantity = 15
                    }
                },
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 2411,
                        Quantity = 30
                    },
                    new RequiredItem
                    {
                        Id = 2416,
                        Quantity = 20
                    },
                    new RequiredItem
                    {
                        Id = 2408,
                        Quantity = 10
                    },
                    new RequiredItem
                    {
                        Id = 2410,
                        Quantity = 20
                    }
                },
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 2411,
                        Quantity = 35
                    },
                    new RequiredItem
                    {
                        Id = 2416,
                        Quantity = 25
                    },
                    new RequiredItem
                    {
                        Id = 2408,
                        Quantity = 12
                    },
                    new RequiredItem
                    {
                        Id = 2410,
                        Quantity = 25
                    },
                    new RequiredItem
                    {
                        Id = 2474,
                        Quantity = 3
                    }
                },
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 2411,
                        Quantity = 60
                    },
                    new RequiredItem
                    {
                        Id = 2416,
                        Quantity = 30
                    },
                    new RequiredItem
                    {
                        Id = 2408,
                        Quantity = 20
                    },
                    new RequiredItem
                    {
                        Id = 2410,
                        Quantity = 20
                    },
                    new RequiredItem
                    {
                        Id = 2474,
                        Quantity = 4
                    }
                },
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 2411,
                        Quantity = 80
                    },
                    new RequiredItem
                    {
                        Id = 2416,
                        Quantity = 40
                    },
                    new RequiredItem
                    {
                        Id = 2408,
                        Quantity = 30
                    },
                    new RequiredItem
                    {
                        Id = 2410,
                        Quantity = 25
                    },
                    new RequiredItem
                    {
                        Id = 2412,
                        Quantity = 3
                    }
                },
                new List<RequiredItem>
                {
                    new RequiredItem
                    {
                        Id = 2411,
                        Quantity = 100
                    },
                    new RequiredItem
                    {
                        Id = 2416,
                        Quantity = 80
                    },
                    new RequiredItem
                    {
                        Id = 2408,
                        Quantity = 40
                    },
                    new RequiredItem
                    {
                        Id = 2410,
                        Quantity = 40
                    },
                    new RequiredItem
                    {
                        Id = 2412,
                        Quantity = 4
                    }
                }
            }
        };

        public UpgradeItemConfiguration UpgradeItem { get; set; } = new UpgradeItemConfiguration
        {
            UpFail = new short[] {0, 0, 0, 5, 20, 40, 65, 80, 95, 98},
            UpFix = new short[] {0, 0, 10, 15, 20, 20, 20, 20, 15, 14},
            GoldPrice = new[] {500, 1500, 3000, 10000, 30000, 80000, 150000, 400000, 700000, 1000000},
            CellaAmount = new short[] {20, 50, 80, 120, 160, 220, 280, 380, 480, 600},
            GemAmount = new short[] {1, 1, 2, 2, 3, 1, 1, 2, 2, 3},
            UpFailR8 = new short[] {50, 40, 60, 50, 60, 70, 75, 85, 90, 98},
            UpFixR8 = new short[] {50, 40, 70, 65, 80, 90, 95, 97, 98, 99},
            GoldPriceR8 = new[] {5000, 15000, 30000, 100000, 300000, 800000, 1500000, 4000000, 7000000, 10000000},
            CellaAmountR8 = new short[] {40, 100, 160, 240, 320, 440, 560, 760, 960, 1200},
            GemAmountR8 = new short[] {2, 2, 4, 4, 6, 2, 2, 4, 4, 6},
            MaximumUpgrade = 15,
            CellaVnum = 1014,
            GemFullVnum = 1016,
            GemVnum = 1015,
            GoldScrollVnum = 5369,
            NormalScrollVnum = 1218,
            ReducedPriceFactor = 0.5
        };

        public UpgradeSPConfiguration UpgradeSP { get; set; } = new UpgradeSPConfiguration
        {
            GoldPrice = new[]
            {
                200000, 200000, 200000, 200000, 200000, 500000, 500000, 500000, 500000, 500000, 1000000, 1000000,
                1000000, 1000000, 1000000
            },
            Destroy = new short[] {0, 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 70},
            UpFail = new short[] {20, 25, 30, 40, 50, 60, 65, 70, 75, 80, 90, 91, 94, 97, 99},
            Feather = new short[] {3, 5, 8, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 70},
            FullMoon = new short[] {1, 3, 5, 7, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30},
            Soul = new short[] {2, 4, 6, 8, 10, 1, 2, 3, 4, 5, 1, 2, 3, 4, 5},
            FeatherVnum = 2282,
            FullmoonVnum = 1030,
            GreenSoulVnum = 2283,
            RedSoulVnum = 2284,
            BlueSoulVnum = 2285,
            DragonSkinVnum = 2511,
            DragonBloodVnum = 2512,
            DragonHeartVnum = 2513,
            BlueScrollVnum = 1363,
            RedScrollVnum = 1364
        };

        public UpgradeSpFunConfiguration UpgradeSpFun { get; set; } = new UpgradeSpFunConfiguration
        {
            UpFail = new byte[] {20, 25, 30, 40, 50, 60, 65, 70, 75, 80, 90, 91, 92, 93, 94},
            CostItemVnum = new short[] {5107, 5207, 5519}
        };

        #endregion
    }
}