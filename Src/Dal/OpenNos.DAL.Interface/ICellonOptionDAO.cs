using System;
using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface ICellonOptionDAO
    {
        #region Methods

        DeleteResult DeleteByEquipmentSerialId(Guid id);

        IEnumerable<CellonOptionDTO> GetOptionsByWearableInstanceId(Guid wearableInstanceId);

        CellonOptionDTO InsertOrUpdate(CellonOptionDTO cellonOption);

        void InsertOrUpdateFromList(List<CellonOptionDTO> cellonOption, Guid equipmentSerialId);

        #endregion
    }
}