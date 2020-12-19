using System;
using System.Collections.Generic;
using System.Linq;
using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using OpenNos.Mapper.Mappers;

namespace OpenNos.DAL.DAO
{
    public class DropDAO : IDropDAO
    {
        #region Methods

        public void Insert(List<DropDTO> drops)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    foreach (var Drop in drops)
                    {
                        var entity = new Drop();
                        DropMapper.ToDrop(Drop, entity);
                        context.Drop.Add(entity);
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

        public DropDTO Insert(DropDTO drop)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var entity = new Drop();
                    context.Drop.Add(entity);
                    context.SaveChanges();
                    if (DropMapper.ToDropDTO(entity, drop)) return drop;

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public List<DropDTO> LoadAll()
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<DropDTO>();
                foreach (var entity in context.Drop)
                {
                    var dto = new DropDTO();
                    DropMapper.ToDropDTO(entity, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        public IEnumerable<DropDTO> LoadByMonster(short monsterVNum)
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<DropDTO>();

                foreach (var Drop in context.Drop.Where(s => s.MonsterVNum == monsterVNum || s.MonsterVNum == null))
                {
                    var dto = new DropDTO();
                    DropMapper.ToDropDTO(Drop, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        #endregion
    }
}