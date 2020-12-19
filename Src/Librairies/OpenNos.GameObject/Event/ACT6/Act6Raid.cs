using System;
using System.Linq;
using System.Reactive.Linq;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;

namespace OpenNos.GameObject.Event.ACT6
{
    public static class Act6Raid
    {
        #region Members

        public static FactionType _faction;

        public static MapInstance EntryMap;

        public static ScriptedInstance RaidInstance;

        #endregion

        #region Methods

        public static void GenerateRaid(FactionType raidType)
        {
            CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
            {
                DestinationCharacterId = null,
                SourceCharacterId = 0,
                SourceWorldId = ServerManager.Instance.WorldId,
                Message = $"Act.6 Raid Opened: ( {raidType} )",
                Type = MessageType.Shout
            });

            _faction = raidType;
            RaidInstance =
                ServerManager.Instance.Act6Raids.FirstOrDefault(s => s.Id == (raidType == FactionType.Angel ? 23 : 24));


            if (RaidInstance == null)
            {
                Logger.Log.InfoFormat(Language.Instance.GetMessageFromKey("CANT_CREATE_RAIDS"));
                return;
            }

            EntryMap = ServerManager.GetMapInstance(
                ServerManager.GetBaseMapInstanceIdByMapId(RaidInstance.MapId));

            if (EntryMap == null)
            {
                Logger.Log.InfoFormat(Language.Instance.GetMessageFromKey("MAP_MISSING"));
                return;
            }

            EntryMap.CreatePortal(new Portal
            {
                Type = (byte) PortalType.Raid,
                SourceMapId = RaidInstance.MapId,
                SourceX = RaidInstance.PositionX,
                SourceY = RaidInstance.PositionY
            }, 3600, true);

            Observable.Timer(TimeSpan.FromHours(1)).Subscribe(o => { EndRaid(); });
        }

        private static void EndRaid()
        {
            switch (_faction)
            {
                case FactionType.Angel:
                    ServerManager.Instance.Act6Zenas.Mode = 0;
                    break;

                case FactionType.Demon:
                    ServerManager.Instance.Act6Erenia.Mode = 0;
                    break;
            }

            ServerManager.Instance.Act6Process();
            ServerManager.Instance.StartedEvents.Remove(EventType.ACT6RAID);
        }

        #endregion
    }
}