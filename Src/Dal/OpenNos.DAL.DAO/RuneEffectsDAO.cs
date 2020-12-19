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
    public class RuneEffectsDAO : IRuneEffectsDAO
    {
        #region Methods

        public DeleteResult DeleteByEquipmentSerialId(Guid id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {

                    List<RuneEffects> deleteentities = context.RuneEffects.Where(s => s.EquipmentSerialId == id).ToList();
                    if (deleteentities.Count != 0)
                    {
                        context.RuneEffects.RemoveRange(deleteentities);
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

        public RuneEffectsDTO InsertOrUpdate(RuneEffectsDTO RuneEffects)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long RuneEffectsId = RuneEffects.RuneEffectId;
                    RuneEffects entity = context.RuneEffects.FirstOrDefault(c => c.RuneEffectId.Equals(RuneEffectsId));

                    if (entity == null)
                    {
                        return insert(RuneEffects, context);
                    }
                    return update(entity, RuneEffects, context);
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("INSERT_ERROR"), RuneEffects, e.Message), e);
                return RuneEffects;
            }
        }

        public void InsertOrUpdateFromList(List<RuneEffectsDTO> RuneEffectss, Guid equipmentSerialId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    void insert(RuneEffectsDTO RuneEffects)
                    {
                        RuneEffects _entity = new RuneEffects();
                        Mapper.Mappers.RuneEffectsMapper.ToRuneEffect(RuneEffects, _entity);
                        context.RuneEffects.Add(_entity);
                        context.SaveChanges();
                        RuneEffects.RuneEffectId = _entity.RuneEffectId;
                    }

                    void update(RuneEffects _entity, RuneEffectsDTO RuneEffects)
                    {
                        if (_entity != null)
                        {
                            Mapper.Mappers.RuneEffectsMapper.ToRuneEffect(RuneEffects, _entity);
                        }
                    }

                    foreach (RuneEffectsDTO item in RuneEffectss)
                    {
                        item.EquipmentSerialId = equipmentSerialId;
                        RuneEffects entity = context.RuneEffects.FirstOrDefault(c => c.RuneEffectId == item.RuneEffectId);

                        if (entity == null)
                        {
                            insert(item);
                        }
                        else
                        {
                            update(entity, item);
                        }
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public IEnumerable<RuneEffectsDTO> LoadByEquipmentSerialId(Guid id)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                List<RuneEffectsDTO> result = new List<RuneEffectsDTO>();
                foreach (RuneEffects entity in context.RuneEffects.Where(c => c.EquipmentSerialId == id))
                {
                    RuneEffectsDTO dto = new RuneEffectsDTO();
                    Mapper.Mappers.RuneEffectsMapper.ToRuneEffectDTO(entity, dto);
                    result.Add(dto);
                }
                return result;
            }
        }

        private static RuneEffectsDTO insert(RuneEffectsDTO RuneEffects, OpenNosContext context)
        {
            RuneEffects entity = new RuneEffects();
            Mapper.Mappers.RuneEffectsMapper.ToRuneEffect(RuneEffects, entity);
            context.RuneEffects.Add(entity);
            context.SaveChanges();
            if (Mapper.Mappers.RuneEffectsMapper.ToRuneEffectDTO(entity, RuneEffects))
            {
                return RuneEffects;
            }

            return null;
        }

        private static RuneEffectsDTO update(RuneEffects entity, RuneEffectsDTO RuneEffects, OpenNosContext context)
        {
            if (entity != null)
            {
                Mapper.Mappers.RuneEffectsMapper.ToRuneEffect(RuneEffects, entity);
                context.SaveChanges();
            }

            if (Mapper.Mappers.RuneEffectsMapper.ToRuneEffectDTO(entity, RuneEffects))
            {
                return RuneEffects;
            }

            return null;
        }

        #endregion
    }
}