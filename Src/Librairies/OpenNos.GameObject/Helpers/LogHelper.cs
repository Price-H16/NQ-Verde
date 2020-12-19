using System;
using System.Collections.Generic;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject.Helpers
{
    public class LogHelper
    {

        #region Members

        private static LogHelper _instance;

        #endregion

        #region Properties

        public static LogHelper Instance => _instance ?? (_instance = new LogHelper());

        #endregion

        #region Methods
        //public void InsertRaidLog(RaidLogDTO log)
        //{
        //    DAOFactory.RaidLogDAO.InsertOrUpdate(log);
        //}

        //public void InsertRaidLog(RaidLogDTO log)
        //{
        //    DAOFactory.RaidLogDAO.InsertOrUpdate(log);
        //}

        //public void InsertUpgradeLog(UpgradeLogDTO log)
        //{
        //    DAOFactory.UpgradeLogDAO.InsertOrUpdate(log);
        //}

        //public void InsertExchangeLog(ExchangeLogDTO log)
        //{
        //    DAOFactory.ExchangeLogDAO.InsertOrUpdate(log);
        //}

        //public void InsertAntiBotLog(AntiBotLogDTO log)
        //{
        //    DAOFactory.AntiBotLogDAO.InsertOrUpdate(log);
        //}

        //public void InsertChatLog(ChatLogDTO log)
        //{
        //    DAOFactory.ChatLogDAO.InsertOrUpdate(log);
        //}

        //public void InsertChatLogs(IEnumerable<ChatLogDTO> logs)
        //{
        //    DAOFactory.ChatLogDAO.InsertOrUpdate(logs);
        //}

        public void InsertCommandsLogs(IEnumerable<LogCommandsDTO> logs)
        {
            DAOFactory.LogsCommandsDAO.InsertOrUpdate(logs);
        }

        public void ClearAllList()
        {
            //ServerManager.Instance.ChatLogs.Clear();
            ServerManager.Instance.CommandsLogs.Clear();
        }

        public void InsertAllLogs()
        {
            //InsertChatLogs(ServerManager.Instance.ChatLogs.GetAllItems());
            InsertCommandsLogs(ServerManager.Instance.CommandsLogs.GetAllItems());
        }

        public void InsertQuestLog(long characterId, string ipAddress, long questId, DateTime lastDaily)
        {
            var log = new QuestLogDTO
            {
                CharacterId = characterId,
                IpAddress = ipAddress,
                QuestId = questId,
                LastDaily = lastDaily
            };
            DAOFactory.QuestLogDAO.InsertOrUpdate(ref log);
        }

        #endregion
    }
}