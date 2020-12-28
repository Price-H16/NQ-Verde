//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Threading.Tasks;
//using DSharpPlus.CommandsNext;
//using DSharpPlus.CommandsNext.Attributes;
//using DSharpPlus.Entities;
//using NosTale.Configuration;
//using NosTale.Configuration.Utilities;
//using OpenNos.Core;
//using OpenNos.DAL;
//using OpenNos.Data;
//using OpenNos.Domain;
//using OpenNos.GameObject;
//using OpenNos.GameObject.Helpers;
//using OpenNos.GameObject.Networking;
//using OpenNos.Master.Library.Client;
//using OpenNos.Master.Library.Data;

//namespace NosTale.Bot
//{
//    public class DiscordBotCommand
//    {
//        #region Methods

//        public static int GetHpMax(CharacterDTO player)
//        {
//            return CharacterHelper.HPData[(byte)player.Class, player.Level];
//        }

//        public static string GetImg(CharacterDTO player)
//        {
//            switch (player.Class)
//            {
//                case ClassType.Adventurer:
//                    return player.Gender == GenderType.Female
//                        ? ""
//                        : "";

//                case ClassType.Archer:
//                    return player.Gender == GenderType.Female
//                        ? ""
//                        : "";

//                case ClassType.Magician:
//                    return player.Gender == GenderType.Female
//                        ? ""
//                        : "";

//                case ClassType.Swordsman:
//                    return player.Gender == GenderType.Female
//                        ? ""
//                        : "";

//                case ClassType.MartialArtist:
//                    return player.Gender == GenderType.Female
//                        ? ""
//                        : "";

//                default:
//                    return "";
//            }
//        }

//        public static int GetMpMax(CharacterDTO player)
//        {
//            return CharacterHelper.MPData[(byte)player.Class, player.Level];
//        }

//        public static double GetXpMax(CharacterDTO player)
//        {
//            return CharacterHelper.XPData[player.Level - 1];
//        }

//        [Command("ban")]
//        public async Task Ban(CommandContext ctx, string characterName, string reason, int duration)
//        {
//            if (!CanRunCmd(ctx, DiscordAuthorityType.ADMIN)) return;

//            var character = DAOFactory.CharacterDAO.LoadByName(characterName);
//            if (character != null)
//            {
//                ServerManager.Instance.Kick(characterName);
//                CommunicationServiceClient.Instance.KickSession(character?.AccountId, null);
//                var log = new PenaltyLogDTO
//                {
//                    AccountId = character.AccountId,
//                    Reason = reason?.Trim(),
//                    Penalty = PenaltyType.Banned,
//                    DateStart = DateTime.Now,
//                    DateEnd = duration == 0 ? DateTime.Now.AddYears(15) : DateTime.Now.AddDays(duration),
//                    AdminName = "Discord-Bot"
//                };
//                Character.InsertOrUpdatePenalty(log);
//                await ctx.RespondAsync($"User {character.Name} is now banned for : {reason}");
//            }
//            else
//            {
//                await ctx.RespondAsync("User Not Found");
//            }
//        }

//        public bool CanRunCmd(CommandContext e, DiscordAuthorityType type)
//        {
//            var id = (long)e.Member.Id;
//            var account = DAOFactory.BotAuthorityDAO.LoadById(id);

//            ulong[] channelIdWhitListed =
//                {699578996697989151, 699653585536942200, 699579042172764210, 699579131788263434};
//            if (!channelIdWhitListed.Contains(e.Channel.Id))
//            {
//                e.Message.DeleteAsync("wrong channel");
//                e.RespondAsync("Wrong Channel Please use BOT CHANNEL");
//                return false;
//            }

//            if (account == null)
//            {
//                e.RespondAsync("Please use RegisterMe cmd");
//                return false;
//            }

//            if (account.Authority < type && id != 699578996697989151)
//            {
//                e.RespondAsync("You do not have the permissions required to execute this command");
//                return false;
//            }

//            return true;
//        }

//        [Command("DropRate")]
//        [Aliases("DROP")]
//        public async Task ChangeDropRate(CommandContext e, byte value)
//        {
//            if (!CanRunCmd(e, DiscordAuthorityType.ADMIN)) return;

//            var a = DependencyContainer.Instance.GetInstance<JsonGameConfiguration>();
//            CommunicationServiceClient.Instance.RunGlobalEvent(EventType.DROPRATE, value);
//            await e.RespondAsync($"DropRate was now changed to: {value} Current: {a.Rate.RateXP}");
//        }

//        [Command("FairyXPRATE")]
//        [Aliases("FXP")]
//        public async Task ChangeFairyXPRate(CommandContext e, byte value)
//        {
//            if (!CanRunCmd(e, DiscordAuthorityType.ADMIN)) return;

//            var a = DependencyContainer.Instance.GetInstance<JsonGameConfiguration>();
//            CommunicationServiceClient.Instance.RunGlobalEvent(EventType.FAIRYRATE, value);
//            await e.RespondAsync($"FairyXPRate was now changed to: {value} Current: {a.Rate.RateXP}");
//        }

//        [Command("HeroXPRate")]
//        [Aliases("HXP")]
//        public async Task ChangeHeroXPRate(CommandContext e, byte value)
//        {
//            if (!CanRunCmd(e, DiscordAuthorityType.ADMIN)) return;

//            var a = DependencyContainer.Instance.GetInstance<JsonGameConfiguration>();
//            CommunicationServiceClient.Instance.RunGlobalEvent(EventType.HERORATE, value);
//            await e.RespondAsync($"HeroXPRate was now changed to: {value} Current: {a.Rate.RateXP}");
//        }

//        [Command("RateXP")]
//        [Aliases("XP")]
//        public async Task ChangeXPRate(CommandContext e, byte value)
//        {
//            if (!CanRunCmd(e, DiscordAuthorityType.ADMIN)) return;

//            var a = DependencyContainer.Instance.GetInstance<JsonGameConfiguration>();
//            CommunicationServiceClient.Instance.RunGlobalEvent(EventType.XPRATE, value);
//            await e.RespondAsync($"RatesXP was now changed to: {value} Current: {a.Rate.RateXP}");
//        }

//        [Command("demote")]
//        public async Task Demote(CommandContext ctx, string name)
//        {
//            if (!CanRunCmd(ctx, DiscordAuthorityType.ADMIN)) return;

//            try
//            {
//                var account = DAOFactory.AccountDAO.LoadById(DAOFactory.CharacterDAO.LoadByName(name).AccountId);
//                if (account?.Authority > AuthorityType.User)
//                {
//                    account.Authority = AuthorityType.User;
//                    DAOFactory.AccountDAO.InsertOrUpdate(ref account);
//                    var session =
//                        ServerManager.Instance.Sessions.FirstOrDefault(s => s.Character?.Name == name);
//                    if (session != null)
//                    {
//                        session.Account.Authority = AuthorityType.User;
//                        session.Character.Authority = AuthorityType.User;
//                        ServerManager.Instance.ChangeMap(session.Character.CharacterId);
//                        DAOFactory.AccountDAO.WriteGeneralLog(session.Account.AccountId, session.IpAddress,
//                            session.Character.CharacterId, GeneralLogType.Demotion, "by: Bot-Discord");
//                    }
//                    else
//                    {
//                        DAOFactory.AccountDAO.WriteGeneralLog(account.AccountId, "127.0.0.1", null,
//                            GeneralLogType.Demotion, "by: Bot-Discord");
//                    }

//                    await ctx.RespondAsync($"User {name} has been demoted ");
//                }
//                else
//                {
//                    await ctx.RespondAsync("User Not Found");
//                }
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e);
//            }
//        }

//        public string GenerateNewsIcoRep(CharacterDTO player)
//        {
//            if (player.Reputation <= 250) return "REPUTATION <:nuub:508653458992267264>";
//            if (player.Reputation <= 500) return "REPUTATION <:nuub:508653406349557760>";
//            if (player.Reputation <= 750) return "REPUTATION <:nuub:508653405594583051>";
//            if (player.Reputation <= 1000) return "REPUTATION <:nuub:508653400272142336>";
//            if (player.Reputation <= 2250) return "REPUTATION <:nuub:508653401337233430>";
//            if (player.Reputation <= 3500) return "REPUTATION <:nuub:508653406357815296>";
//            if (player.Reputation <= 5000) return "REPUTATION <:nuub:508653406114676747>";
//            if (player.Reputation <= 9500) return "REPUTATION <:nuub:508653398678175744>";
//            if (player.Reputation <= 19000) return "REPUTATION <:nuub:508653405904830469>";
//            if (player.Reputation <= 25000) return "REPUTATION <:nuub:508653396501331978>";
//            if (player.Reputation <= 40000) return "REPUTATION <:nuub:508653401899401236>";
//            if (player.Reputation <= 60000) return "REPUTATION <:nuub:508653406236442625>";
//            if (player.Reputation <= 85000) return "REPUTATION <:nuub:508653402021036076>";
//            if (player.Reputation <= 115000) return "REPUTATION <:nuub:508653406702010388>";
//            if (player.Reputation <= 150000) return "REPUTATION <:nuub:508653406634639360>";
//            if (player.Reputation <= 190000) return "REPUTATION <:nuub:508653406609473547>";
//            if (player.Reputation <= 235000) return "REPUTATION <:nuub:508653407129829397>";
//            if (player.Reputation <= 285000) return "REPUTATION <:nuub:508653401769246750>";
//            if (player.Reputation <= 350000) return "REPUTATION <:nuub:508653407037423642>";
//            if (player.Reputation <= 500000) return "REPUTATION <:nuub:508653402150928394>";
//            if (player.Reputation <= 1500000) return "REPUTATION <:nuub:508653406265802753>";
//            if (player.Reputation <= 2500000) return "REPUTATION <:nuub:508653402268368900>";
//            if (player.Reputation <= 3750000) return "REPUTATION <:nuub:508653400251039747>";
//            if (player.Reputation <= 5000000) return "REPUTATION <:nuub:508653400615944218>";
//            if (player.Reputation <= 7500000) return "REPUTATION <:nuub:508653402096402472>";
//            if (player.Reputation <= 500000) return "REPUTATION <:greennos:>";
//            if (player.Reputation <= 1500000) return "REPUTATION <:bluenos:>";
//            if (player.Reputation <= 2000000) return "REPUTATION <:rednos:>";
//            if (player.Reputation <= 3000000) return "REPUTATION <:greenelite:>";
//            if (player.Reputation <= 3000000) return "REPUTATION <:greenelite:>";
//            if (player.Reputation <= 5000000) return "REPUTATION <:blueelite:>";

//            return player.Reputation <= 50000000 ? "REPUTATION :redelite:" : "REPUTATION :redelite:";
//        }

//        [Command("help")]
//        public async Task Help(CommandContext ctx)
//        {


//            await ctx.TriggerTypingAsync();

//            var builder = new DiscordEmbedBuilder();
//            builder.WithTitle("Command Info");
//            builder.AddField("Start <Amount> <isAct4>", "[++Start Number Bool] Launch X World ( Channel )");
//            builder.AddField("Restart", "[++Restart] Restart All channel ( 30 sec )"); // Like $Restart ig
//            builder.AddField("Searchitem <Name>", "[++SearchItem Name] Give Item Vnum"); // Like $SearchItem IG
//            builder.AddField("SearchMonster <Name>",
//                "[++SearchMonster Name] Give Monster Vnum"); // Like $SearchMonster IG
//            builder.AddField("ShowInfo <Name>", "[++ShowInfo Name] Give Character Info"); // Like $SearchMonster IG
//            builder.AddField("Shout <Value>", "[++Shout Value] Send Shout Ig"); // Like $Shout Ig
//            builder.AddField("Kick <Value>", "[++Kick Value] Kick User"); // Like $KickSession Ig
//            builder.AddField("Event <Value>", "[++Event Value] Start X Event on All channel"); // Like $GlobalEvent Ig
//            builder.AddField("Ban", "[++Ban <Name> <Reason> <Duration>] Ban an user");
//            builder.AddField("Stat", "[++Stat] Show the Amount of player online"); // Like $Ban Ig
//            builder.AddField("UnBan", "[++UnBan <Name>] UnBan an user"); // Like $UnBan Ig
//            builder.AddField("Mute", "[++Mute <Name> <Reason> <Duration>] Mute an user"); // Like $Mute Ig
//            builder.AddField("UnMute", "[++UnMute <Name>] UnMute an user"); // Like $UnMute Ig
//            builder.AddField("Demote", "[++Demote <Name>] Demote an user"); // Like $demote Ig
//            builder.AddField("Help", "[++Help] Show All Cmd available"); // Like $Help Ig
//            builder.AddField("RegisterMe", "[++RegisterMe] Register you discord account");
//            builder.AddField("PromoteME", "[++PromoteMe Value] Promote me to X authority");
//            builder.AddField("PromoteYou", "[++PromoteYou DiscordId] Promote one User with discordId");
//            builder.AddField("XPRate", "[++XPRate value] Change XPRATE");
//            builder.AddField("FairyXPRate", "[++FairyXPRate value] Change FairyXPRate");
//            builder.AddField("HeroXPRate", "[++HeroXPRate value] Change HeroXPRate");
//            builder.AddField("DropRate", "[++DropRate value] Change DropRate");
//            builder.WithColor(DiscordColor.Chartreuse);

//            //        TODO: //Add Cmd =>   Gift / SearchMap / SearchSkill

//            await ctx.RespondAsync(embed: builder);
//        }

//        [Command("kick")]
//        public async Task Kick(CommandContext ctx, string characterName)
//        {
//            if (!CanRunCmd(ctx, DiscordAuthorityType.GM)) return;

//            var character = DAOFactory.CharacterDAO.LoadByName(characterName);
//            if (character != null)
//            {
//                var account = DAOFactory.AccountDAO.LoadByName(character.Name);
//                ServerManager.Instance.Kick(characterName);
//                CommunicationServiceClient.Instance.KickSession(character?.AccountId, null);
//                await ctx.RespondAsync($"User {character.Name} has been kicked");
//            }
//            else
//            {
//                await ctx.RespondAsync("User Not Found");
//            }
//        }

//        [Command("Mute")]
//        public async Task Mute(CommandContext e, string name, [RemainingText] string reason, int duration)
//        {
//            if (!CanRunCmd(e, DiscordAuthorityType.GM)) return;

//            if (duration == 0) duration = 60;

//            var characterToMute = DAOFactory.CharacterDAO.LoadByName(name);
//            if (characterToMute != null) await e.RespondAsync("User Not Found");
//            var session = ServerManager.Instance.GetSessionByCharacterName(name);
//            if (session?.Character.IsMuted() == false)
//                session.SendPacket(UserInterfaceHelper.GenerateInfo(
//                    string.Format(Language.Instance.GetMessageFromKey("MUTED_PLURAL"), reason, duration)));

//            var log = new PenaltyLogDTO
//            {
//                AccountId = characterToMute.AccountId,
//                Reason = reason,
//                Penalty = PenaltyType.Muted,
//                DateStart = DateTime.Now,
//                DateEnd = DateTime.Now.AddMinutes(duration),
//                AdminName = "Bot-Discord"
//            };
//            Character.InsertOrUpdatePenalty(log);

//            await e.RespondAsync("User Muted");
//        }

//        [Command("PromoteMe")]
//        [RequireOwner]
//        public async Task PromoteMe(CommandContext e, string type)
//        {
//            try
//            {
//                if (!CanRunCmd(e, DiscordAuthorityType.Player)) return;

//                var id = (long)e.Member.Id;
//                var output = (DiscordAuthorityType)Enum.Parse(typeof(DiscordAuthorityType), type);

//                var account = new BotAuthorityDTO
//                {
//                    Authority = output,
//                    DiscordId = id
//                };

//                DAOFactory.BotAuthorityDAO.InsertOrUpdate(ref account);
//                await e.RespondAsync($"Your account Authority is now changed to : {output}");
//            }
//            catch
//            {
//                await e.RespondAsync("Only Player/Helper/GM/ADMIN rank is available");
//            }
//        }

//        [Command("PromoteYou")]
//        public async Task PromoteYou(CommandContext e, string type, long discordId)
//        {
//            try
//            {
//                if (!CanRunCmd(e, DiscordAuthorityType.ADMIN)) return;

//                var id = discordId;
//                var output = (DiscordAuthorityType)Enum.Parse(typeof(DiscordAuthorityType), type);

//                var account = DAOFactory.BotAuthorityDAO.LoadById(id);
//                if (account == null)
//                {
//                    await e.RespondAsync(
//                        "User dont have any account, since im a good bot I have created an account for this user ");
//                    account = new BotAuthorityDTO
//                    {
//                        Authority = DiscordAuthorityType.Player,
//                        DiscordId = id
//                    };
//                }
//                else
//                {
//                    account = new BotAuthorityDTO
//                    {
//                        Authority = output,
//                        DiscordId = id
//                    };
//                    await e.RespondAsync($"The account Authority is now changed to : {output}");
//                }

//                DAOFactory.BotAuthorityDAO.InsertOrUpdate(ref account);
//            }
//            catch
//            {
//                await e.RespondAsync("Only Player/Helper/GM/ADMIN rank is available");
//            }
//        }

//        [Command("Restart")]
//        public async Task RebootAll(CommandContext e)
//        {
//            if (!CanRunCmd(e, DiscordAuthorityType.ADMIN)) return;

//            CommunicationServiceClient.Instance.RunGlobalEvent(EventType.AUTOREBOOT);
//            await e.RespondAsync("ALL channel gonna reboot in 30 sec");
//        }

//        [Command("RegisterMe")]
//        public async Task RegisterMe(CommandContext ctx)
//        {
//            var id = (long)ctx.Member.Id;
//            var account = DAOFactory.BotAuthorityDAO.LoadById(id);
//            if (account != null)
//            {
//                await ctx.RespondAsync("Already Registered");
//                return;
//            }

//            account = new BotAuthorityDTO
//            {
//                Authority = DiscordAuthorityType.Player,
//                DiscordId = id
//            };

//            DAOFactory.BotAuthorityDAO.InsertOrUpdate(ref account);
//            await ctx.RespondAsync("You are Registered guys");
//        }

//        [Command("ResetRate")]
//        public async Task ResetRate(CommandContext e)
//        {
//            if (!CanRunCmd(e, DiscordAuthorityType.ADMIN)) return;

//            CommunicationServiceClient.Instance.RunGlobalEvent(EventType.RESETRATE);
//            await e.RespondAsync("All RateXp was reseted");
//        }

//        [Command("Searchitem")]
//        [Aliases("si")]
//        public async Task SearchItem(CommandContext ctx, [RemainingText] string Msg)
//        {
//            if (!CanRunCmd(ctx, DiscordAuthorityType.Helper)) return;

//            var contents = Msg;
//            var name = string.Empty;
//            byte page = 0;
//            if (!string.IsNullOrEmpty(contents))
//            {
//                var packetsplit = contents.Split(' ');
//                var withPage = byte.TryParse(packetsplit[0], out page);
//                name = packetsplit.Length == 1 && withPage
//                    ? string.Empty
//                    : packetsplit.Skip(withPage ? 1 : 0).Aggregate((a, b) => a + ' ' + b);
//            }

//            IEnumerable<ItemDTO> itemlist = DAOFactory.ItemDAO.FindByName(name).OrderBy(s => s.VNum)
//                .Skip(page * 200).Take(200).ToList();
//            if (itemlist.Any())
//            {
//                var aa = string.Empty;
//                foreach (var item in itemlist)
//                    aa +=
//                        $"\nItem: {(string.IsNullOrEmpty(item.Name) ? "none" : item.Name)} VNum: {item.VNum} Effect/Value: {item.Effect}/{item.EffectValue} Type: {item.ItemType}";
//                await ctx.RespondAsync($"{aa}");
//            }
//            else
//            {
//                await ctx.RespondAsync("Not found");
//            }
//        }

//        [Command("SearchMonster")]
//        [Aliases("sm")]
//        public async Task SearchMonster(CommandContext ctx, [RemainingText] string Msg)
//        {
//            if (!CanRunCmd(ctx, DiscordAuthorityType.Helper)) return;

//            var contents = Msg;
//            var name = string.Empty;
//            byte page = 0;
//            if (!string.IsNullOrEmpty(contents))
//            {
//                var packetsplit = contents.Split(' ');
//                var withPage = byte.TryParse(packetsplit[0], out page);
//                name = packetsplit.Length == 1 && withPage
//                    ? string.Empty
//                    : packetsplit.Skip(withPage ? 1 : 0).Aggregate((a, b) => a + ' ' + b);
//            }

//            IEnumerable<NpcMonsterDTO> monsterlist = DAOFactory.NpcMonsterDAO.FindByName(name)
//                .OrderBy(s => s.NpcMonsterVNum).Skip(page * 200).Take(200).ToList();
//            if (monsterlist.Any())
//            {
//                var aa = "";
//                foreach (var npcMonster in monsterlist)
//                    aa +=
//                        $"\nMonster: {(string.IsNullOrEmpty(npcMonster.Name) ? "none" : npcMonster.Name)} VNum: {npcMonster.NpcMonsterVNum}";
//                await ctx.RespondAsync($"{aa}");
//            }
//            else
//            {
//                await ctx.RespondAsync("Not found");
//            }
//        }

//        [Command("Shout")]
//        public async Task Shout(CommandContext ctx, [RemainingText] string Msg)
//        {
//            if (!CanRunCmd(ctx, DiscordAuthorityType.GM)) return;

//            CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
//            {
//                DestinationCharacterId = null,
//                SourceCharacterId = 1,
//                SourceWorldId = ServerManager.Instance.WorldId,
//                Message = Msg,
//                Type = OpenNos.Domain.MessageType.Shout
//            });
//            await ctx.RespondAsync(" Shout Sent !");
//        }

//        [Command("ShowInfo")]
//        public async Task ShowInfo(CommandContext ctx, string name)
//        {
//            if (!CanRunCmd(ctx, DiscordAuthorityType.Player)) return;

//            if (DAOFactory.CharacterDAO.LoadByName(name) != null)
//                try
//                {
//                    var characterDto = DAOFactory.CharacterDAO.LoadByName(name);
//                    var account = DAOFactory.AccountDAO.LoadById(characterDto.AccountId);
//                    IEnumerable<PenaltyLogDTO> penaltyLogs =
//                        DAOFactory.PenaltyLogDAO.LoadByAccount(account.AccountId).ToList();
//                    var penalty = penaltyLogs.LastOrDefault(s => s.DateEnd > DateTime.Now);

//                    var tmp = new DiscordEmbedBuilder
//                    {
//                        Author = new DiscordEmbedBuilder.EmbedAuthor
//                        {
//                            Name = $"{characterDto.Name}'s Character",
//                            IconUrl = ctx.Member.AvatarUrl
//                        },
//                        Color = new DiscordColor(0xFFA500),
//                        Footer = new DiscordEmbedBuilder.EmbedFooter
//                        {
//                            IconUrl = ctx.Member.AvatarUrl,
//                            Text = $"{ctx.Member.Username}#{ctx.Member.Discriminator}"
//                        },

//                    };
//                    var bio = characterDto.Biography != null ? characterDto.Biography : "";
//                    tmp.AddField($"CharacterId : {characterDto.CharacterId}", "Stats");
//                    tmp.AddField("HP :heart:", $"{characterDto.Hp}/{GetHpMax(characterDto)}", true);
//                    tmp.AddField("MP :star:", $"{characterDto.Mp}/0", true);
//                    tmp.AddField("XP <:xp:508349291308253219>",
//                        "Level" + characterDto.Level + $" (+{characterDto.HeroLevel})" +
//                        $" [JLvl {characterDto.JobLevel}]", true); //+$" Xp [{characterDto.LevelXp}/0]"
//                    tmp.AddField("Gold :gold:", characterDto.Gold.ToString(), true);
//                    tmp.AddField(GenerateNewsIcoRep(characterDto), characterDto.Reputation.ToString(), true);
//                    tmp.AddField("Popularity <:popu:510849043635634177>", characterDto.Compliment.ToString());
//                    tmp.AddField("Faction",
//                        $"{(characterDto.Faction == FactionType.Demon ? Language.Instance.GetMessageFromKey("DEMON") : Language.Instance.GetMessageFromKey("ANGEL"))}",
//                        true);

//                    tmp.AddField("Bio", $"{bio}", true); //Search Another way
//                    tmp.AddField("State", characterDto.State.ToString(), true);
//                    tmp.AddField("Dignity", characterDto.Dignity.ToString(), true);
//                    tmp.AddField("Rage", characterDto.RagePoint.ToString(), true);
//                    tmp.AddField("MapId", characterDto.MapId.ToString(), true);
//                    tmp.AddField("MapX", characterDto.MapX.ToString(), true);
//                    tmp.AddField("MapY", characterDto.MapY.ToString(), true);
//                    if (penalty != null)
//                    {
//                        tmp.AddField("PENALTY :", "Value");
//                        tmp.AddField("Type:", $"{penalty.Penalty}", true);
//                        tmp.AddField("AdminName:", $"{penalty.AdminName}", true);
//                        tmp.AddField("Reason:", $"{penalty.Reason}", true);
//                        tmp.AddField("DateStart:", $"{penalty.DateStart}", true);
//                        tmp.AddField("DateEnd:", $"{penalty.DateEnd}", true);
//                    }

//                    tmp.AddField("Bans", $"{penaltyLogs.Count(s => s.Penalty == PenaltyType.Banned)}", true);
//                    tmp.AddField("Mutes", $"{penaltyLogs.Count(s => s.Penalty == PenaltyType.Muted)}", true);
//                    tmp.AddField("Warnings", $"{penaltyLogs.Count(s => s.Penalty == PenaltyType.Warning)}", true);
//                    if (account != null) tmp.AddField("Authority:", $"{account.Authority}", true);

//                    await ctx.RespondAsync(embed: tmp);
//                }
//                catch (Exception e)
//                {
//                    Console.WriteLine(e);
//                }
//            else
//                await ctx.RespondAsync("Not found");
//        }

//        [Command("start")]
//        public async Task StartChannel(CommandContext e, byte amount, bool isAct4 = false)
//        {
//            if (!CanRunCmd(e, DiscordAuthorityType.ADMIN)) return;

//            var a = DependencyContainer.Instance.GetInstance<JsonGameConfiguration>();

//            var arg = isAct4 ? $"--port {a.Server.Act4Port}" : string.Empty;

//            amount = (byte)(isAct4 ? 1 : amount);
//            for (byte i = 0; i != amount; i++) Process.Start("..\\World\\OpenNos.World.exe", "--nomsg " + arg);

//            await e.RespondAsync($"{amount} World Channel Started!");
//        }

//        [Command("Stat")]
//        public async Task Stat(CommandContext e)
//        {
//            if (!CanRunCmd(e, DiscordAuthorityType.Helper)) return;

//            foreach (var message in CommunicationServiceClient.Instance.RetrieveServerStatistics(false))
//                await e.RespondAsync(message);
//        }

//        [Command("UnBan")]
//        public async Task UnBan(CommandContext e, string name)
//        {
//            if (!CanRunCmd(e, DiscordAuthorityType.ADMIN)) return;

//            var chara = DAOFactory.CharacterDAO.LoadByName(name);
//            if (chara == null) await e.RespondAsync("User Not Found");
//            var log = ServerManager.Instance.PenaltyLogs.Find(s =>
//                s.AccountId == chara.AccountId && s.Penalty == PenaltyType.Banned && s.DateEnd > DateTime.Now);
//            if (log == null) await e.RespondAsync("User Not Banned");

//            log.DateEnd = DateTime.Now.AddSeconds(-1);
//            Character.InsertOrUpdatePenalty(log);

//            await e.RespondAsync("User UnBanned");
//        }

//        [Command("UnMute")]
//        public async Task UnMute(CommandContext e, string name)
//        {
//            if (!CanRunCmd(e, DiscordAuthorityType.GM)) return;

//            var chara = DAOFactory.CharacterDAO.LoadByName(name);
//            if (chara == null) await e.RespondAsync("User Not Found");

//            if (!ServerManager.Instance.PenaltyLogs.Any(s =>
//                s.AccountId == chara.AccountId && s.Penalty == (byte)PenaltyType.Muted
//                                               && s.DateEnd > DateTime.Now))
//                await e.RespondAsync("User Not Muted");

//            var log = ServerManager.Instance.PenaltyLogs.Find(s =>
//                s.AccountId == chara.AccountId && s.Penalty == (byte)PenaltyType.Muted
//                                               && s.DateEnd > DateTime.Now);
//            if (log != null)
//            {
//                log.DateEnd = DateTime.Now.AddSeconds(-1);
//                Character.InsertOrUpdatePenalty(log);
//            }

//            await e.RespondAsync("User UnMuted");
//        }

//        #endregion
//    }
//}