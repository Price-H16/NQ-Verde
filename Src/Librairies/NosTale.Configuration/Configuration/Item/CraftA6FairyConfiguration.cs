using System.Collections.Generic;

namespace NosTale.Configuration.Configuration.Item
{
    public struct CraftA6FairyConfiguration
    {
        #region Properties

        // Just a Simple configuration
        public int[] FairyVnum { get; set; }

        public int[] GoldPrice { get; set; }

        public List<RequiredItem>[] Item { get; set; }

        public int[] PercentSucess { get; set; }

        public int[] SuccesVnumFairy { get; set; }

        #endregion
    }
}