using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;

namespace OpenNos.Handler.PacketHandler.Basic
{
    public class GameStartPacketHandler : IPacketHandler
    {
        #region Instantiation

        public GameStartPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public void StartGame(GameStartPacket gameStartPacket)
        {
            #region System Code Lock
            if (ServerManager.Instance.Configuration.LockSystem)
            {
                if (Session.Character.LockCode != null)
                {
                    #region Lock Code
                    Session.Character.HeroChatBlocked = true;
                    Session.Character.ExchangeBlocked = true;
                    Session.Character.WhisperBlocked = true;
                    Session.Character.Invisible = true;
                    Session.Character.NoAttack = true;
                    Session.Character.NoMove = true;
                    Session.Character.VerifiedLock = false;
                    #endregion
                    Session.SendPacket(Session.Character.GenerateSay($"Your account doesn't have a lock. If you want more security, use $SetLock and a code.", 12));
                }
                else
                {
                    #region Unlock Code
                    Session.Character.HeroChatBlocked = false;
                    Session.Character.ExchangeBlocked = false;
                    Session.Character.WhisperBlocked = false;
                    Session.Character.Invisible = false;
                    Session.Character.NoAttack = false;
                    Session.Character.NoMove = false;
                    Session.Character.VerifiedLock = true;
                    #endregion
                    Session.SendPacket(Session.Character.GenerateSay($"Your account is locked. Please, use $Unlock command.", 12));
                }
            }
            else
            {
                Session.Character.VerifiedLock = true;
            }
            #endregion

            if (Session?.Character == null || Session.IsOnMap || !Session.HasSelectedCharacter)
            // character should have been selected in SelectCharacter
            {
                return;
            }

            var shouldRespawn = false;

            if (Session.Character.MapInstance?.Map?.MapTypes != null)
            {
                if (Session.Character.MapInstance.Map.MapTypes.Any(m => m.MapTypeId == (short)MapTypeEnum.Act4)
                 && ServerManager.Instance.ChannelId != 51)
                {
                    if (ServerManager.Instance.IsAct4Online())
                    {
                        Session.Character.ChangeChannel(ServerManager.Instance.Configuration.Act4IP,
                                ServerManager.Instance.Configuration.Act4Port, 2, false);
                        return;
                    }

                    shouldRespawn = true;
                }
            }

            Session.CurrentMapInstance = Session.Character.MapInstance;

            //if (ServerManager.Instance.Configuration.SceneOnCreate && Session.Character.GeneralLogs.CountLinq(s => s.LogType == "Connection") < 2)
            //{
            //    Session.SendPacket("scene 40");
            //}
            if (ServerManager.Instance.Configuration.WorldInformation)
            {
                var assembly = Assembly.GetEntryAssembly();
                var productVersion = assembly?.Location != null
                    ? FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion
                    : "1337";

                Session.SendPacket(Session.Character.GenerateSay("------------------[NosQuest]------------------", 10));
                Session.SendPacket(Session.Character.GenerateSay("Website: https://nosquestreborn.com", 12)); 
                Session.SendPacket(Session.Character.GenerateSay("Discord: https://discord.gg/zM5JxBK", 12));
                Session.SendPacket(Session.Character.GenerateSay("------------------[Counter]--------------------", 10));
                Session.SendPacket(Session.Character.GenerateSay($"Mob Kill Counter: {Session.Character.MobKillCounter.ToString("###,##0")}", 10));
                Session.SendPacket(Session.Character.GenerateSay("--------------------------------------------------", 10));
                //#region Count Online Players

                //foreach (string message in CommunicationServiceClient.Instance.RetrieveServerStatisticsPlayer())
                //{
                //    Session.SendPacket(Session.Character.GenerateSay(message, 13));
                //}

                //#endregion
            }

            if (Session?.Character?.Level == 1)
            {
                Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(s => Session.SendPacket(Session.Character.GenerateSay(Session.Character.Name + ", Welcome and enjoy your stay in NosQuest!", 12)));
            }
            else
            {
                Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(s => Session.SendPacket(Session.Character.GenerateSay("Welcome back, " + Session.Character.Name, 12)));
                //Session.SendPacket(Session.Character.GenerateSay("You have access to $Warp - Unlock maps as you visit them!", 12));
                Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(s => Session.SendPacket(Session.Character.GenerateSay("Stay updated and check #upcoming-patch in our discord!", 12)));
            }

            Session.Character.LoadSpeed();
            Session.Character.LoadSkills();
            Session.SendPacket(Session.Character.GenerateCInfo());
            Session.SendPacket("c_info_reset");
            Session.SendPacket(Session.Character.GenerateTit());
            Session.SendPacket(Session.Character.GenerateSpPoint());
            Session.SendPacket(Session.Character.GenerateRsfi());
            Session.SendPacket(Session.Character.GenerateEventIcon());


            Session.Character.Quests?.Where(q => q?.Quest?.TargetMap != null).ToList().ForEach(qst => Session.SendPacket(qst.Quest.TargetPacket()));

            if (Session.Character.Hp <= 0 && (!Session.Character.IsSeal || ServerManager.Instance.ChannelId != 51))
            {
                ServerManager.Instance.ReviveFirstPosition(Session.Character.CharacterId);
            }
            else
            {
                if (shouldRespawn)
                {
                    var resp = Session.Character.Respawn;
                    var x = (short)(resp.DefaultX + ServerManager.RandomNumber(-3, 3));
                    var y = (short)(resp.DefaultY + ServerManager.RandomNumber(-3, 3));
                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId, resp.DefaultMapId, x, y);
                }
                else
                {
                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId);
                }
            }

            Session.SendPacket(Session.Character.GenerateSki());
            Session.SendPacket($"fd {Session.Character.Reputation} 0 {(int)Session.Character.Dignity} {Math.Abs(Session.Character.GetDignityIco())}");
            Session.SendPacket(Session.Character.GenerateFd());
            Session.SendPacket("rage 0 250000");
            Session.SendPacket("rank_cool 0 0 18000");
            var specialistInstance = Session.Character.Inventory.LoadBySlotAndType(8, InventoryType.Wear);

            #region Check StaticBonusType

            StaticBonusDTO medal = Session.Character.StaticBonusList.Find(s => s.StaticBonusType == StaticBonusType.BazaarMedalGold || s.StaticBonusType == StaticBonusType.BazaarMedalSilver);

            StaticBonusDTO buffs = Session.Character.StaticBonusList.Find(s => s.StaticBonusType == StaticBonusType.MedalOfErenia);

            if (medal != null)
            {
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("LOGIN_MEDAL"), 12));
            }

            var autoloot = Session.Character.StaticBonusList.Find(s => s.StaticBonusType == StaticBonusType.AutoLoot);
            if (autoloot != null)
            {
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("AUTOLOOT_STILL_ENABLED"), 12));
            }

            if (Session.Character.StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.PetBasket))
            {
                Session.SendPacket("ib 1278 1");
            }

            #endregion 

            if (Session.Character.MapInstance?.Map?.MapTypes?.Any(m => m.MapTypeId == (short)MapTypeEnum.CleftOfDarkness) == true)
            {
                Session.SendPacket("bc 0 0 0");
            }

            if (specialistInstance != null)
            {
                Session.SendPacket(Session.Character.GenerateSpPoint());
            }

            Session.SendPacket("scr 0 0 0 0 0 0");
            for (var i = 0; i < 10; i++)
            {
                Session.SendPacket($"bn {i} {Language.Instance.GetMessageFromKey($"BN{i}")}");
            }

            Session.SendPacket(Session.Character.GenerateExts());
            Session.SendPacket(Session.Character.GenerateMlinfo());
            Session.SendPacket(UserInterfaceHelper.GeneratePClear());
            Session.SendPacket(Session.Character.GeneratePetskill());

            Session.SendPacket(Session.Character.GeneratePinit());
            Session.SendPackets(Session.Character.GeneratePst());

            Session.SendPacket("zzim");
            Session.SendPacket($"twk 1 {Session.Character.CharacterId} {Session.Account.Name} {Session.Character.Name} shtmxpdlfeoqkr");


            var familyId = DAOFactory.FamilyCharacterDAO.LoadByCharacterId(Session.Character.CharacterId)?.FamilyId;
            if (familyId.HasValue)
            {
                Session.Character.Family = ServerManager.Instance.FamilyList[familyId.Value];
            }

            if (Session.Character.Family != null && Session.Character.FamilyCharacter != null)
            {
                if (Session.Character.Faction != (FactionType)Session.Character.Family.FamilyFaction)
                {
                    Session.Character.Faction = (FactionType)Session.Character.Family.FamilyFaction;

                }

                Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateGidx());
                Session.SendPacket(Session.Character.GenerateGInfo());
                Session.SendPackets(Session.Character.GetFamilyHistory());
                Session.SendPacket(Session.Character.GenerateFamilyMember());
                Session.SendPacket(Session.Character.GenerateFamilyMemberMessage());
                Session.SendPacket(Session.Character.GenerateFamilyMemberExp());
                //Session.SendPacket($"gcon {Session.Character.CharacterId}|1|0");
                /*Session.SendPacket("fmi 0|9002|2|0|0 " +
                                   "0|9003|2|0|0 " +
                                   "0|9004|2|3|8 " +
                                   "0|9005|2|1|8 " +
                                   "0|9006|2|0|26 " +
                                   "0|9007|2|0|0 " +
                                   "0|9008|2|0|6 " +
                                   "0|9009|2|3|0 " +
                                   "0|9010|2|2|0 " +
                                   "0|9011|2|0|0 " +
                                   "0|9012|2|0|0 " +
                                   "0|9013|2|0|0 " +
                                   "0|9014|2|4|0 " +
                                   "0|9015|2|1|0 " +
                                   "0|9016|2|1|0 " +
                                   "0|9017|2|7|13 " +
                                   "1|9018|1|20190707|0 " +
                                   "1|9019|1|20190722|0 " +
                                   "1|9020|1|20190921|0 " +
                                   "1|9021|2|0|0 " +
                                   "1|9037|1|20190719|0 " +
                                   "1|9038|2|71|0 " +
                                   "1|9042|1|20190722|0 " +
                                   "1|9043|2|94|0 " +
                                   "1|9047|1|20190721|0 " +
                                   "1|9048|2|45|0 " +
                                   "1|9052|1|20190703|0 " +
                                   "1|9053|2|42|0 " +
                                   "1|9055|1|20190702|0 " +
                                   "1|9056|2|5|0 " +
                                   "1|9060|1|20190519|0 " +
                                   "1|9061|1|20190708|0 " +
                                   "1|9062|2|23|0 " +
                                   "1|9065|1|20190707|0 " +
                                   "1|9066|2|8|0 " +
                                   "1|9070|1|20190629|0 " +
                                   "1|9071|2|6|0 " +
                                   "0|9077|2|6|0 " +
                                   "0|9078|2|0|40 " +
                                   "0|9079|2|0|0");
                Session.SendPacket("fmp 9747|0 9732|0 9742|0 9738|0");*/

                if (!string.IsNullOrWhiteSpace(Session.Character.Family.FamilyMessage))
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateInfo("--- Family Message ---\n" + Session.Character.Family.FamilyMessage));

                }
            }
            RewardsHelper.Instance.MobKillRewards(Session);
            RewardsHelper.Instance.DailyReward(Session);
            Session.SendPacket(Session.Character.GetSqst());
            Session.SendPacket("act6");
            Session.SendPacket(Session.Character.GenerateFaction());
            Session.SendPackets(Session.Character.GenerateScP());
            Session.SendPackets(Session.Character.GenerateScN());
#pragma warning disable 618
            Session.Character.GenerateStartupInventory();
#pragma warning restore 618

            Session.SendPacket(Session.Character.GenerateGold());
            Session.SendPackets(Session.Character.GenerateQuicklist());

            var clinit = "clinit";
            var flinit = "flinit";
            var kdlinit = "kdlinit";
            foreach (var character in ServerManager.Instance.TopComplimented)
            {
                clinit += $" {character.CharacterId}|{character.Level}|{character.HeroLevel}|{character.Compliment}|{character.Name}";

            }

            foreach (var character in ServerManager.Instance.TopReputation)
            {
                flinit += $" {character.CharacterId}|{character.Level}|{character.HeroLevel}|{character.Reputation}|{character.Name}";

            }

            foreach (var character in ServerManager.Instance.TopPoints)
            {
                kdlinit += $" {character.CharacterId}|{character.Level}|{character.HeroLevel}|{character.Act4Points}|{character.Name}";

            }

            Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateGidx());

            Session.SendPacket(Session.Character.GenerateFinit());
            Session.SendPacket(Session.Character.GenerateBlinit());
            Session.SendPacket(clinit);
            Session.SendPacket(flinit);
            Session.SendPacket(kdlinit);

            Session.Character.LastPVPRevive = DateTime.Now;

            var warning = DAOFactory.PenaltyLogDAO.LoadByAccount(Session.Character.AccountId).Where(p => p.Penalty == PenaltyType.Warning);
            if (warning.Any())
            {
                Session.SendPacket(UserInterfaceHelper.GenerateInfo(string.Format(Language.Instance.GetMessageFromKey("WARNING_INFO"), warning.Count())));

            }

            //Messagge GM
            //if (Session.Character.Authority == AuthorityType.Administrator)

            //{
            //    CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage()
            //    {
            //        DestinationCharacterId = null,
            //        SourceCharacterId = Session.Character.CharacterId,
            //        SourceWorldId = ServerManager.Instance.WorldId,
            //        Message = $"Welcome Now Game Master {Session.Character.Name} to NQ!",
            //        Type = MessageType.Shout
            //    });
            //}

            //Messagge GameMaster
            if (Session.Character.Authority == AuthorityType.Administrator)
            {
                Session.SendPacket(Session.Character.GenerateSay("==========Owner==========", 10));
                Session.SendPacket(Session.Character.GenerateSay("Welcome " + Session.Character.Name, 12));
                Session.SendPacket(Session.Character.GenerateSay("Use $Bank Help for info about Bank.", 10));
                Session.SendPacket(Session.Character.GenerateSay("Use $HelpMe to contact a NQ Team", 10));
                Session.SendPacket(Session.Character.GenerateSay("Use $Warp + Name to Move Map", 10));
                Session.SendPacket(Session.Character.GenerateSay("Use $Help to see the list of additional commands available", 10));
                Session.SendPacket(Session.Character.GenerateSay("Use $CheckStat for check status Glacer and Act6", 10));
                Session.SendPacket(Session.Character.GenerateSay("=========================", 10));
            }

            //Messagge GameMaster
            if (Session.Character.Authority == AuthorityType.GM)
            {
                Session.SendPacket(Session.Character.GenerateSay("============GM===========", 10));
                Session.SendPacket(Session.Character.GenerateSay("Welcome " + Session.Character.Name, 12));
                Session.SendPacket(Session.Character.GenerateSay("Use $Bank Help for info about Bank.", 10));
                Session.SendPacket(Session.Character.GenerateSay("Use $HelpMe to contact a NQ Team", 10));
                Session.SendPacket(Session.Character.GenerateSay("Use $Warp + Name to Move Map", 10));
                Session.SendPacket(Session.Character.GenerateSay("Use $Help to see the list of additional commands available", 10));
                Session.SendPacket(Session.Character.GenerateSay("Use $CheckStat for check status Glacer and Act6", 10));
                Session.SendPacket(Session.Character.GenerateSay("=========================", 10));
            }

            //Messagge GameSage
            if (Session.Character.Authority == AuthorityType.GA)
            {
                Session.SendPacket(Session.Character.GenerateSay("============GS===========", 10));
                Session.SendPacket(Session.Character.GenerateSay("Welcome " + Session.Character.Name, 12));
                Session.SendPacket(Session.Character.GenerateSay("Use $Bank Help for info about Bank.", 10));
                Session.SendPacket(Session.Character.GenerateSay("Use $HelpMe to contact a NQ Team", 10));
                Session.SendPacket(Session.Character.GenerateSay("Use $Warp + Name to Move Map", 10));
                Session.SendPacket(Session.Character.GenerateSay("Use $Help to see the list of additional commands available", 10));
                Session.SendPacket(Session.Character.GenerateSay("Use $CheckStat for check status Glacer and Act6", 10));
                Session.SendPacket(Session.Character.GenerateSay("=========================", 10));
            }

            //Messagge Users
            if (Session.Character.Authority == AuthorityType.User)

            {
                Session.SendPacket(Session.Character.GenerateSay("===========NosQuest===========", 10));
                Session.SendPacket(Session.Character.GenerateSay("Welcome " + Session.Character.Name, 12));
                Session.SendPacket(Session.Character.GenerateSay("Use $Bank Help for info about Bank.", 10));
                Session.SendPacket(Session.Character.GenerateSay("Use $HelpMe to contact a NQ Team", 10));
                Session.SendPacket(Session.Character.GenerateSay("Use $Warp + Name to Move Map", 10));
                Session.SendPacket(Session.Character.GenerateSay("Use $Help to see the list of additional commands available", 10));
                Session.SendPacket(Session.Character.GenerateSay("Use $CheckStat for check status Glacer and Act6", 10));
                Session.SendPacket(Session.Character.GenerateSay("========================", 10));
            }

            //Messagge Support
            if (Session.Character.Authority == AuthorityType.Supporter)

            {
                CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage()
                {
                    DestinationCharacterId = null,
                    SourceCharacterId = Session.Character.CharacterId,
                    SourceWorldId = ServerManager.Instance.WorldId,
                    Message = $"Welcome  Support {Session.Character.Name} To NQ!",
                    Type = MessageType.Shout
                });
            }

            Observable.Interval(TimeSpan.FromSeconds(30)).Subscribe(s =>
            {
                Session?.SendPacket(Session?.Character?.GenerateSayTime());
            });

            // finfo - friends info
            Session.Character.LoadMail();
            Session.Character.LoadSentMail();
            Session.Character.DeleteTimeout();

            // Title
            Session.SendPacket(Session.Character.GenerateTitle());
            Session.SendPacket(Session.Character.GenerateTitInfo());
            Session.Character.GetTitleFromLevel();
            Session.Character.GetEffectFromTitle();

            // Simple Speed x59 for admins
            //if (Session.Account.Authority == AuthorityType.Administrator)
            //{
            //    Session.Character.Speed = 59;
            //    Session.Character.IsCustomSpeed = true;
            //    Session.SendPacket(Session.Character.GenerateCond());
            //}

            if (Session.Character.Quests.Any())
            {
                Session.SendPacket(Session.Character.GenerateQuestsPacket());
            }

            Session.Character.SendSomePacket();

            if (Session.Character.IsSeal)
            {
                if (ServerManager.Instance.ChannelId == 51)
                {
                    Session.Character.SetSeal();
                }
                else
                {
                    Session.Character.IsSeal = false;
                }
            }

            if (Session.Character.Reputation < 1)
            {
                Session.Character.Reputation = 1;
            }
        }

        #endregion
    }
}