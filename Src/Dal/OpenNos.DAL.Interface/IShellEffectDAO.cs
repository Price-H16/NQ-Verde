using System;
using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface IShellEffectDAO
    {
        #region Methods

        DeleteResult DeleteByEquipmentSerialId(Guid id, bool isRune = false);

        ShellEffectDTO InsertOrUpdate(ShellEffectDTO shelleffect);

        void InsertOrUpdateFromList(List<ShellEffectDTO> shellEffects, Guid equipmentSerialId);

        IEnumerable<ShellEffectDTO> LoadByEquipmentSerialId(Guid id, bool isRune = false);

        #endregion
    }
}