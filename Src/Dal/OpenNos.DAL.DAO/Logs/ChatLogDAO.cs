//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using OpenNos.Core;
//using OpenNos.DAL.EF;
//using OpenNos.DAL.EF.Helpers;
//using OpenNos.DAL.Interface;
//using OpenNos.Data;
//using OpenNos.Data.Enums;
//using OpenNos.Mapper.Mappers;

//namespace OpenNos.DAL.DAO
//{
//    public class ChatLogDAO : IChatLogDAO
//    {
//        #region Methods

//        public SaveResult InsertOrUpdate(ChatLogDTO log)
//        {
//            try
//            {
//                using (var context = DataAccessHelper.CreateContext())
//                {
//                    var entity = context.ChatLogs.FirstOrDefault(x => x.Id.Equals(log.Id));

//                    if (entity == null)
//                    {
//                        log = insert(log, context);
//                        return SaveResult.Inserted;
//                    }

//                    log = update(entity, log, context);
//                    return SaveResult.Updated;
//                }
//            }
//            catch (Exception e)
//            {
//                Logger.Error(
//                    string.Format(Language.Instance.GetMessageFromKey("UPDATE_CHATLOG_ERROR"), log.Id, e.Message), e);
//                return SaveResult.Error;
//            }
//        }

//        public void InsertOrUpdate(IEnumerable<ChatLogDTO> logs)
//        {
//            try
//            {
//                using (var context = DataAccessHelper.CreateContext())
//                {
//                    context.Configuration.AutoDetectChangesEnabled = false;
//                    foreach (var card in logs) InsertOrUpdate(card);
//                    context.Configuration.AutoDetectChangesEnabled = true;
//                    context.SaveChanges();
//                }
//            }
//            catch (Exception e)
//            {
//                Logger.Error(e);
//            }
//        }

//        private static ChatLogDTO insert(ChatLogDTO account, OpenNosContext context)
//        {
//            var entity = new ChatLog();
//            ChatLogMapping.ToChatLog(account, entity);
//            context.ChatLogs.Add(entity);
//            context.SaveChanges();
//            ChatLogMapping.ToChatLogDTO(entity, account);
//            return account;
//        }

//        private static ChatLogDTO update(ChatLog entity, ChatLogDTO account, OpenNosContext context)
//        {
//            if (entity != null)
//            {
//                ChatLogMapping.ToChatLog(account, entity);
//                context.Entry(entity).State = EntityState.Modified;
//                context.SaveChanges();
//            }

//            if (ChatLogMapping.ToChatLogDTO(entity, account)) return account;

//            return null;
//        }

//        #endregion
//    }
//}