using System.Collections.Generic;
using OpenNos.Data;

namespace OpenNos.DAL.Interface
{
    public interface IMaintenanceLogDAO
    {
        #region Methods

        MaintenanceLogDTO Insert(MaintenanceLogDTO maintenanceLog);

        IEnumerable<MaintenanceLogDTO> LoadAll();

        MaintenanceLogDTO LoadFirst();

        #endregion
    }
}