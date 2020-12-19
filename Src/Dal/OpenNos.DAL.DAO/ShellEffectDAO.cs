using System;
using System.Collections.Generic;
using System.Linq;
using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using OpenNos.Data.Enums;
using OpenNos.Mapper.Mappers;

namespace OpenNos.DAL.DAO
{
    public class ShellEffectDAO : IShellEffectDAO
    {
        #region Methods

        public DeleteResult DeleteByEquipmentSerialId(Guid id, bool isRune)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var deleteentities = context.ShellEffect.Where(s => s.EquipmentSerialId == id && s.IsRune == isRune)
                        .ToList();
                    if (deleteentities.Count != 0)
                    {
                        context.ShellEffect.RemoveRange(deleteentities);
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

        public ShellEffectDTO InsertOrUpdate(ShellEffectDTO shelleffect)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var shelleffectId = shelleffect.ShellEffectId;
                    var entity = context.ShellEffect.FirstOrDefault(c => c.ShellEffectId.Equals(shelleffectId));

                    if (entity == null) return insert(shelleffect, context);
                    return update(entity, shelleffect, context);
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Language.Instance.GetMessageFromKey("INSERT_ERROR"), shelleffect, e.Message),
                    e);
                return shelleffect;
            }
        }

        public void InsertOrUpdateFromList(List<ShellEffectDTO> shellEffects, Guid equipmentSerialId)
        {
            try
            {
                if (!shellEffects.Any()) return;

                using (var context = DataAccessHelper.CreateContext())
                {
                    void insert(ShellEffectDTO shelleffect)
                    {
                        var _entity = new ShellEffect();
                        ShellEffectMapper.ToShellEffect(shelleffect, _entity);
                        context.ShellEffect.Add(_entity);
                        context.SaveChanges();
                        shelleffect.ShellEffectId = _entity.ShellEffectId;
                    }

                    void update(ShellEffect _entity, ShellEffectDTO shelleffect)
                    {
                        if (_entity != null) ShellEffectMapper.ToShellEffect(shelleffect, _entity);
                    }


                    foreach (var item in shellEffects)
                    {
                        item.EquipmentSerialId = equipmentSerialId;
                        var entity = context.ShellEffect.FirstOrDefault(c => c.ShellEffectId == item.ShellEffectId);

                        if (entity == null)
                            insert(item);
                        else
                            update(entity, item);
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public IEnumerable<ShellEffectDTO> LoadByEquipmentSerialId(Guid id, bool isRune)
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<ShellEffectDTO>();
                foreach (var entity in context.ShellEffect.Where(c => c.EquipmentSerialId == id && c.IsRune == isRune))
                {
                    var dto = new ShellEffectDTO();
                    ShellEffectMapper.ToShellEffectDTO(entity, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        private static ShellEffectDTO insert(ShellEffectDTO shelleffect, OpenNosContext context)
        {
            var entity = new ShellEffect();
            ShellEffectMapper.ToShellEffect(shelleffect, entity);
            context.ShellEffect.Add(entity);
            context.SaveChanges();
            if (ShellEffectMapper.ToShellEffectDTO(entity, shelleffect)) return shelleffect;

            return null;
        }

        private static ShellEffectDTO update(ShellEffect entity, ShellEffectDTO shelleffect, OpenNosContext context)
        {
            if (entity != null)
            {
                ShellEffectMapper.ToShellEffect(shelleffect, entity);
                context.SaveChanges();
            }

            if (ShellEffectMapper.ToShellEffectDTO(entity, shelleffect)) return shelleffect;

            return null;
        }

        #endregion
    }
}