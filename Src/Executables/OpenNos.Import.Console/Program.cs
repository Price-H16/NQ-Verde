using log4net;
using NosTale.Configuration;
using NosTale.Configuration.Helper;
using NosTale.Configuration.Utilities;
using OpenNos.Core;
using OpenNos.DAL.EF.Helpers;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace OpenNos.Import.Console
{
    public static class Program
    {
        #region Methods

        public static void Main(string[] args)
        {
            bool isDebug = false;
#if DEBUG
            isDebug = true;
#endif
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            System.Console.Title = $"OpenNos Import Console{(isDebug ? " Development Environment" : "")}";

            bool ignoreStartupMessages = false;
            foreach (string arg in args)
            {
                ignoreStartupMessages |= arg == "--nomsg";
            }

            // initialize logger
            Logger.InitializeLogger(LogManager.GetLogger(typeof(Program)));

            ConfigurationHelper.CustomisationRegistration();
            var a = ContainerIoc.GetInstance<JsonGameConfiguration>().Server;

            if (!ignoreStartupMessages)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                string text = $"IMPORT CONSOLE VERSION {fileVersionInfo.ProductVersion} by OpenNos Team";
                int offset = (System.Console.WindowWidth / 2) + (text.Length / 2);
                string separator = new string('=', System.Console.WindowWidth);
                System.Console.WriteLine(separator + string.Format("{0," + offset + "}\n", text) + separator);
            }
            DataAccessHelper.Initialize();
            Logger.Warn(Language.Instance.GetMessageFromKey("NEED_TREE"));
            System.Console.BackgroundColor = ConsoleColor.Blue;
            System.Console.WriteLine("Root");
            System.Console.ResetColor();
            System.Console.WriteLine($"-----_code_{a.Language}_Card.txt");
            System.Console.WriteLine($"-----_code_{a.Language}_Item.txt");
            System.Console.WriteLine($"-----_code_{a.Language}_MapIDData.txt");
            System.Console.WriteLine($"-----_code_{a.Language}_monster.txt");
            System.Console.WriteLine($"-----_code_{a.Language}_Skill.txt");
            System.Console.WriteLine("-----packet.txt");
            System.Console.WriteLine("-----Card.dat");
            System.Console.WriteLine("-----Item.dat");
            System.Console.WriteLine("-----MapIDData.dat");
            System.Console.WriteLine("-----monster.dat");
            System.Console.WriteLine("-----Skill.dat");
            System.Console.BackgroundColor = ConsoleColor.Blue;
            System.Console.WriteLine("-----map");
            System.Console.ResetColor();
            System.Console.WriteLine("----------0");
            System.Console.WriteLine("----------1");
            System.Console.WriteLine("----------...");

            try
            {
                Logger.Warn(Language.Instance.GetMessageFromKey("ENTER_PATH"));

                string folder = System.Console.ReadLine();
                System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_ALL")} [Y/n]");
                ConsoleKeyInfo key = System.Console.ReadKey(true);

                ImportFactory factory = null;
                if (!string.IsNullOrWhiteSpace(folder))
                {
                    factory = new ImportFactory(folder);
                }
                else
                {
                    factory = new ImportFactory(Directory.GetCurrentDirectory() + "/parser");
                }

                factory.ImportPackets();
                if (key.KeyChar != 'n')
                {
                    factory.ImportMaps();
                    factory.LoadMaps();
                    ImportFactory.ImportRespawnMapType();
                    ImportFactory.ImportMapType();
                    ImportFactory.ImportMapTypeMap();
                    ImportFactory.ImportAccounts();
                    factory.ImportPortals();
                    factory.ImportScriptedInstances();
                    factory.ImportItems();
                    factory.ImportSkills();
                    factory.ImportCards();
                    factory.ImportNpcMonsters();
                    factory.ImportNpcMonsterData();
                    factory.ImportMapNpcs();
                    factory.ImportMonsters();
                    factory.ImportShops();
                    factory.ImportTeleporters();
                    factory.ImportShopItems();
                    factory.ImportShopSkills();
                    factory.ImportRecipe();
                    factory.ImportHardcodedItemRecipes();
                    factory.ImportQuests();
                }
                else
                {
                    System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_MAPS")} [Y/n]");
                    key = System.Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportMaps();
                        factory.LoadMaps();
                    }

                    System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_MAPTYPES")} [Y/n]");
                    key = System.Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        ImportFactory.ImportMapType();
                        ImportFactory.ImportMapTypeMap();
                    }

                    System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_ACCOUNTS")} [Y/n]");
                    key = System.Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        ImportFactory.ImportAccounts();
                    }

                    System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_PORTALS")} [Y/n]");
                    key = System.Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportPortals();
                    }

                    System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_TIMESPACES")} [Y/n]");
                    key = System.Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportScriptedInstances();
                    }

                    System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_ITEMS")} [Y/n]");
                    key = System.Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportItems();
                    }

                    System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_SKILLS")} [Y/n]");
                    key = System.Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportSkills();
                    }

                    System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_MONSTERS")} [Y/n]");
                    key = System.Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportNpcMonsters();
                    }

                    System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_NPCMONSTERDATA")} [Y/n]");
                    key = System.Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportNpcMonsterData();
                    }

                    System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_CARDS")} [Y/n]");
                    key = System.Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportCards();
                    }

                    System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_MAPNPCS")} [Y/n]");
                    key = System.Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportMapNpcs();
                    }

                    System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_MAPMONSTERS")} [Y/n]");
                    key = System.Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportMonsters();
                    }

                    System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_SHOPS")} [Y/n]");
                    key = System.Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportShops();
                    }

                    System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_TELEPORTERS")} [Y/n]");
                    key = System.Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportTeleporters();
                    }

                    System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_SHOPITEMS")} [Y/n]");
                    key = System.Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportShopItems();
                    }

                    System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_SHOPSKILLS")} [Y/n]");
                    key = System.Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportShopSkills();
                    }

                    System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_RECIPES")} [Y/n]");
                    key = System.Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportRecipe();
                        factory.ImportHardcodedItemRecipes();
                    }
                    System.Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_QUESTS")} [Y/n]");
                    key = System.Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportQuests();
                    }
                }
                System.Console.WriteLine(Language.Instance.GetMessageFromKey("DONE"));
                System.Console.ReadKey();
            }
            catch (FileNotFoundException ex)
            {
                Logger.Error(Language.Instance.GetMessageFromKey("AT_LEAST_ONE_FILE_MISSING"), ex);
                System.Console.ReadKey();
            }
        }

        #endregion
    }
}