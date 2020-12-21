using OpenNos.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.Interface
{
    public interface IRuneEffectDAO
    {
        RuneEffectDTO InsertOrUpdate(RuneEffectDTO dto);
        IEnumerable<RuneEffectDTO> LoadByEquipmentSerialId(Guid id);
        void InsertOrUpdateFromList(List<RuneEffectDTO> runeEffects, Guid equipmentSerialId);
        void DeleteByEquipmentSerialId(Guid equipmentSerialId);
    }
}
