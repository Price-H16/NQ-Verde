using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using log4net;
using NosQuest.Plugins.Logging;
using NosTale.Configuration;
using NosTale.Configuration.Helper;
using NosTale.Configuration.Utilities;
using NosTale.Parser.Import;
using OpenNos.Core;
using OpenNos.DAL.EF.Helpers;

namespace NosTale.Parser
{
    public class Program
    {
        #region Members

        private static ImportConfiguration configuration;

        #endregion

        #region Methods

        private static void Init()
        {
            // initialize logger
            Logger.InitializeLogger(new SerilogLogger());

            //Logger.InitializeLogger(LogManager.GetLogger(typeof(Program)));
            ConfigurationHelper.CustomisationRegistration();
            configuration = new ImportConfiguration
            {
                Folder = string.Empty,
                Lang = DependencyContainer.Instance.GetInstance<JsonGameConfiguration>().Server.Language,
                Packets = new List<string[]>(),
                LangFolder = string.Empty,
                DatFolder = string.Empty,
                PacketFolder = string.Empty
            };
        }

        private static void Main(string[] args)
        {
            Init();
            Message(args);
            RequiredFiles();
            DataAccessHelper.Initialize();

            try
            {
                Logger.Warn(Language.Instance.GetMessageFromKey("ENTER_PATH"));
                configuration.Folder = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(configuration.Folder))
                {
                    configuration.Folder = Directory.GetCurrentDirectory() + "/parser";
                }

                var folder = configuration.Folder;
                configuration.LangFolder = folder + $"\\Lang\\{configuration.Lang}_{configuration.Lang}";
                configuration.DatFolder = folder + "\\Dat\\";
                configuration.PacketFolder = folder + "\\Packet\\";
                configuration.MapFolder = folder + "\\Map\\";

                new ImportPackets(configuration).Import();

                if (AskToParse($"{Language.Instance.GetMessageFromKey("PARSE_ALL")} [Y/n]").KeyChar != 'n')
                {
                    new ImportMaps(configuration).Import();
                    new ImportSecondaryMaps(configuration).Import();
                    new ImportRespawnMapType().Import();
                    new ImportMapType().Import();
                    new ImportMapTypeMap().Import();
                    new ImportAccounts().Import();
                    new ImportPortals(configuration).Import();
                    new ImportScriptedInstances(configuration).Import();
                    new ImportItems(configuration).Import();
                    new ImportSkills(configuration).Import();
                    new ImportCards(configuration).Import();
                    new ImportNpcMonsters(configuration).Import();
                    new ImportNpcMonsterData(configuration).Import();
                    new ImportDrops().Import();
                    new ImportMapNpcs(configuration).Import();
                    new ImportMonsters(configuration).Import();
                    new ImportShops(configuration).Import();
                    new ImportTeleporters(configuration).Import();
                    new ImportShopItems(configuration).Import();
                    new ImportShopSkills(configuration).Import();
                    new ImportRecipe(configuration).Import();
                    new ImportHardcodedRecipes().Import();
                    new ImportQuests(configuration).Import();
                }
                else
                {
                    if (AskToParse($@"{Language.Instance.GetMessageFromKey("PARSE_MAPS")} [Y/n]").KeyChar != 'n')
                    {
                        new ImportMaps(configuration).Import();
                        new ImportSecondaryMaps(configuration).Import();
                    }

                    if (AskToParse($@"{Language.Instance.GetMessageFromKey("PARSE_MAPTYPES")} [Y/n]").KeyChar != 'n')
                    {
                        new ImportMapType().Import();
                        new ImportMapTypeMap().Import();
                    }

                    if (AskToParse($@"{Language.Instance.GetMessageFromKey("PARSE_ACCOUNTS")} [Y/n]").KeyChar != 'n')
                    {
                        new ImportAccounts().Import();
                    }

                    if (AskToParse($@"{Language.Instance.GetMessageFromKey("PARSE_PORTALS")} [Y/n]").KeyChar != 'n')
                    {
                        new ImportPortals(configuration).Import();
                    }

                    if (AskToParse($@"{Language.Instance.GetMessageFromKey("PARSE_TIMESPACES")} [Y/n]").KeyChar != 'n')
                    {
                        new ImportScriptedInstances(configuration).Import();
                    }

                    if (AskToParse($@"{Language.Instance.GetMessageFromKey("PARSE_ITEMS")} [Y/n]").KeyChar != 'n')
                    {
                        new ImportItems(configuration).Import();
                    }

                    if (AskToParse($@"{Language.Instance.GetMessageFromKey("PARSE_SKILLS")} [Y/n]").KeyChar != 'n')
                    {
                        new ImportSkills(configuration).Import();
                    }

                    if (AskToParse($@"{Language.Instance.GetMessageFromKey("PARSE_MONSTERS")} [Y/n]").KeyChar != 'n')
                    {
                        new ImportNpcMonsters(configuration).Import();
                    }

                    if (AskToParse($@"{Language.Instance.GetMessageFromKey("PARSE_NPCMONSTERDATA")} [Y/n]").KeyChar !=
                        'n')
                    {
                        new ImportNpcMonsterData(configuration).Import();
                    }

                    if (AskToParse($@"{Language.Instance.GetMessageFromKey("PARSE_DROPS")} [Y/n]").KeyChar != 'n')
                    {
                        new ImportDrops().Import();
                    }

                    if (AskToParse($@"{Language.Instance.GetMessageFromKey("PARSE_CARDS")} [Y/n]").KeyChar != 'n')
                    {
                        new ImportCards(configuration).Import();
                    }

                    if (AskToParse($@"{Language.Instance.GetMessageFromKey("PARSE_MAPNPCS")} [Y/n]").KeyChar != 'n')
                    {
                        new ImportMapNpcs(configuration).Import();
                    }

                    if (AskToParse($@"{Language.Instance.GetMessageFromKey("PARSE_MAPMONSTERS")} [Y/n]").KeyChar != 'n')
                    {
                        new ImportMonsters(configuration).Import();
                    }

                    if (AskToParse($@"{Language.Instance.GetMessageFromKey("PARSE_SHOPS")} [Y/n]").KeyChar != 'n')
                    {
                        new ImportShops(configuration).Import();
                    }

                    if (AskToParse($@"{Language.Instance.GetMessageFromKey("PARSE_TELEPORTERS")} [Y/n]").KeyChar != 'n')
                    {
                        new ImportTeleporters(configuration).Import();
                    }

                    if (AskToParse($@"{Language.Instance.GetMessageFromKey("PARSE_SHOPITEMS")} [Y/n]").KeyChar != 'n')
                    {
                        new ImportShopItems(configuration).Import();
                    }

                    if (AskToParse($@"{Language.Instance.GetMessageFromKey("PARSE_SHOPSKILLS")} [Y/n]").KeyChar != 'n')
                    {
                        new ImportShopSkills(configuration).Import();
                    }

                    if (AskToParse($@"{Language.Instance.GetMessageFromKey("PARSE_RECIPES")} [Y/n]").KeyChar != 'n')
                    {
                        new ImportRecipe(configuration).Import();
                        new ImportHardcodedRecipes().Import();
                    }

                    if (AskToParse($@"{Language.Instance.GetMessageFromKey("PARSE_QUESTS")} [Y/n]").KeyChar != 'n')
                    {
                        new ImportQuests(configuration).Import();
                    }
                }

                Console.WriteLine(Language.Instance.GetMessageFromKey("DONE"));
                Console.ReadKey();
            }
            catch (FileNotFoundException ex)
            {
                Logger.Error(Language.Instance.GetMessageFromKey("AT_LEAST_ONE_FILE_MISSING"), ex);
                Console.ReadKey();
            }
        }

        private static ConsoleKeyInfo AskToParse(string msg)
        {
            Console.WriteLine(msg);
            return Console.ReadKey(true);
        }

        private static void Message(string[] args)
        {
            var isDebug = false;
#if DEBUG
            isDebug = true;
#endif
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            Console.Title = $"NosTale Import Console{(isDebug ? " Development Environment" : "")}";

            var ignoreStartupMessages = false;
            foreach (var arg in args)
            {
                ignoreStartupMessages |= arg == "--nomsg";
            }

            if (!ignoreStartupMessages)
            {
                var assembly = Assembly.GetExecutingAssembly();
                var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                var text = $"IMPORT CONSOLE VERSION {fileVersionInfo.ProductVersion} by Zanou";
                var offset = Console.WindowWidth / 2 + text.Length / 2;
                var separator = new string('=', Console.WindowWidth);
                Console.WriteLine(separator + string.Format("{0," + offset + "}\n", text) + separator);
            }
        }

        private static void RequiredFiles()
        {
            Logger.Warn(Language.Instance.GetMessageFromKey("NEED_TREE"));
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine("Root");
            Console.WriteLine($"    \\Lang\\{configuration.Lang}_{configuration.Lang.ToUpper()}");
            Console.ResetColor();
            Console.WriteLine($"    _code_{configuration.Lang}_Card.txt");
            Console.WriteLine($"    _code_{configuration.Lang}_Item.txt");
            Console.WriteLine($"    _code_{configuration.Lang}_MapIDData.txt");
            Console.WriteLine($"    _code_{configuration.Lang}_monster.txt");
            Console.WriteLine($"    _code_{configuration.Lang}_Skill.txt");
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine("    \\Packet\\");
            Console.ResetColor();
            Console.WriteLine("     packet.txt");
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine("    \\Dat\\");
            Console.ResetColor();
            Console.WriteLine("     Card.dat");
            Console.WriteLine("     Item.dat");
            Console.WriteLine("     MapIDData.dat");
            Console.WriteLine("     monster.dat");
            Console.WriteLine("     Skill.dat");
            Console.WriteLine("     quest.dat");
            Console.WriteLine("     qstprize.dat");
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine("     \\Map\\");
            Console.ResetColor();
            Console.WriteLine("     0");
            Console.WriteLine("     1");
            Console.WriteLine("     ...");
        }

        #endregion
    }
}