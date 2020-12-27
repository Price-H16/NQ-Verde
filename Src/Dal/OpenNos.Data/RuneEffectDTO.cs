using System;
using ChickenAPI.Enums.Game.BCard;

namespace OpenNos.Data
{
    public class RuneEffectDTO
    {
        public int RuneEffectId { get; set; }

        public Guid EquipmentSerialId { get; set; }

        public BCardType Type { get; set; }

        public byte SubType { get; set; }

        public int FirstData { get; set; }

        public int SecondData { get; set; }

        public int ThirdData { get; set; }

        public bool IsPower { get; set; }


        public RuneEffectDTO DeepCopy() => (RuneEffectDTO)MemberwiseClone();
    }
}
