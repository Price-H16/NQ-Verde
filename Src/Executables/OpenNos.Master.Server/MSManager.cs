using NosTale.Configuration;
using NosTale.Configuration.Utilities;
using OpenNos.Core;
using OpenNos.Master.Library.Data;
using OpenNos.SCS.Communication.ScsServices.Service;
using System;
using System.Collections.Generic;

namespace OpenNos.Master.Server
{
    internal class MSManager
    {
        #region Members

        private static MSManager _instance;

        #endregion

        #region Instantiation

        public MSManager()
        {
            WorldServers = new List<WorldServer>();
            LoginServers = new List<IScsServiceClient>();
            CharactersUnderSaveProcess = new Dictionary<long, DateTime>();
            ConnectedAccounts = new ThreadSafeGenericList<AccountConnection>();
            AuthentificatedClients = new ThreadSafeGenericLockedList<long>();
            var a = DependencyContainer.Instance.GetInstance<JsonGameConfiguration>();
            ConfigurationObject = new ConfigurationObject
            {
                #region Rate

                RateXP = a.Rate.RateXP,
                RateHeroicXP = a.Rate.RateHeroicXP,
                RateDrop = a.Rate.RateDrop,
                RateGoldDrop = a.Rate.RateGoldDrop,
                RateGold = a.Rate.RateGold,
                RateReputation = a.Rate.RateReputation,
                RateFairyXP = a.Rate.RateFairyXP,
                PartnerSpXp = a.Rate.PartnerSpXp,
                QuestDropRate = a.Rate.QuestDropRate,
                GlacernonPercentRatePvm = a.Rate.GlacernonPercentRatePvm,
                GlacernonPercentRatePvp = a.Rate.GlacernonPercentRatePvp,
                CylloanPercentRate = a.Rate.CylloanPercentRate,

                #endregion

                #region Max

                MaxGold = a.Max.MaxGold,
                MaxLevel = a.Max.MaxLevel,
                MaxJobLevel = a.Max.MaxJobLevel,
                MaxSPLevel = a.Max.MaxSPLevel,
                MaxHeroLevel = a.Max.MaxHeroLevel,
                HeroicStartLevel = a.Max.HeroicStartLevel,
                MaxUpgrade = a.Max.MaxUpgrade,
                MaxGoldBank = a.Max.MaxGoldBank,

                #endregion

                #region Server

                SceneOnCreate = a.Server.SceneOnCreate,
                SessionLimit = a.Server.SessionLimit,
                WorldInformation = a.Server.WorldInformation,
                Act4IP = a.Server.IPAddress,
                Act4Port = a.Server.Act4Port,
                LockSystem = a.Server.LockSystem,
                BCardsInArenaTalent = a.Server.BCardsInArenaTalent,

                #endregion

                #region Event

                HalloweenEvent = a.Event.HalloweenEvent,
                ChristmasEvent = a.Event.ChristmasEvent,
                EstivalEvent = a.Event.EstivalEvent,
                ValentineEvent = a.Event.ValentineEvent,
                LunarNewYearEvent = a.Event.LunarNewYearEvent,
                EasterEvent = a.Event.EasterEvent,
                EventLvlUpEq = a.Event.EventLvlUpEq,
                EventRareUpEq = a.Event.EventRareUpEq,
                EventSpUp = a.Event.EventSpUp,
                EventSpPerfection = a.Event.EventSpPerfection,
                EventXPF = a.Event.EventXPF,
                EventSealed = a.Event.EventSealed,
                EventXp = a.Event.EventXp,
                EventGold = a.Event.EventGold,
                EventRep = a.Event.EventRep,
                EventDrop = a.Event.EventDrop,
                EventRuneUp = a.Event.EventRuneUp,
                EventTattoUp = a.Event.EventTattoUp

                #endregion
            };
        }

        #endregion

        #region Properties

        public static MSManager Instance => _instance ?? (_instance = new MSManager());

        public ThreadSafeGenericLockedList<long> AuthentificatedClients { get; set; }

        public Dictionary<long, DateTime> CharactersUnderSaveProcess { get; set; }

        public ConfigurationObject ConfigurationObject { get; set; }

        public ThreadSafeGenericList<AccountConnection> ConnectedAccounts { get; set; }

        public List<IScsServiceClient> LoginServers { get; set; }

        public List<WorldServer> WorldServers { get; set; }

        #endregion
    }
}