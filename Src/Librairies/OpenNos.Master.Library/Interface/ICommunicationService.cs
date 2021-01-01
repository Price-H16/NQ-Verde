﻿using System;
using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.Master.Library.Data;
using OpenNos.SCS.Communication.ScsServices.Service;

namespace OpenNos.Master.Library.Interface
{
    [ScsService(Version = "1.1.0.0")]
    public interface ICommunicationService
    {
        /*
        * Account
        */
        bool IsAccountConnected(long accountId);
        void DisconnectAccount(long accountId);
        bool ConnectAccount(Guid worldId, long accountId, int sessionId);
        void PulseAccount(long accountId);
        void RegisterAccountLogin(long accountId, int sessionId, string ipAddress);
        bool IsLoginPermitted(long accountId, int sessionId);


        /*
        * Character
        */
        bool IsCharacterSaving(long characterId);
        void AddOrRemoveSavingCharacters(long characterId, bool add);
        void CheckForStuckAccountsAtSaving();
        bool IsCharacterConnected(string worldGroup, long characterId);
        bool ConnectCharacter(Guid worldId, long characterId);
        void DisconnectCharacter(Guid worldId, long characterId);

        /*
        * Session
        */
        bool Authenticate(string authKey);
        void KickSession(long? accountId, int? sessionId);
        IEnumerable<string> RetrieveServerStatistics(bool isStart);

        /*
         * Character Interaction
         */
        int? SendMessageToCharacter(SCSCharacterMessage message);

        /*
         * World
         */
        long[][] GetOnlineCharacters();
        int? RegisterWorldServer(SerializableWorldServer worldServer);
        int? GetChannelIdByWorldId(Guid worldId);
        void Shutdown(string worldGroup);
        void UnregisterWorldServer(Guid worldId);
        string RetrieveRegisteredWorldServers(string username, byte regionType, int sessionId, bool ignoreUserName);
        long[][] RetrieveOnlineCharacters(long characterId);
        void Restart(string worldGroup, int time = 5);
        string RetrieveOriginWorld(long accountId);
        void RefreshPenalty(int penaltyId);
        void RunGlobalEvent(EventType eventType, byte value);
        void SetWorldServerAsInvisible(Guid worldId);

        /*
         * Session
         */
        void CleanupOutdatedSession();
        bool ConnectAccountCrossServer(Guid worldId, long accountId, int sessionId);
        bool IsCrossServerLoginPermitted(long accountId, int sessionId);
        void RegisterCrossServerAccountLogin(long accountId, int sessionId);

        /*
         * Act4
         */
        bool IsAct4Online(string worldGroup);

        /*
         * Update
         */
        void UpdateRelation(string worldGroup, long relationId);
        void UpdateFamily(string worldGroup, long familyId, bool changeFaction);
        void UpdateBazaar(string worldGroup, long bazaarItemId);
        void Cleanup();

    }
}