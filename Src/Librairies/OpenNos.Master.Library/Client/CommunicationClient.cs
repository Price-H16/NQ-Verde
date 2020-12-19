﻿using System.Threading.Tasks;
using OpenNos.Domain;
using OpenNos.Master.Library.Data;
using OpenNos.Master.Library.Interface;

namespace OpenNos.Master.Library.Client
{
    internal class CommunicationClient : ICommunicationClient
    {
        #region Methods

        public void CharacterConnected(long characterId)
        {
            Task.Run(() => CommunicationServiceClient.Instance.OnCharacterConnected(characterId));
        }

        public void CharacterDisconnected(long characterId)
        {
            Task.Run(() => CommunicationServiceClient.Instance.OnCharacterDisconnected(characterId));
        }

        public void KickSession(long? accountId, int? sessionId)
        {
            Task.Run(() => CommunicationServiceClient.Instance.OnKickSession(accountId, sessionId));
        }

        public void Restart(int time = 5)
        {
            Task.Run(() => CommunicationServiceClient.Instance.OnRestart(time));
        }

        public void RunGlobalEvent(EventType eventType, byte value)
        {
            Task.Run(() => CommunicationServiceClient.Instance.OnRunGlobalEvent(eventType, value));
        }

        public void SendMessageToCharacter(SCSCharacterMessage message)
        {
            Task.Run(() => CommunicationServiceClient.Instance.OnSendMessageToCharacter(message));
        }

        public void Shutdown()
        {
            Task.Run(() => CommunicationServiceClient.Instance.OnShutdown());
        }

        public void UpdateBazaar(long bazaarItemId)
        {
            Task.Run(() => CommunicationServiceClient.Instance.OnUpdateBazaar(bazaarItemId));
        }

        public void UpdateFamily(long familyId, bool changeFaction)
        {
            Task.Run(() => CommunicationServiceClient.Instance.OnUpdateFamily(familyId, changeFaction));
        }

        public void UpdatePenaltyLog(int penaltyLogId)
        {
            Task.Run(() => CommunicationServiceClient.Instance.OnUpdatePenaltyLog(penaltyLogId));
        }

        public void UpdateRelation(long relationId)
        {
            Task.Run(() => CommunicationServiceClient.Instance.OnUpdateRelation(relationId));
        }

        public void UpdateStaticBonus(long characterId)
        {
            Task.Run(() => CommunicationServiceClient.Instance.OnUpdateStaticBonus(characterId));
        }

        #endregion
    }
}