using Hik.Communication.ScsServices.Service;
using NosTale.Configuration;
using NosTale.Configuration.Utilities;
using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.Log.Networking;
using OpenNos.Log.Shared;
using OpenNos.Master.Library.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.Log.Server
{
    internal class LogService : ScsService, ILogService
    {
        #region Methods

        public bool Authenticate(string authKey)
        {
            if (string.IsNullOrWhiteSpace(authKey))
            {
                return false;
            }

            var a = ContainerIoc.GetInstance<JsonGameConfiguration>().Server;

            if (authKey == a.LogKey)
            {
                LogManager.Instance.AuthentificatedClients.Add(CurrentClient.ClientId);
                return true;
            }

            return false;
        }

        public bool AuthenticateAdmin(string user, string passHash)
        {
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(passHash))
            {
                return false;
            }

            if (AuthentificationServiceClient.Instance.ValidateAccount(user, passHash) is AccountDTO account && account.Authority > Domain.AuthorityType.User)
            {
                LogManager.Instance.AuthentificatedClients.Add(CurrentClient.ClientId);
                return true;
            }

            return false;
        }

        public AccountDTO GetAccount(string user, string passHash)
        {
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(passHash))
            {
                return null;
            }

            if (AuthentificationServiceClient.Instance.ValidateAccount(user, passHash) is AccountDTO account && account.Authority > Domain.AuthorityType.User)
            {
                return account;
            }

            return null;
        }

        public List<LogEntry> GetChatLogEntries(string[] sender, string[] senderid, string[] receiver, string[] receiverid, string[] message, DateTime? start, DateTime? end, LogType? logType)
        {
            Logger.Info($"Received Log Request - Sender: {sender} SenderId: {senderid} Receiver: {receiver} ReceiverId: {receiverid} Message: {message} DateStart: {start} DateEnd: {end} ChatLogType: {logType}");
            List<LogEntry> tmp = LogManager.Instance.AllChatLogs.GetAllItems();
            if (sender != null)
            {
                tmp = tmp.Where(s => sender.Any(x => s.Sender.IndexOf(x, StringComparison.CurrentCultureIgnoreCase) >= 0)).ToList();
            }
            if (senderid != null)
            {
                tmp = tmp.Where(s => senderid.Any(x => s.SenderId == long.Parse(x))).ToList();
            }
            if (receiver != null)
            {
                tmp = tmp.Where(s => receiver.Any(x => s.Receiver?.IndexOf(x, StringComparison.CurrentCultureIgnoreCase) >= 0)).ToList();
            }
            if (receiverid != null)
            {
                tmp = tmp.Where(s => receiverid.Any(x => s.ReceiverId == long.Parse(x))).ToList();
            }
            if (message != null)
            {
                tmp = tmp.Where(s => message.Any(x => s.Message.IndexOf(x, StringComparison.CurrentCultureIgnoreCase) >= 0)).ToList();
            }
            if (start.HasValue)
            {
                tmp = tmp.Where(s => s.Timestamp >= start).ToList();
            }
            if (end.HasValue)
            {
                tmp = tmp.Where(s => s.Timestamp <= end).ToList();
            }
            if (logType.HasValue)
            {
                tmp = tmp.Where(s => s.MessageType == logType).ToList();
            }
            return tmp;
        }

        public List<PacketLogEntry> GetPacketLogEntries(string[] sender, string[] senderid, string[] receiver, string[] receiverid, string[] packet, DateTime? start, DateTime? end, LogType? logType, AuthorityType authority)
        {
            Logger.Info($"Received Log Request - Sender: {sender} SenderId: {senderid} Receiver: {receiver} ReceiverId: {receiverid} Packet: {packet} DateStart: {start} DateEnd: {end} ChatLogType: {logType}");
            List<PacketLogEntry> tmp = LogManager.Instance.AllPacketLogs.GetAllItems();
            if (sender != null)
            {
                tmp = tmp.Where(s => sender.Any(x => s.Sender.IndexOf(x, StringComparison.CurrentCultureIgnoreCase) >= 0)).ToList();
            }
            if (senderid != null)
            {
                tmp = tmp.Where(s => senderid.Any(x => s.SenderId == long.Parse(x))).ToList();
            }
            if (receiver != null)
            {
                tmp = tmp.Where(s => receiver.Any(x => s.Receiver.IndexOf(x, StringComparison.CurrentCultureIgnoreCase) >= 0)).ToList();
            }
            if (receiverid != null)
            {
                tmp = tmp.Where(s => receiverid.Any(x => s.ReceiverId == long.Parse(x))).ToList();
            }
            if (packet != null)
            {
                tmp = tmp.Where(s => packet.Any(x => s.Packet.IndexOf(x, StringComparison.CurrentCultureIgnoreCase) >= 0)).ToList();
            }
            if (start.HasValue)
            {
                tmp = tmp.Where(s => s.Timestamp >= start).ToList();
            }
            if (end.HasValue)
            {
                tmp = tmp.Where(s => s.Timestamp <= end).ToList();
            }
            if (logType.HasValue)
            {
                tmp = tmp.Where(s => s.PacketType == logType).ToList();
            }
            if (authority <= AuthorityType.GM)
            {
                tmp = tmp.Where(s => s.PacketType <= LogType.ItemCreate).ToList();
            }
            if (authority <= AuthorityType.SGM)
            {
                tmp = tmp.Where(s => s.PacketType <= LogType.GMCommand).ToList();
            }
            if (authority <= AuthorityType.GA)
            {
                tmp = tmp.Where(s => s.PacketType <= LogType.Packet).ToList();
            }
            return tmp;
        }

        public void LogChatMessage(LogEntry logEntry)
        {
            if (!LogManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)) || logEntry == null)
            {
                return;
            }

            logEntry.Timestamp = DateTime.Now;
            LogManager.Instance.ChatLogs.Add(logEntry);
            LogManager.Instance.AllChatLogs.Add(logEntry);
        }

        public void LogPacket(PacketLogEntry logEntry)
        {
            if (!LogManager.Instance.AuthentificatedClients.Any(s => s.Equals(CurrentClient.ClientId)) || logEntry == null)
            {
                return;
            }

            logEntry.Timestamp = DateTime.Now;
            LogManager.Instance.PacketLogs.Add(logEntry);
            LogManager.Instance.AllPacketLogs.Add(logEntry);
        }

        #endregion
    }
}