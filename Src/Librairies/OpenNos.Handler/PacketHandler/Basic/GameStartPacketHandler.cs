using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using ChickenAPI.Enums;
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
                                ServerManager.Instance.Configuration.Act4Port, 2);
                        return;
                    }

                    shouldRespawn = true;
                }
            }

            Session.CurrentMapInstance = Session.Character.MapInstance;

            if (ServerManager.Instance.Configuration.SceneOnCreate && Session.Character.GeneralLogs.CountLinq(s => s.LogType == "Connection") < 2)
            {
                Session.SendPacket("scene 40");
            }

            if (ServerManager.Instance.Configuration.WorldInformation)
            {
                var assembly = Assembly.GetEntryAssembly();
                var productVersion = assembly?.Location != null
                    ? FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion
                    : "1337";

                Session.SendPacket(Session.Character.GenerateSay("------------------[NosQuest]------------------", 10));
                Session.SendPacket(Session.Character.GenerateSay("Website: https://nosquestreborn.com", 12)); 
                Session.SendPacket(Session.Character.GenerateSay("Discord: https://discord.gg/zM5JxBK", 12));
                Session.SendPacket(Session.Character.GenerateSay("Founders:  Price, Devilion", 11));
                Session.SendPacket(Session.Character.GenerateSay("TeamManager: Linky", 11));
                Session.SendPacket(Session.Character.GenerateSay("ComumnityManager: LemonTree, Childe", 11));
                Session.SendPacket(Session.Character.GenerateSay("SGA: Calliope", 11));
                Session.SendPacket(Session.Character.GenerateSay("--------------------------------------------------", 10));
                Session.SendPacket(Session.Character.GenerateSay("Hey friends, use this Command: $usr (User-Command-List)", 11));
                Session.SendPacket(Session.Character.GenerateSay("------------------[Counter]--------------------", 10));
                Session.SendPacket(Session.Character.GenerateSay($"Mob Kill Counter: {Session.Character.MobKillCounter.ToString("###,##0")}", 10));
                Session.SendPacket(Session.Character.GenerateSay("--------------------------------------------------", 10));
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

            if (Session.Character.Level == 90)
            {
                Session.SendPacket(Session.Character.GenerateSay("You can now access to Family Dungeon!", 10));
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

            var medal = Session.Character.StaticBonusList.Find(s => s.StaticBonusType == StaticBonusType.BazaarMedalGold || s.StaticBonusType == StaticBonusType.BazaarMedalSilver);
            var buffs = Session.Character.StaticBonusList.Find(s => s.StaticBonusType == StaticBonusType.MedalOfErenia);

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
            Session.SendPacket( $"twk 1 {Session.Character.CharacterId} {Session.Account.Name} {Session.Character.Name} shtmxpdlfeoqkr");


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

                Session.SendPacket(Session.Character.GenerateGInfo());
                Session.SendPackets(Session.Character.GetFamilyHistory());
                Session.SendPacket(Session.Character.GenerateFamilyMember());
                Session.SendPacket(Session.Character.GenerateFamilyMemberMessage());
                Session.SendPacket(Session.Character.GenerateFamilyMemberExp());

                if (!string.IsNullOrWhiteSpace(Session.Character.Family.FamilyMessage))
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateInfo("--- Family Message ---\n" + Session.Character.Family.FamilyMessage));

                }
            }
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

            IEnumerable<PenaltyLogDTO> warning = DAOFactory.PenaltyLogDAO.LoadByAccount(Session.Character.AccountId).Where(p => p.Penalty == PenaltyType.Warning);
            if (warning.Any())
            {
                Session.SendPacket(UserInterfaceHelper.GenerateInfo(string.Format(Language.Instance.GetMessageFromKey("WARNING_INFO"), warning.Count())));
            }


            Observable.Interval(TimeSpan.FromSeconds(20)).Subscribe(s =>
            {
                Session.SendPacket(Session.Character.GenerateSayTime());
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

            if (Session.Character.Quests.Any())
            {
                Session.SendPacket(Session.Character.GenerateQuestsPacket());
            }


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
        }

        #endregion
    }
}