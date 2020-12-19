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
    public class MaintenanceLogDAO : IMaintenanceLogDAO
    {
        #region Methods

        public MaintenanceLogDTO Insert(MaintenanceLogDTO maintenanceLog)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var entity = new MaintenanceLog();
                    MaintenanceLogMapper.ToMaintenanceLog(maintenanceLog, entity);
                    context.MaintenanceLog.Add(entity);
                    context.SaveChanges();
                    if (MaintenanceLogMapper.ToMaintenanceLogDTO(entity, maintenanceLog)) return maintenanceLog;

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<MaintenanceLogDTO> LoadAll()
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<MaintenanceLogDTO>();
                foreach (var maintenanceLog in context.MaintenanceLog)
                {
                    var dto = new MaintenanceLogDTO();
                    MaintenanceLogMapper.ToMaintenanceLogDTO(maintenanceLog, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        public MaintenanceLogDTO LoadFirst()
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var dto = new MaintenanceLogDTO();
                    if (MaintenanceLogMapper.ToMaintenanceLogDTO(
                        context.MaintenanceLog.FirstOrDefault(m => m.DateEnd > DateTime.Now), dto)) return dto;

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        #endregion
    }
}