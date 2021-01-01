using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using OpenNos.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.DAL.DAO
{
    public class FamilySkillMissionDAO : IFamilySkillMissionDAO
    {
        #region Methods

        public void DailyReset(FamilySkillMissionDTO fsm)
        {
            try
            {
                fsm.CurrentValue = (short)(fsm.ItemVNum < 9604 ? 1 : 0);
                InsertOrUpdate(ref fsm);
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("INSERT_ERROR"), "famskillmission", e.Message), e);
            }
        }

        public DeleteResult Delete(long itemVNum, long familyId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    FamilySkillMission famskillmission = context.FamilySkillMission.FirstOrDefault(c => c.ItemVNum.Equals(itemVNum) && c.FamilyId.Equals(familyId));

                    if (famskillmission != null)
                    {
                        context.FamilySkillMission.Remove(famskillmission);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_ERROR"), "famskillmission", e.Message), e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref FamilySkillMissionDTO familySkillMission)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    short ItemVNum = familySkillMission.ItemVNum;
                    long FamilyId = familySkillMission.FamilyId;
                    FamilySkillMission entity = context.FamilySkillMission.FirstOrDefault(c => c.ItemVNum.Equals(ItemVNum) && c.FamilyId.Equals(FamilyId));

                    if (entity == null)
                    {
                        familySkillMission = insert(familySkillMission, context);
                        return SaveResult.Inserted;
                    }

                    familySkillMission = update(entity, familySkillMission, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_FamilySkillMission_ERROR"), familySkillMission.FamilySkillMissionId, e.Message), e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<FamilySkillMissionDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<FamilySkillMissionDTO> result = new List<FamilySkillMissionDTO>();
                foreach (FamilySkillMission FamilySkillMission in context.FamilySkillMission)
                {
                    FamilySkillMissionDTO dto = new FamilySkillMissionDTO();
                    Mapper.Mappers.FamilySkillMissionMapper.ToFamilySkillMissionDTO(FamilySkillMission, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        public IList<FamilySkillMissionDTO> LoadByFamilyId(long familyId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<FamilySkillMissionDTO> result = new List<FamilySkillMissionDTO>();
                foreach (FamilySkillMission entity in context.FamilySkillMission.Where(fs => fs.FamilyId.Equals(familyId)))
                {
                    FamilySkillMissionDTO dto = new FamilySkillMissionDTO();
                    Mapper.Mappers.FamilySkillMissionMapper.ToFamilySkillMissionDTO(entity, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        private static FamilySkillMissionDTO insert(FamilySkillMissionDTO famskillmission, OpenNosContext context)
        {
            FamilySkillMission entity = new FamilySkillMission();
            Mapper.Mappers.FamilySkillMissionMapper.ToFamilySkillMission(famskillmission, entity);
            context.FamilySkillMission.Add(entity);
            context.SaveChanges();
            if (Mapper.Mappers.FamilySkillMissionMapper.ToFamilySkillMissionDTO(entity, famskillmission))
            {
                return famskillmission;
            }

            return null;
        }

        private static FamilySkillMissionDTO update(FamilySkillMission entity, FamilySkillMissionDTO famskillmission, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mappers.FamilySkillMissionMapper.ToFamilySkillMission(famskillmission, entity);
                context.SaveChanges();
            }

            if (Mapper.Mappers.FamilySkillMissionMapper.ToFamilySkillMissionDTO(entity, famskillmission))
            {
                return famskillmission;
            }

            return null;
        }

        #endregion
    }
}