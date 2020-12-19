using OpenNos.DAL.EF;
using OpenNos.DAL.Interface.PropertiesMapping;
using OpenNos.Data;
using OpenNos.Mapper.Props;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Mapper.Mappers
{
    public static class RuneEffectsMapper
    {
        #region Methods

        public static bool ToRuneEffect(RuneEffectsDTO input, RuneEffects output)
        {
            if (input == null)
            {
                return false;
            }
            output.RuneEffectId = input.RuneEffectId;
            output.EquipmentSerialId = input.EquipmentSerialId;
            output.Type = input.Type;
            output.SubType = input.SubType;
            output.FirstData = input.FirstData;
            output.SecondData = input.SecondData;
            output.ThirdDada = input.ThirdDada;

            return true;
        }

        public static bool ToRuneEffectDTO(RuneEffects input, RuneEffectsDTO output)
        {
            if (input == null)
            {
                return false;
            }
            output.RuneEffectId = input.RuneEffectId;
            output.EquipmentSerialId = input.EquipmentSerialId;

            output.Type = input.Type;
            output.SubType = input.SubType;
            output.FirstData = input.FirstData;
            output.SecondData = input.SecondData;
            output.ThirdDada = input.ThirdDada;

            return true;
        }

        #endregion
    }
}