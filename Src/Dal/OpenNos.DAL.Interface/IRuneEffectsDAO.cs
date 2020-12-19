using OpenNos.Data;
using OpenNos.Data.Enums;
using System;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface IRuneEffectsDAO
    {
        #region Methods

        DeleteResult DeleteByEquipmentSerialId(Guid id);

        RuneEffectsDTO InsertOrUpdate(RuneEffectsDTO runeEffect);

        void InsertOrUpdateFromList(List<RuneEffectsDTO> runeEffects, Guid equipmentSerialId);

        IEnumerable<RuneEffectsDTO> LoadByEquipmentSerialId(Guid id);

        #endregion
    }
}