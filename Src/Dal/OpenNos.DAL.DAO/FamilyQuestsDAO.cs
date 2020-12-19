using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using OpenNos.Data.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace OpenNos.DAL.DAO
{
    public class FamilyQuestsDAO : IFamilyQuestsDAO
    {
        public IEnumerable<FamilyQuestsDTO> LoadByFamilyAndQuestId(FamilyQuestsDTO log)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<FamilyQuestsDTO> result = new List<FamilyQuestsDTO>();
                foreach (FamilyQuests Log in context.FamilyQuests.Where(s => s.QuestId.Equals(log.QuestId) && s.FamilyId.Equals(log.FamilyId)))
                {
                    FamilyQuestsDTO dto = new FamilyQuestsDTO();
                    Mapper.Mappers.FamilyQuestsMapper.ToFamilyQuestsDTO(Log, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public IEnumerable<FamilyQuestsDTO> LoadAllByFamilyId(long Id)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<FamilyQuestsDTO> result = new List<FamilyQuestsDTO>();
                foreach (FamilyQuests Log in context.FamilyQuests.Where(s => s.FamilyId.Equals(Id)))
                {
                    FamilyQuestsDTO dto = new FamilyQuestsDTO();
                    Mapper.Mappers.FamilyQuestsMapper.ToFamilyQuestsDTO(Log, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public SaveResult InsertOrUpdate(FamilyQuestsDTO log)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    FamilyQuests entity = context.FamilyQuests.FirstOrDefault(c => c.FamilyQuestsId.Equals(log.FamilyQuestsId));

                    if (entity == null)
                    {
                        log = insert(log, context);
                        return SaveResult.Inserted;
                    }
                    log = update(entity, log, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_FamilyQuests_ERROR"), log.FamilyQuestsId, e.Message), e);
                return SaveResult.Error;
            }
        }

        private static FamilyQuestsDTO insert(FamilyQuestsDTO FamilyQuests, OpenNosContext context)
        {
            FamilyQuests entity = new FamilyQuests();
            Mapper.Mappers.FamilyQuestsMapper.ToFamilyQuests(FamilyQuests, entity);
            context.FamilyQuests.Add(entity);
            context.SaveChanges();
            Mapper.Mappers.FamilyQuestsMapper.ToFamilyQuestsDTO(entity, FamilyQuests);
            return FamilyQuests;
        }

        private static FamilyQuestsDTO update(FamilyQuests entity, FamilyQuestsDTO FamilyQuests, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mappers.FamilyQuestsMapper.ToFamilyQuests(FamilyQuests, entity);
                context.Entry(entity).State = EntityState.Modified;
                context.SaveChanges();
            }
            if (Mapper.Mappers.FamilyQuestsMapper.ToFamilyQuestsDTO(entity, FamilyQuests))
            {
                return FamilyQuests;
            }

            return null;
        }
    }
}
