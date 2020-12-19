using System;
using System.Collections.Generic;
using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using OpenNos.Data.Enums;
using OpenNos.Mapper.Mappers;

namespace OpenNos.DAL.DAO
{
    public class QuestDAO : IQuestDAO
    {
        #region Methods

        public DeleteResult DeleteById(long id)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var deleteEntity = context.Quest.Find(id);
                    if (deleteEntity != null)
                    {
                        context.Quest.Remove(deleteEntity);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_ERROR"), id, e.Message), e);
                return DeleteResult.Error;
            }
        }

        public void Insert(List<QuestDTO> questList)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    foreach (var quest in questList)
                    {
                        var entity = new Quest();
                        QuestMapper.ToQuest(quest, entity);
                        context.Quest.Add(entity);
                    }

                    context.Configuration.AutoDetectChangesEnabled = true;
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public QuestDTO InsertOrUpdate(QuestDTO quest)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var entity = context.Quest.Find(quest.QuestId);

                    if (entity == null) return insert(quest, context);
                    return update(entity, quest, context);
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("INSERT_ERROR"), quest, e.Message), e);
                return quest;
            }
        }

        public IEnumerable<QuestDTO> LoadAll()
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<QuestDTO>();
                foreach (var entity in context.Quest)
                {
                    var dto = new QuestDTO();
                    QuestMapper.ToQuestDTO(entity, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        public QuestDTO LoadById(long id)
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var dto = new QuestDTO();
                if (QuestMapper.ToQuestDTO(context.Quest.Find(id), dto)) return dto;

                return null;
            }
        }

        private static QuestDTO insert(QuestDTO quest, OpenNosContext context)
        {
            var entity = new Quest();
            QuestMapper.ToQuest(quest, entity);
            context.Quest.Add(entity);
            context.SaveChanges();
            if (QuestMapper.ToQuestDTO(entity, quest)) return quest;

            return null;
        }

        private static QuestDTO update(Quest entity, QuestDTO quest, OpenNosContext context)
        {
            if (entity != null)
            {
                QuestMapper.ToQuest(quest, entity);
                context.SaveChanges();
            }

            if (QuestMapper.ToQuestDTO(entity, quest)) return quest;

            return null;
        }

        #endregion
    }
}