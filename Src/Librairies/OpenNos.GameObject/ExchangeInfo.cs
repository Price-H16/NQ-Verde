using System.Collections.Generic;

namespace OpenNos.GameObject
{
    public class ExchangeInfo
    {
        #region Instantiation

        public ExchangeInfo()
        {
            Confirmed = false;
            Gold = 0;
            TargetCharacterId = -1;
            ExchangeList = new List<ItemInstance>();
            Validated = false;
        }

        #endregion

        #region Properties

        public bool Confirmed { get; set; }

        public List<ItemInstance> ExchangeList { get; set; }

        public long Gold { get; set; }

        public long BankGold { get; set; }

        public long TargetCharacterId { get; set; }

        public bool Validated { get; set; }

        #endregion
    }
}