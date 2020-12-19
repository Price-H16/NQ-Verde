using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using OpenNos.Data.Enums;
using OpenNos.Mapper.Mappers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace OpenNos.DAL.DAO
{
    public class LogsCommandsDAO : ILogsCommandsDAO
    {
        private static LogCommandsDTO Insert(LogCommandsDTO account, OpenNosContext context)
        {
            var entity = new LogCommands();
            LogsCommmandsMapper.ToLogsCommmand(account, entity);
            context.LogCommands.Add(entity);
            context.SaveChanges();
            LogsCommmandsMapper.LogsCommmandDTO(entity, account);
            return account;
        }

        private static LogCommandsDTO Update(LogCommands entity, LogCommandsDTO account, OpenNosContext context)
        {
            if (entity != null)
            {
                LogsCommmandsMapper.ToLogsCommmand(account, entity);
                context.Entry(entity).State = EntityState.Modified;
                context.SaveChanges();
            }

            if (LogsCommmandsMapper.LogsCommmandDTO(entity, account)) return account;

            return null;
        }

        public SaveResult InsertOrUpdate(LogCommandsDTO log)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var entity = context.LogCommands.FirstOrDefault(x => x.CommandId.Equals(log.CommandId));

                    if (entity == null)
                    {
                        log = Insert(log, context);
                        return SaveResult.Inserted;
                    }

                    log = Update(entity, log, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(
                    string.Format(Language.Instance.GetMessageFromKey("INSERT_LOGSCMD_ERROR"), log.CommandId, e.Message), e);
                return SaveResult.Error;
            }
        }

        public void InsertOrUpdate(IEnumerable<LogCommandsDTO> logs)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    foreach (var card in logs) InsertOrUpdate(card);
                    context.Configuration.AutoDetectChangesEnabled = true;
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
