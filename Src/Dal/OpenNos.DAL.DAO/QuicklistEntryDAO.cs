using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using OpenNos.Data.Enums;
using OpenNos.Mapper.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.DAL.DAO
{
    public class QuicklistEntryDAO : IQuicklistEntryDAO
    {
        #region Methods

        public DeleteResult Delete(Guid id)
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var entity = context.Set<QuicklistEntry>().FirstOrDefault(i => i.Id == id);
                if (entity != null)
                {
                    context.Set<QuicklistEntry>().Remove(entity);
                    context.SaveChanges();
                }

                return DeleteResult.Deleted;
            }
        }

        public IEnumerable<QuicklistEntryDTO> InsertOrUpdate(IEnumerable<QuicklistEntryDTO> dtos)
        {
            try
            {
                IList<QuicklistEntryDTO> results = new List<QuicklistEntryDTO>();
                using (var context = DataAccessHelper.CreateContext())
                {
                    foreach (var dto in dtos) results.Add(InsertOrUpdate(context, dto));
                }

                return results;
            }
            catch (Exception e)
            {
                Logger.Error($"Message: {e.Message}", e);
                return Enumerable.Empty<QuicklistEntryDTO>();
            }
        }

        public QuicklistEntryDTO InsertOrUpdate(QuicklistEntryDTO dto)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    return InsertOrUpdate(context, dto);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Message: {e.Message}", e);
                return null;
            }
        }

        public IEnumerable<QuicklistEntryDTO> LoadByCharacterId(long characterId)
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<QuicklistEntryDTO>();
                foreach (var QuicklistEntryobject in context.QuicklistEntry.Where(i => i.CharacterId == characterId))
                {
                    var quicklistEntryDTO = new QuicklistEntryDTO();
                    QuicklistEntryMapper.ToQuicklistEntryDTO(QuicklistEntryobject, quicklistEntryDTO);
                    result.Add(quicklistEntryDTO);
                }

                return result;
            }
        }

        public QuicklistEntryDTO LoadById(Guid id)
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var quicklistEntryDTO = new QuicklistEntryDTO();
                if (QuicklistEntryMapper.ToQuicklistEntryDTO(
                    context.QuicklistEntry.FirstOrDefault(i => i.Id.Equals(id)), quicklistEntryDTO))
                    return quicklistEntryDTO;

                return null;
            }
        }

        public IEnumerable<Guid> LoadKeysByCharacterId(long characterId)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    return context.QuicklistEntry.Where(i => i.CharacterId == characterId).Select(qle => qle.Id)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        protected static QuicklistEntryDTO Insert(QuicklistEntryDTO dto, OpenNosContext context)
        {
            var entity = new QuicklistEntry();
            QuicklistEntryMapper.ToQuicklistEntry(dto, entity);
            context.Set<QuicklistEntry>().Add(entity);
            context.SaveChanges();
            if (QuicklistEntryMapper.ToQuicklistEntryDTO(entity, dto)) return dto;

            return null;
        }

        protected static QuicklistEntryDTO InsertOrUpdate(OpenNosContext context, QuicklistEntryDTO dto)
        {
            var primaryKey = dto.Id;
            var entity = context.Set<QuicklistEntry>().FirstOrDefault(c => c.Id == primaryKey);
            if (entity == null)
                return Insert(dto, context);
            return Update(entity, dto, context);
        }

        protected static QuicklistEntryDTO Update(QuicklistEntry entity, QuicklistEntryDTO inventory,
            OpenNosContext context)
        {
            if (entity != null)
            {
                QuicklistEntryMapper.ToQuicklistEntry(inventory, entity);
                context.SaveChanges();
            }

            if (QuicklistEntryMapper.ToQuicklistEntryDTO(entity, inventory)) return inventory;

            return null;
        }

        #endregion
    }
}